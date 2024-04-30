using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletImpact; // 총알 파편 효과
    private ParticleSystem bulletEffect; // 총알 파편 파티클 시스템
    private AudioSource bulletAudio; // 총알 발사 사운드
    
    public Transform firePosition;
    private LineRenderer bulletLineRenderer; // 탄알 궤적을 그리기 위한 렌더러

    public GameObject Magazine;
    public GameObject Laser;
    public int BulletNumber = 12;

    private float fireDistance = 50f; // 사정거리
    private bool fireDelay = false;
    private bool isReload = false;

    
    // crosshair를 위한 속성
    public Transform crosshair;
    
    void Start()
    {
        // 총알 효과 파티클 시스템 컴포넌트 가져오기
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        // 총알 효과 오디오 소스 컴포넌트 가져오기
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }
    
    void Awake()
    {
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    void Update()
    {
        // 크로스 헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);
        
        if((Input.GetKeyDown(KeyCode.R) || BulletNumber <= 0) && !isReload) StartCoroutine(ReloadBullet());

        // 사용자가 indexTrigger 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            FireBullet();
            // 컨트롤러의 진동 재생
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);
            // 총알 오디오 재생
            bulletAudio.Stop();
            bulletAudio.Play();
            
            // Ray를 카메라의 위치로부터 나가도록 만든다
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            // Ray의 충돌정보를 저장하기 위한 변수 지정
            RaycastHit hitInfo;
            // 플레이어 레이어 얻어오기
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            // 타워 레이어 얻어오기
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMask = playerLayer | towerLayer;
            // Ray를 쏜다. ray가 부딪힌 정보는 hitInfo에 담긴다.
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                // 총알 파편 효과 처리
                // 총알 이펙트 진행되고 있으면 멈추고 재생
                bulletEffect.Stop();
                bulletEffect.Play();
                
                
                // 부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
                bulletImpact.position = hitInfo.point;
                // 부딛힌 지점의 방향으로 총알 이펙트의 방향을 설정
                bulletImpact.forward = hitInfo.normal;
                
                // ray와 부딪힌 객체가 drone이라면 피격 처리
                if (hitInfo.transform.name.Contains("Drone"))
                {
                    DroneAI drone = hitInfo.transform.GetComponent<DroneAI>();
                    if (drone)
                    {
                        drone.OnDamageProcess();
                    }
                }
            }
        }
    }
    
    void FireBullet()
    {
        if (fireDelay) return;
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;

        //(시작지점, 발사 방향, 충돌 정보 컨테이너, 사정거리)
        if (Physics.Raycast(firePosition.position, firePosition.forward, out hit, fireDistance))
        {
            hitPosition = hit.point; //맞은 곳 저장
            if (hit.transform.gameObject.tag == "Enemy")
            {
                Debug.Log("Hit");
                HitPart hitScript = hit.transform.gameObject.GetComponent<HitPart>();
                hitScript.OnDamage();
            }
        }
        else
        {
            //충돌 안함 -> 탄알 최대 사정거리 위치를 충돌 위치로 사용
            hitPosition = firePosition.position + firePosition.forward * fireDistance;
        }
    
        StartCoroutine(FireBulletEffect(hitPosition));

        BulletNumber--;
        Debug.Log("Fire");
    }

    IEnumerator FireBulletEffect(Vector3 hitPosition)
    {
    
        bulletLineRenderer.SetPosition(0,firePosition.position);
        bulletLineRenderer.SetPosition(1,hitPosition);
    
        // 라인 렌더러를 활성화하여 탄알 궤적을 그림
        bulletLineRenderer.enabled = true;

        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.1f);

        // 라인 렌더러를 비활성화하여 탄알 궤적을 지움
        bulletLineRenderer.enabled = false;
    }

    IEnumerator ReloadBullet()
    {
        if(isReload) yield break;
        isReload = true;
        Debug.Log("Reload");
        fireDelay = true;
        // Debug.Log(gameObject.GetComponent<Transform>().transform.localPosition);
        Magazine.GetComponent<Transform>().transform.localPosition = new Vector3(0,-2f,0);
        yield return new WaitForSeconds(2.0f);
        BulletNumber = 12;
        fireDelay = false;
        Magazine.GetComponent<Transform>().transform.localPosition = new Vector3(0,-0.5f,0);
        isReload = false;
        Debug.Log("Finish");
    }
}
