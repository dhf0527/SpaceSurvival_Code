using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toggle_Main : MonoBehaviour
{
    [Header("자기 자신")]
    [SerializeField] Toggle toggle;

    [Header("토글 이미지")]
    [SerializeField] Sprite normal_Image;
    [SerializeField] Sprite selected_Image;

    [Header("선택시 활성화할 오브젝트")]
    [SerializeField] GameObject activeObj;


    // Start is called before the first frame update
    void Start()
    {
        toggle.image.sprite = toggle.isOn ? selected_Image : normal_Image;
        activeObj.SetActive(toggle.isOn);
    }

    // IsOn 값이 바뀌었을 때 토글 이미지를 바꾸고, UI를 활성화하는 함수
    public void OnValueChanged()
    {
        toggle.image.sprite = toggle.isOn ? selected_Image : normal_Image;
        activeObj.SetActive(toggle.isOn);
    }
}
