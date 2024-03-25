using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toggle_Main : MonoBehaviour
{
    [Header("�ڱ� �ڽ�")]
    [SerializeField] Toggle toggle;

    [Header("��� �̹���")]
    [SerializeField] Sprite normal_Image;
    [SerializeField] Sprite selected_Image;

    [Header("���ý� Ȱ��ȭ�� ������Ʈ")]
    [SerializeField] GameObject activeObj;


    // Start is called before the first frame update
    void Start()
    {
        toggle.image.sprite = toggle.isOn ? selected_Image : normal_Image;
        activeObj.SetActive(toggle.isOn);
    }

    // IsOn ���� �ٲ���� �� ��� �̹����� �ٲٰ�, UI�� Ȱ��ȭ�ϴ� �Լ�
    public void OnValueChanged()
    {
        toggle.image.sprite = toggle.isOn ? selected_Image : normal_Image;
        activeObj.SetActive(toggle.isOn);
    }
}
