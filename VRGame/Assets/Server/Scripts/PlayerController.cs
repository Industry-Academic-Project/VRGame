using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Server
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IDamageable
    {
        private Rigidbody _rigidbody;
        
        // 포톤 뷰, 이 컴포넌트가 남에게 속한 것인지 아닌지를 판별할 수 있다.
        public PhotonView pView;
        private Material _characterMaterial;
        private Vector3 _currPos;
        
        public float moveSpeed = 8.0f;
        public float jumpPower = 10000.0f;
        
        // 점프 중복을 막기 위한 변수
        private bool _isJumping;

        [SerializeField] private float lookXLimit = 80f;
        private float _rotationX;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private GameObject camHolder;
        
        #region Unity Event Functions
        private void Awake()
        {
            _characterMaterial = GetComponent<MeshRenderer>().material;
            
            // 자기 자신만 초록색이고 나머지는 빨간색으로 보인다.
            _characterMaterial.color = pView.IsMine ? Color.green : Color.red;
            
            if (pView.IsMine == false)
            {
                // 남의 컴포넌트(상호작용 필요 없는 것만) 파괴 (모든 컴퓨터에서 자기 것의 컴포넌트만 유지됨)
                Destroy(GetComponent<AudioListener>());
                Destroy(GetComponentInChildren<Camera>());
                Destroy(healthText.transform.parent.gameObject);
                
                // 파괴하면 다른 플레이어와 상호작용을 못 한다.
                // Destroy(this);

                return;
            }
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            if (pView.IsMine == false) return;

            // 커서 잠그기
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (pView.IsMine == false)
            {
                // 상대방 위치 동기화
                #region Synchronize Position
            
                // 너무 멀 때는 순간이동
                if ((transform.position - _currPos).sqrMagnitude >= 100f)
                {
                    transform.position = _currPos;
                }
                else
                {
                    // 부드럽게 이동
                    transform.position = Vector3.Lerp(transform.position, _currPos, Time.deltaTime * 10);
                }
            
                #endregion

                return;
            }
            
            // 점프 (스페이스바)
            if (Input.GetKeyDown(KeyCode.Space) && _isJumping == false)
            {
                pView.RPC(nameof(JumpRPC), RpcTarget.All);
            }

            // 커서 잠금/잠금 해제
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                // lockState가 Lock이면 none, none이면 locked로 바뀜
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                    ? CursorLockMode.None
                    : CursorLockMode.Locked;

                // 커서 보인다면 숨기고, 숨겨졌다면 보이기
                Cursor.visible = !Cursor.visible;
            }
            
            // 1인칭 카메라 회전
            #region Camera Rotation
            
            // x축 카메라 회전 (상하)
            _rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            camHolder.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            
            // y축 플레이어 회전 (좌우)
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);

            #endregion
        }

        // 리지드바디 다루는 것은 FixedUpdate에서 실행해야 움직이는 것에 끊김이 덜하다.
        private void FixedUpdate()
        {
            if (pView.IsMine == false)
            {
                return;
            }
            
            // 움직임 실행
            #region Movement
            
            // WASD
            var horizontalMovement = Input.GetAxisRaw("Horizontal");
            var verticalMovement = Input.GetAxisRaw("Vertical");

            // 움직일 방향 계산
            Vector3 moveVelocity = (transform.right * horizontalMovement + transform.forward * verticalMovement).normalized;
            
            // 속도 곱
            moveVelocity *= moveSpeed;

            _rigidbody.MovePosition(transform.position + moveVelocity * Time.fixedDeltaTime);
            
            #endregion
        }
        
        private void OnCollisionEnter(Collision other)
        {
            // 콜리전에 닿았을 때 점프를 할 수 있는지 체크
            // 플레이어라면 점프하지 못하게 막는다
            if (pView.IsMine == false || other.gameObject.CompareTag("Player"))
            {
                return;
            }

            _isJumping = false;
        }

        private void OnCollisionExit(Collision other)
        {
            // 콜리전에서 떨어질 때 점프를 못하게 되는지 체크
            // 플레이어라면 점프 비활성화를 막는다
            if (pView.IsMine == false || other.gameObject.CompareTag("Player"))
            {
                return;
            }

            _isJumping = true;
        }
        
        #endregion
        
        #region private functions
        
        // RPC는 이 스크립트가 있는 (다른 컴퓨터에) 복사된 자기 오브젝트에 명령하는 것임.
        // 그래야 복사된 자기 플레이어를 알아보고 그걸 복사된 스크립트에 실행할 수 있음.  
        // 다른 플레이어가 볼 때 IsMine이 아니면서 나의 pView를 가진오브젝트에서 실행하는 것.
        [PunRPC]
        private void JumpRPC()
        {
            if (_isJumping) return;
            _isJumping = true;
            Debug.Log("someone is jumping");
            // _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * jumpPower);
        }

        [PunRPC]
        private void DestroyRPC() => Destroy(gameObject);

        [PunRPC]
        public void GetDamage(float amount)
        {
            Health -= amount;
            Debug.Log($"Hit! hit player's health is now: {Health}");
        }


        #endregion

        #region IPunObservable
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 스트림 동기화
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(Health);
            }
            else
            {
                _currPos = (Vector3)stream.ReceiveNext();
                Health = (float)stream.ReceiveNext();
            }
        }
        #endregion
        
        #region IDamagable

        [SerializeField] private Text healthText;
        [SerializeField] private float health;
        public float Health
        {
            get => health;
            set
            {
                // 본인만 데미지 입음
                if (pView.IsMine == false) return;
                
                health = value;
                healthText.text = $"+ {health}";
                if (health > 0f) return;
                
                // 죽음 로직 (체력이 0 초과이면 실행되지 않음)
                health = 0f;
                OnDeath();
            }
        }

        public void OnDeath()
        {
            if (pView.IsMine == false)
            {
                return;
            }
            // 죽을 때 실행
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameObject.Find("Canvas").transform.Find("Respawn Panel").gameObject.SetActive(true);
            
            pView.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered);
        }
        #endregion
    }

}
