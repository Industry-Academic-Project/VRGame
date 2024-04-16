using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Server
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        private Rigidbody _rigidbody;
        public PhotonView pView;
        private Material _characterMaterial;
        public float moveSpeed = 8.0f;
        public float jumpPower = 10000.0f;
        private bool _isJumping = false;
        
        #region Unity Event Functions
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            _characterMaterial = GetComponent<MeshRenderer>().material;
            _characterMaterial.color = pView.IsMine ? Color.green : Color.red;
        }

        private void Update()
        {
            if (pView.IsMine == false)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && _isJumping == false)
            {
                pView.RPC(nameof(JumpRPC), RpcTarget.All);
            }
        }

        private void FixedUpdate()
        {
            if (pView.IsMine == false)
            {
                return;
            }

            float horizontalMovement = Input.GetAxisRaw("Horizontal");
            float verticalMovement = Input.GetAxisRaw("Vertical");

            Vector3 moveVelocity = (transform.right * horizontalMovement + transform.forward * verticalMovement).normalized;
            moveVelocity *= moveSpeed;

            _rigidbody.MovePosition(transform.position + moveVelocity * Time.fixedDeltaTime);

            
        }
        
        #endregion
        
        #region private functions

        private void OnCollisionEnter(Collision other)
        {
            if (pView.IsMine == false || other.gameObject.TryGetComponent<PlayerController>(out Server.PlayerController player))
            {
                return;
            }

            _isJumping = false;
        }

        private void OnCollisionExit(Collision other)
        {
            if (pView.IsMine == false || other.gameObject.TryGetComponent<PlayerController>(out Server.PlayerController player))
            {
                return;
            }

            _isJumping = true;
        }

        [PunRPC]
        private void JumpRPC()
        {
            if (_isJumping) return;
            _isJumping = true;
            Debug.Log("someone is jumping");
            // _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(Vector3.up * jumpPower);
        }

        private void Jump()
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
    }

}
