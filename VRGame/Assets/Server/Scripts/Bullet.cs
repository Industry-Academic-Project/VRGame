using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Server
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private PhotonView pView;

        private float _damage;

        public float Damage
        {
            get => _damage;
            set
            {
                _damage = value;
                pView.RPC(nameof(SetDamageRPC), RpcTarget.AllBuffered, value);
            }
        }

        [PunRPC]
        private void SetDamageRPC(float amount)
        {
            _damage = amount;
        }
        // Start is called before the first frame update
        private void Start()
        {
            var bulletMat = GetComponent<MeshRenderer>().material;
            if (pView.IsMine == false)
            {
                bulletMat.color = Color.red;
                return;
            }

            bulletMat.color = Color.green;

            pView.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered, 3f);
        }

        private void OnTriggerEnter(Collider col)
        {
            // 느린 쪽 (남이 쏜 것을 본 플레이어)에 맞춰서 hit 판정
            // 총알이 남의 것이고 Player 태그를 가진 내 플레이어가 맞았다면
            if (pView.IsMine == false && col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("Hit!");

                PlayerController otherPlayer = col.GetComponent<PlayerController>();
                otherPlayer.pView.RPC(nameof(otherPlayer.GetDamage), RpcTarget.All, Damage);
                pView.RPC(nameof(DestroyRPC), RpcTarget.AllBuffered, 0f);
            }
        }
        
        [PunRPC]
        private void DestroyRPC(float second = 0f) => Destroy(gameObject, second);
    }

}