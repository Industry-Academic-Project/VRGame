using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombYangGeng : MonoBehaviour
{
    // 폭발 효과
    private Transform explosion;
    private ParticleSystem expEffect;
    private AudioSource expAudio;
    // 폭발 영역
    public float range = 5f;
    
    void Start()
    {
        // 씬에서 Explosion 객체를 찾아 transform 가져오기
        explosion = GameObject.Find("Explosion").transform;
        // Explosion 객체의 ParticleSystem 컴포넌트 얻어오기
        expEffect = explosion.GetComponent<ParticleSystem>();
        // Explosion 객체의 AudioSource 컴포넌트 얻어오기
        expAudio = explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 레이어 마스크 가져오기
        int layerMask = 1 << LayerMask.NameToLayer("Drone");
        // 폭탄을 중심으로 range 크기의 반경 안에 들어온 드론 검사
        Collider[] drones = Physics.OverlapSphere(transform.position, range, layerMask);
        // 영역 안에 있는 드론을 모두 제거
        foreach (Collider drone in drones)
        {
            Destroy(drone.gameObject);
        }
        // 폭발 효과의 위치 지정
        explosion.position = transform.position;
        // 이펙트 재생
        expEffect.Play();
        // 이펙트 사운드 재생
        expAudio.Play();
        // 폭탄 없애기
        Destroy(gameObject);
    }
}
