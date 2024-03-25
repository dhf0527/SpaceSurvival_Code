using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI_Animated : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Awake() => anim = GetComponent<Animator>();

    // Update is called once per frame
    void Update() => SkipAnimation();

    // 화면을 클릭하여 애니메이션을 스킵하는 함수
    void SkipAnimation()
    {
        if (Input.GetMouseButtonUp(0))
            anim.speed = 100;
    }

    //스킵할 때 바꾸었던 speed를 원래대로
    private void OnEnable() => anim.speed = 1;
}
