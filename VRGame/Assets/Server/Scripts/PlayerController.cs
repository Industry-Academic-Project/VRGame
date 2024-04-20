using Photon.Pun;
using UnityEngine;

namespace Server
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IDamageable
    {
        private Rigidbody _rigidbody;
        
        // 포톤 뷰, 이 컴포넌트가 남에게 속한 것인지 아닌지를 판별할 수 있다.
        public PhotonView pView;
        private Material _characterMaterial;
        
        public float moveSpeed = 8.0f;
        public float jumpPower = 10000.0f;
        
        // 점프 중복을 막기 위한 변수
        private bool _isJumping;
        
        #region Unity Event Functions
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            _characterMaterial = GetComponent<MeshRenderer>().material;
            
            // 자기 자신만 초록색이고 나머지는 빨간색으로 보인다.
            _characterMaterial.color = pView.IsMine ? Color.green : Color.red;
        }

        private void Update()
        {
            if (pView.IsMine == false)
            {
                return;
            }
            
            // 점프 (스페이스바)
            if (Input.GetKeyDown(KeyCode.Space) && _isJumping == false)
            {
                pView.RPC(nameof(JumpRPC), RpcTarget.All);
            }
        }

        // 리지드바디 다루는 것은 FixedUpdate에서 실행해야 움직이는 것에 끊김이 덜하다.
        private void FixedUpdate()
        {
            if (pView.IsMine == false)
            {
                return;
            }

            // 움직임 실행
            var horizontalMovement = Input.GetAxisRaw("Horizontal");
            var verticalMovement = Input.GetAxisRaw("Vertical");

            Vector3 moveVelocity = (transform.right * horizontalMovement + transform.forward * verticalMovement).normalized;
            moveVelocity *= moveSpeed;

            _rigidbody.MovePosition(transform.position + moveVelocity * Time.fixedDeltaTime);
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
        
        [PunRPC]
        private void JumpRPC()
        {
            if (_isJumping) return;
            _isJumping = true;
            Debug.Log("someone is jumping");
            // _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * jumpPower);
        }
        
        #endregion

        #region IPunObservable
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
        #endregion
        
        #region IDamagable

        private float _health;
        public float Health
        {
            get => _health;
            set
            {
                // 본인만 데미지 입음
                if (pView.IsMine == false) return;
                
                _health = value;
                if (_health > 0f) return;
                
                // 죽음 로직 (체력이 0 초과이면 실행되지 않음)
                _health = 0f;
                OnDeath();
            }
        }
        public void GetDamage(float amount)
        {
            if (pView.IsMine == false)
            {
                return;
            }
            
            Health -= amount;
        }

        public void OnDeath()
        {
            if (pView.IsMine == false)
            {
                return;
            }
            // 죽을 때 실행
        }
        #endregion
    }

}
