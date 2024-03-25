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
    
    [Header("���â�� Cell������ ���")] 
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

    // �κ��丮 ĭ������ �̹����� �����ϴ� �Լ�
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

    // �������� Ŭ������ �� (Item_Button)
    public void OnButtonClicked()
    {
        //���� ��尡 �������� ��
        if (InventoryManager.Instance.isSelectMode)
        {
            //����� �������� ���õ��� ����
            if (!icon)
            {
                //���õ��� �ʾҴٸ� ���õ� �����ۿ� �߰�
                if (!IsSelected)
                {
                    InventoryManager.Instance.selectedCells.Add(this);
                    IsSelected = true;
                }
                //�̹� ���õǾ��ִٸ� ���õ� �����ۿ��� ����
                else
                {
                    InventoryManager.Instance.selectedCells.Remove(this);
                    IsSelected = false;
                }
            }
        }
        //���� ��尡 �������� ���� �� �� ������ Ŵ
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

    // ��޿� ���� �׵θ� ������ �����ϴ� �Լ�
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
                Debug.LogError("InventoryCell : enum.rarity ���� �ʰ�");
                break;
        }
    }
}
