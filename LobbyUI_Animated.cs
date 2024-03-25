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

    // ȭ���� Ŭ���Ͽ� �ִϸ��̼��� ��ŵ�ϴ� �Լ�
    void SkipAnimation()
    {
        if (Input.GetMouseButtonUp(0))
            anim.speed = 100;
    }

    //��ŵ�� �� �ٲپ��� speed�� �������
    private void OnEnable() => anim.speed = 1;
}
