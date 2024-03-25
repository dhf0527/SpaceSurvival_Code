using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using TMPro;

#region 아이템 class, struct
[System.Serializable]
public class ItemData
{
    public ItemStaticData itemStaticData;
    public Enum_GM.Rarity rarity;
    public List<Item_Ability> abilities;

    public ItemData(ItemStaticData itemStaticData, Enum_GM.Rarity rarity, List<Item_Ability> abilities)
    {
        this.itemStaticData = itemStaticData;
        this.rarity = rarity;
        this.abilities = abilities;
    }
}

[System.Serializable]
//아이템 종류별 특징 (이름이나 장착 부위같이 종류가 다를 경우에만 달라지는 특징)
public class ItemStaticData
{
    public string name;
    public Enum_GM.ItemPlace place;
    public PlayerWeapon weaponKind;
    public string spriteName;
    public string description;

    public ItemStaticData(string name, Enum_GM.ItemPlace place, PlayerWeapon weaponKind, string spriteName, string description)
    {
        this.name = name;
        this.place = place;
        this.weaponKind = weaponKind;
        this.spriteName = spriteName;
        this.description = description;
    }
}

//아이템 능력치
public struct Item_Ability
{
    public Enum_GM.abilityName abilityName;
    public float abilityValue;
    public Enum_GM.Rarity abilityrarity;
}
#endregion
    
public class InventoryManager : MonoBehaviour
{
    #region Hierarchy에서 넣어주어야 할 것들
    [Header("ItemDatas(ScriptableObj)")]
    [SerializeField] List<ItemScriptableData> isdList;

    [SerializeField] TMP_Text totalAb_Text;
    [SerializeField] TMP_Dropdown dropdown;
    public Inventory_UI inventory_UI;
    public ItemDetail itemDetail;
    public ItemDetail equipmentDetail;

    [Header("장비칸 (순서-무기,옷,신발,귀고리,반지,펫)")]
    public List<InventoryCell> equipCells = new List<InventoryCell>();

    [Header("골드")]
    [SerializeField] TMP_Text lobbyGold_Text;
    [SerializeField] TMP_Text shopGold_Text;
    #endregion

    //인벤토리에 들어있는 아이템들 리스트
    [HideInInspector] public List<ItemData> inventoryDatas = new List<ItemData>();
    //부위별 장착 아이템
    [HideInInspector] public Dictionary<Enum_GM.ItemPlace, ItemData> d_equipments = new Dictionary<Enum_GM.ItemPlace, ItemData>();
    //능력별 능력치 증가값
    [HideInInspector] public Dictionary<Enum_GM.abilityName, float> d_totalAb = new Dictionary<Enum_GM.abilityName, float>();
    //선택모드인지 나타내는 변수
    [HideInInspector] public bool isSelectMode = false;
    //선택모드에서 선택한 아이템들 리스트
    [HideInInspector] public List<InventoryCell> selectedCells = new List<InventoryCell>();
    //골드
    private int gold = 300;
    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            lobbyGold_Text.text = value.ToString();
            shopGold_Text.text = value.ToString();
            gold = value;
        }
    }
    
    //정렬 기준
    Enum_GM.SortBy sortBy = Enum_GM.SortBy.name;

    //선택한 아이템(선택모드x)
    public ItemData SelectedItem 
    { 
        get 
        { 
            return selectedItem; 
        } 
        set 
        {
            itemDetail.SetDetails(value);
            equipmentDetail.SetDetails(value);
            selectedItem = value;
        } 
    }
    private ItemData selectedItem;

    #region 싱글톤(Awake 포함)
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static InventoryManager Instance
    {
        get { return instance; }
    }
    #endregion

    private void Start()
    {
        LoadInventory();
        LoadEquipment();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem(RandomItem());
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Gold += 500;
        }
    }

    #region 아이템 추가/삭제
    // 랜덤 아이템 반환 함수
    public ItemData RandomItem()
    {
        int rand = Random.Range(0, isdList.Count);
        ItemScriptableData isd = isdList[rand];
        ItemStaticData newIsData = new ItemStaticData(isd.ItemName, isd.Place, isd.WeaponKind, isd.SpriteName, isd.Description);

        List<Item_Ability> newItemAbs = new List<Item_Ability>();

        //랜덤으로 어빌리티의 종류/수치를 결정함
        //Random.Range(최소 개수, 최대 개수+1)
        for (int i = 0; i < Random.Range(2,6); i++)
        {
            Item_Ability item_Ab = new Item_Ability();

            int rand_Name = Random.Range(0, 4);
            item_Ab.abilityName = (Enum_GM.abilityName)rand_Name;

            int rand_Rare = Random.Range(0, 100);

            #region rand_Rare에 따른 abilityValue 기본값 설정

            if (rand_Rare < 50)
            {
                item_Ab.abilityrarity = Enum_GM.Rarity.normal;
                item_Ab.abilityValue = 6;
            }
            else if (rand_Rare < 80)
            {
                item_Ab.abilityrarity = Enum_GM.Rarity.rare;
                item_Ab.abilityValue = 10;
            }
            else if (rand_Rare < 95)
            {
                item_Ab.abilityrarity = Enum_GM.Rarity.unique;
                item_Ab.abilityValue = 13;
            }
            else
            {
                item_Ab.abilityrarity = Enum_GM.Rarity.legendary;
                item_Ab.abilityValue = 15;
            }
            #endregion

            //abilityValue 보정값
            item_Ab.abilityValue += Random.Range(0, 4);
            newItemAbs.Add(item_Ab);
            //인수에 들어있는 람다식에 따라 리스트를 정렬
            //-> abilityValue값이 크면 앞으로 정렬
            newItemAbs.Sort((Item_Ability ab_A, Item_Ability ab_B) => ab_B.abilityValue.CompareTo(ab_A.abilityValue));
        }

        //아이템 자체의 희귀도 (가장 높은 등급의 ability 희귀도를 따라감)
        Enum_GM.Rarity newRarity = (Enum_GM.Rarity)3;
        foreach (var item in newItemAbs)
        {
            if (item.abilityrarity < newRarity)
                newRarity = item.abilityrarity;
        }

        return new ItemData(newIsData, newRarity, newItemAbs);
    }

    public void AddItem(ItemData id)
    {
        inventoryDatas.Add(id);
        inventory_UI?.OnCellsEnable();
    }

    // 아이템 삭제 함수(ItemDetail - ItemDestroy 버튼)
    public void OnRemoveItem()
    {
        inventoryDatas.Remove(selectedItem);
        inventory_UI?.OnCellsEnable();
    }
    #endregion

    #region 아이템 데이터 저장/불러오기
    public void SaveInventory()
    {
        string jitems = JsonConvert.SerializeObject(inventoryDatas, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/Inventory.json", jitems);
    }

    void LoadInventory()
    {
        string str = File.ReadAllText(Application.dataPath + "/Inventory.json");
        List<ItemData> itemDatas_json = JsonConvert.DeserializeObject<List<ItemData>>(str);
        foreach (var item in itemDatas_json)
        {
            inventoryDatas.Add(new ItemData(item.itemStaticData , item.rarity, item.abilities));
        }
    }

    public void SaveEquipment()
    {
        string jEquipments = JsonConvert.SerializeObject(d_equipments, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/Equipments.json", jEquipments);
    }

    private void LoadEquipment()
    {
        string str = File.ReadAllText(Application.dataPath + "/Equipments.json");
        Dictionary<Enum_GM.ItemPlace, ItemData> equipmentDatas_json = JsonConvert.DeserializeObject<Dictionary<Enum_GM.ItemPlace, ItemData>>(str);
        foreach (var item in equipmentDatas_json)
        {
            d_equipments.Add(item.Key ,(new ItemData(item.Value.itemStaticData, item.Value.rarity, item.Value.abilities)));
            PutCellData(item.Key);
        }
    }
    #endregion

    #region 장비 장착/해제
    // 장비 장착(버튼)
    public void OnEquip()
    {
        //장착중인 아이템이 있다면 해제
        if (d_equipments.ContainsKey(SelectedItem.itemStaticData.place))
            OnTakeOff();

        //장착
        d_equipments.Add(SelectedItem.itemStaticData.place, SelectedItem);
        PutCellData(SelectedItem.itemStaticData.place);
        inventoryDatas.Remove(SelectedItem);
        inventory_UI.OnCellsEnable();
    }

    // 장비 장착 해제(버튼)
    public void OnTakeOff()
    {
        //간략화
        Enum_GM.ItemPlace place = SelectedItem.itemStaticData.place;

        //장착 해제
        inventoryDatas.Add(d_equipments[place]);
        d_equipments.Remove(place);
        PutCellData(place);
        inventory_UI.OnCellsEnable();
    }

    // 장비창 새로고침
    public void SetEquipments()
    {
        foreach (var item in equipCells)
            item.SetImage();

        SetTotalAbTxt();
    }

    // 총 능력치 증가량 표시
    public void SetTotalAbTxt()
    {
        totalAb_Text.text = "";
        foreach (var item in d_totalAb)
        {
            totalAb_Text.text += $"{itemDetail.AbEnumToString(item.Key)} + {item.Value}%" + "\n";
        }
    }

    // 장비 셀에 장비 정보를 넣어주는 함수
    void PutCellData(Enum_GM.ItemPlace place)
    {
        //장비 딕셔너리에 있는 장비 정보와 장비창(셀)을 동기화
        if (d_equipments.ContainsKey(place))
        {
            equipCells[(int)place].cellData = d_equipments[place];

            //어빌리티 총합값 계산
            foreach (var item in equipCells[(int)place].cellData.abilities)
            {
                if (!d_totalAb.TryAdd(item.abilityName, item.abilityValue))
                {
                    d_totalAb[item.abilityName] += item.abilityValue;
                }
            }
        }
        else
        {
            //어빌리티 총합값 계산
            foreach (var item in equipCells[(int)place].cellData.abilities)
            {
                d_totalAb[item.abilityName] -= item.abilityValue;
                if (d_totalAb[item.abilityName] <= 0)
                    d_totalAb.Remove(item.abilityName);
            }

            equipCells[(int)place].cellData = null;
        }
    }
    #endregion

    #region 선택 모드
    // 선택모드에서 선택한 아이템 일괄 삭제
    public void OnRemoveSeletedItem()
    {
        foreach (var item in selectedCells)
        {
            inventoryDatas.Remove(item.cellData);
            item.IsSelected = false;
        }

        selectedCells.Clear();
        inventory_UI.OnCellsEnable();
    }

    // 선택된 아이템 리스트 삭제
    public void SelectModeOff()
    {
        foreach (var item in selectedCells)
            item.IsSelected = false;

        selectedCells.Clear();
    }
    #endregion

    #region 정렬
    // 정렬 기준을 설정하는 함수 - Sort_Dropdown의 OnValueChange
    public void OnSetSortBy()
    {
        switch (dropdown.value)
        {
            case 0:
                sortBy = Enum_GM.SortBy.name;
                break;
            case 1:
                sortBy = Enum_GM.SortBy.rare;
                break;
            default:
                Debug.LogError("잘못된 정렬 기준");
                break;
        }
    }

    // 정렬 기준(sortBy)에 따라 정렬 함수를 호출하는 함수 - Sort_Button의 OnClick
    public void OnSort()
    {
        switch (sortBy)
        {
            case Enum_GM.SortBy.name:
                SortItemByName();
                break;
            case Enum_GM.SortBy.rare:
                SortItemByRare();
                break;
            default:
                Debug.LogError("잘못된 정렬 기준");
                break;
        }
    }

    // 이름순으로 정렬하는 함수
    void SortItemByName()
    {
        inventoryDatas.Sort((ItemData id_A , ItemData id_B) => id_A.itemStaticData.name.CompareTo(id_B.itemStaticData.name));
        inventory_UI.OnCellsEnable();
    }


    // 레어도순으로 정렬하는 함수
    void SortItemByRare()
    {
        inventoryDatas.Sort((ItemData id_A, ItemData id_B) => id_A.rarity.CompareTo(id_B.rarity));
        inventory_UI.OnCellsEnable();
    } 
    #endregion
}
