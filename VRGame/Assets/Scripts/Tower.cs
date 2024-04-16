using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    // Tower의 싱글턴 객체
    public static Tower Instance = null;
    
    // 데미지 표현할 UI
    public Transform damagedUI;
    public Image damageImage;
    
    // 타워의 최초 HP
    public int initialHP = 10;
    // 내부 hp 변수
    private int _hp = 0;
    // 깜빡거리는 시간
    public float damageTime = 0.1f;
    
    // _hp의 get/set 프로퍼티
    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            // 기존에 진행중인 코루틴 해제
            StopAllCoroutines();
            // 깜빡거림을 처리할 코루틴 호출
            StartCoroutine(DamageEvent());
            
            // hp가 0이하이면 제거
            if (_hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _hp = initialHP;
        // 카메라의 nearClipPlane 값을 기억해 둔다.
        float z = Camera.main.nearClipPlane + 0.01f;
        // damageUI 객체의 부모를 카메라로 설정
        damagedUI.parent = Camera.main.transform;
        // damageUI의 위치를 X, Y는 0, Z 값은 카메라의 near 값으로 설정
        damagedUI.localPosition = new Vector3(0, 0, z);
        // damageImage는 보이지 않도록 초기에 비활성화 해 놓는다.
        damageImage.enabled = false;
    }

    void Update()
    {
        
    }
    
    // 데미지 처리를 위한 코루틴 메서드
    IEnumerator DamageEvent()
    {
        // damageImage 컴포넌트를 활성화
        damageImage.enabled = true;
        // damageTime 만큼 기다린다.
        yield return new WaitForSeconds(damageTime);
        // 다시 원래대로 비활성화 한다.
        damageImage.enabled = false;
    }
}
