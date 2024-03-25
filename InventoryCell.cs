using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    [HideInInspector] public ItemData cellData = null;

    public Image itemImage;
    [SerializeField] Image cellBg;
    [SerializeField] Image cellFrame;
    [SerializeField] GameObject itemButton;
    
    [Header("장비창의 Cell에서만 사용")] 
    [SerializeField] GameObject icon;

    bool isSelected = false;
    public bool IsSelected
    {
        get 
        { 
            return isSelected; 
        }
        set 
        {
            isSelected = value;
            cellBg.color = isSelected ? new Color(0.5f, 0.5f, 0.5f, 96f/255f) : new Color(1, 1, 1, 96f/255f);
        }
    }

    // 인벤토리 칸에서의 이미지를 변경하는 함수
    public void SetImage()
    {
        bool isNoData = cellData == null || cellData.itemStaticData.name == "";
        
        itemButton.SetActive(!isNoData);
        if (icon)
            icon.SetActive(isNoData);

        if (!isNoData)
        {
            itemImage.sprite = Resources.Load<Sprite>("ItemIcons/" + cellData.itemStaticData.spriteName);
            if (!icon)
                SetColor(cellData.rarity, cellFrame);
        }
        else if(!icon)
            cellFrame.color = Color.white;
    }

    // 아이템을 클릭했을 때 (Item_Button)
    public void OnButtonClicked()
    {
        //선택 모드가 켜져있을 때
        if (InventoryManager.Instance.isSelectMode)
        {
            //장비한 아이템은 선택되지 않음
            if (!icon)
            {
                //선택되지 않았다면 선택된 아이템에 추가
                if (!IsSelected)
                {
                    InventoryManager.Instance.selectedCells.Add(this);
                    IsSelected = true;
                }
                //이미 선택되어있다면 선택된 아이템에서 제거
                else
                {
                    InventoryManager.Instance.selectedCells.Remove(this);
                    IsSelected = false;
                }
            }
        }
        //선택 모드가 켜져있지 않을 때 상세 설명을 킴
        else
        {
            InventoryManager.Instance.itemDetail.gameObject.SetActive(true);
            InventoryManager.Instance.itemDetail.mask.SetActive(true);
            InventoryManager.Instance.SelectedItem = cellData;
        }
    }

    public void OnSetDetail()
    {
        InventoryManager.Instance.SelectedItem = cellData;
    }

    // 등급에 따라 테두리 색깔을 변경하는 함수
    void SetColor(Enum_GM.Rarity rarity, Image img)
    {
        switch (rarity)
        {
            case Enum_GM.Rarity.legendary:
                img.color = ItemDetail.yellow;
                break;
            case Enum_GM.Rarity.unique:
                img.color = ItemDetail.purple;
                break;
            case Enum_GM.Rarity.rare:
                img.color = ItemDetail.blue;
                break;
            case Enum_GM.Rarity.normal:
                img.color = ItemDetail.white;
                break;
            default:
                Debug.LogError("InventoryCell : enum.rarity 개수 초과");
                break;
        }
    }
}
