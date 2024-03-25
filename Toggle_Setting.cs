using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toggle_Setting : MonoBehaviour
{
    [Header("해당 Toggle의 Panel")]
    [SerializeField] RectTransform panel_rTrans;
    [Header("Panel의 자식")]
    [SerializeField] GameObject panelChild;

    RectTransform toggle_rTrans;
    Toggle toggle;
    Coroutine cor;

    float origintHeight_panel;
    float originHeight_toggle;

    void Start()
    {
        toggle_rTrans = GetComponent<RectTransform>();
        toggle = GetComponent<Toggle>();
        originHeight_toggle = toggle_rTrans.rect.height;
        origintHeight_panel = panel_rTrans.rect.height;

        if (toggle.isOn)
        {
            toggle_rTrans.sizeDelta *= new Vector2(1, 1.5f);
            panel_rTrans.sizeDelta = new Vector2(panel_rTrans.sizeDelta.x, origintHeight_panel * 8);
        }
        panelChild.SetActive(toggle.isOn);
    }

    #region UI크기 변경
    public void OnChangeValue()
    {
        //isON에 따라 목표 사이즈 설정
        Vector2 changedSize = toggle.isOn ? new Vector2(toggle_rTrans.rect.width, originHeight_toggle * 1.5f) : new Vector2(toggle_rTrans.rect.width, originHeight_toggle);

        if (cor != null)
            StopCoroutine(cor);

        cor = StartCoroutine(C_ChangeUISize(changedSize, toggle.isOn));
    }


    // isOn에 따라 UI 크기를 변경하는 함수
    IEnumerator C_ChangeUISize(Vector2 targetSize, bool isOn)
    {
        //  (totalTime)초에 걸쳐 (deltaY)만큼 크기 변경
        float totalTime = 0.2f;
        float deltaY_toggle = originHeight_toggle * 0.5f;
        float deltaY_panel = origintHeight_panel * 7;

        if (isOn)
        {
            //isOn일 때 커짐
            while (toggle_rTrans.sizeDelta.y < targetSize.y)
            {
                toggle_rTrans.sizeDelta += new Vector2(0, deltaY_toggle * (Time.deltaTime / totalTime));
                panel_rTrans.sizeDelta += new Vector2(0, deltaY_panel * (Time.deltaTime / totalTime));
                yield return new WaitForEndOfFrame();
            }
            toggle_rTrans.sizeDelta = new Vector2(toggle_rTrans.sizeDelta.x, targetSize.y);
            panel_rTrans.sizeDelta = new Vector2(panel_rTrans.sizeDelta.x, origintHeight_panel + deltaY_panel);
            panelChild.SetActive(true);
        }
        else
        {
            //isOn일 때 작아짐
            panelChild.SetActive(false);

            while (toggle_rTrans.sizeDelta.y > targetSize.y)
            {
                toggle_rTrans.sizeDelta -= new Vector2(0, deltaY_toggle * (Time.deltaTime / totalTime));
                panel_rTrans.sizeDelta -= new Vector2(0, deltaY_panel * (Time.deltaTime / totalTime));
                yield return new WaitForEndOfFrame();
            }
            toggle_rTrans.sizeDelta = new Vector2(toggle_rTrans.sizeDelta.x, targetSize.y);
            panel_rTrans.sizeDelta = new Vector2(panel_rTrans.sizeDelta.x, origintHeight_panel);
        }
    }
    #endregion
}
