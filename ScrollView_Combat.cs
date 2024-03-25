using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollView_Combat : MonoBehaviour, IDragHandler , IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Scrollbar scr;
    [SerializeField] Transform content_Trans;

    bool isDrag = false;
    float curValue;
    float targetValue;

    //거리 (scroll에서 다음 패널로 넘어갔을 때 변동하는 value의 수치)
    float distance;

    private void Start()
    {
        int childcnt = content_Trans.childCount;
        distance = 1f / (childcnt - 1);
    }

    private void Update()
    {
        if (!isDrag)
            scr.value = Mathf.Lerp(scr.value, targetValue, 0.1f);
    }

    public void OnBeginDrag(PointerEventData eventData)=> curValue = TargetIndex() * distance;

    public void OnDrag(PointerEventData eventData)=> isDrag = true;
    
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;

        //가까운 패널의 value를 넘겨 자동으로 스크롤하게 하는 함수
        targetValue = TargetIndex() * distance;

        //tagertValue가 바뀌지 않아도(스크롤을 짧게 해도) 빠르게 넘기면 넘어감
        if (curValue == targetValue)
        {
            //오른쪽으로 밀 때(왼쪽으로 이동), 맨끝 예외
            if (eventData.delta.x > 15 && curValue > distance)
                targetValue -= distance;
            //왼쪽으로 밀 때(오른쪽으로 이동), 맨끝 예외
            else if (eventData.delta.x < -15 && curValue + distance < 1.01f)
                targetValue += distance;
        }
    }

    // 더 가까운 패널의 index를 반환하는 함수
    int TargetIndex()
    {
        //몇 번째 패널인지 알기위한 변수
        int targetIndex = (int)(scr.value / distance);
        //나머지 (어느 패널에 더 가까운지 알기위한 변수)
        float remainder = scr.value % distance;

        //다음 패널에 더 가깝다면 index 증가
        if (remainder > distance * 0.5f)
            targetIndex++;

        //맨끝 예외처리
        if (targetIndex >= content_Trans.childCount)
            targetIndex = content_Trans.childCount - 1;

        //스크롤 변경
        return targetIndex;
    }
}
