using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneAI : MonoBehaviour
{
    // 드론의 상태 상수 정의
    enum DronState
    {
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }
    // 초기 시작 상태는 Idle로 설정
    private DronState state = DronState.Idle;
    // 대기 상태의 지속 시간
    public float idleDelayTime = 2f;
    // 경과 시간
    private float currentTime;
    // 이동 속도
    public float moveSpeed = 1f;
    // 타워 위치
    private Transform tower;
    // 길 찾기를 수행할 내비게이션 메시 에이전트
    private NavMeshAgent agent;
    // 공격 범위
    public float attackRange = 3f;
    // 공격 지연 시간
    public float attackDelayTime = 2f;
    // 체력
    [SerializeField] private int hp = 3;
    // 폭발 효과
    private Transform explosion;
    private ParticleSystem expEffect;
    private AudioSource expAudio;
    
    void Start()
    {
        // 타워 찾기
        tower = GameObject.Find("Tower").transform;
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        // agent 이동 속도 설정
        agent.speed = moveSpeed;

        explosion = GameObject.Find("Explosion").transform;
        expEffect = explosion.GetComponent<ParticleSystem>();
        expAudio = explosion.GetComponent<AudioSource>();
    }

    void Update()
    {
        Debug.Log($"Current State = {state}");
        switch (state)
        {
            case DronState.Idle:
                Idle();
                break;
            case DronState.Move:
                Move();
                break;
            case DronState.Attack:
                Attack();
                break;
            case DronState.Damage:
                // Damage();
                break;
            case DronState.Die:
                Die();
                break;
        }
    }
    
    // 일정 시간 동안 기다렸다가 상태를 공격으로 전환하기
    private void Idle()
    {
        // 1. 시간이 흘러야 한다.
        currentTime += Time.deltaTime;
        // 2. 만약 경과 시간이 대기 시간을 초과했다면
        if (currentTime > idleDelayTime)
        {
            // 3. 상태를 이동으로 전환
            state = DronState.Move;
            // agent 활성화
            agent.enabled = true; // <--------------
        }
    }

    // 타워를 향해 이동하고 싶다.
    private void Move()
    {
        if (tower == null)
        {
            return;
        }
        // 네비게이션 할 목적지 설정
        agent.SetDestination(tower.position);
        // 공격 범위 안에 들어오면 공격 상태로 전환
        if (Vector3.Distance(transform.position, tower.position) < attackRange)
        {
            state = DronState.Attack;
            // agent의 동작 정지
            agent.enabled = false;
        }
    }

    private void Attack()
    {
        // 1. 시간이 흐른다.
        currentTime += Time.deltaTime;
        // 2. 경과 시간이 공격 지연 시간을 초과하면
        if (currentTime > attackDelayTime)
        {
            // 3. 공격 -> Tower의 HP를 호출해 데미지 처리를 한다.
            Tower.Instance.HP--;
            // 4. 경과 시간 초기화
            currentTime = 0f;
        }

    }

    IEnumerator  Damage()
    {
        // 1. 길 찾기 중지
        agent.enabled = false;
        // 2. 자식 객체의 MeshRenderer에서 재질 얻어오기
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        // 3. 원래 색을 저장
        Color originalColor = mat.color;
        // 4. 재질의 색을 변경
        mat.color = Color.red;
        // 5. 0.1초 기다리기
        yield return new WaitForSeconds(0.1f);
        // 6. 재질의 색을 원래대로 변경
        mat.color = originalColor;
        // 7. 상태를 Idle로 전환
        state = DronState.Idle;
        // 8. 경과 시간 초기화
        currentTime = 0f;
    }

    private void Die()
    {
        
    }
    
    // 피격 상태 알림 이벤트 메서드
    public void OnDamageProcess()
    {
        // 체력을 감소시키고 죽지 않았다면 상태를 데미지로 전환하고 싶다.
        // 1. 체력 감소
        hp--;
        // 2. 만약 죽지 않았다면
        if (hp > 0)
        {
            // 3. 상태를 데미지로 전환
            state = DronState.Damage;
            // 코루틴 호출
            StopAllCoroutines();
            StartCoroutine(Damage());
        }
        else // 죽었다면 폭발 효과를 발생시키고 드론을 없앤다.
        {
            // 폴발 효과의 위치 지정
            explosion.position = transform.position;
            // 이펙트 재생
            expEffect.Play();
            // 이펙트 사운드 재생
            expAudio.Play();
            // 드론 없애기
            Destroy(gameObject);
        }
    }
}
