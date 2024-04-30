using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerTest : MonoBehaviour
{
    // 이동 속도
    public float speed = 5f;

    // 플레이어 Rigidbody
    public Rigidbody _rigidbody;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 사용자의 입력에 따라 전후좌우로 이동하고 싶다.]
        // 1. 사용자의 입력을 받는다.
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");
        
        // 2. 이동한다.
        transform.Translate(new Vector3(h, 0f, v).normalized * 0.1f);

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
    }
}
