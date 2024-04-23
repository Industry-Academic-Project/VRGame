using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Server
{
    public class Gun : MonoBehaviour, IGun
    {
        public PhotonView pView;
        // Start is called before the first frame update
        private void Start()
        {
            if (pView.IsMine == false)
            {
                // Destroy(this);
                return;
            }
            InitGun();
        }

        // Update is called once per frame
        private void Update()
        {
            if (pView.IsMine == false) return;

            // 발사 (LMB)
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                TryFire();
            }

            // 재장전 (R)
            if (Input.GetKeyDown(KeyCode.R))
            {
                TryReload();
            }
        }
        
        #region Gun

        #region Gun Properties
        
        // ScriptableObject
        [SerializeField] private GunData gunData;
        [SerializeField] private Transform firePos;
        public int CurrentAmmoInMag { get; set; }
        public int MaxAmmoInMag => gunData.maxAmmoInMag;

        public int CurrentExtraAmmo { get; set; }
        public int MaxExtraAmmo => gunData.maxExtraAmmo;
        public float TimeToReload => gunData.timeToReload;
        public bool IsReloading { get; set; }

        public bool IsFiring { get; set; }
        public float FireDelay => 60 / gunData.rpm;      
        public float Damage => gunData.damage;

        [SerializeField]
        private GameObject bullet;
        public GameObject Bullet { get => bullet; private set => bullet = value; }
        #endregion
        
        #region IGun Implements
        public void TryFire()
        {
            // 자기 총인가?
            if (pView.IsMine == false) return;
            
            // 총알이 많은가?
            if (CurrentAmmoInMag <= 0) return;
            if (IsFiring) return;
            
            StartCoroutine(FireCoroutine());
        }

        private void Fire()
        {
            if (pView.IsMine == false) return;
            var bulletInstance = PhotonNetwork.Instantiate("Bullet", firePos.position, firePos.rotation);
            bulletInstance.GetComponent<Bullet>().Damage = Damage;
            bulletInstance.GetComponent<Rigidbody>().AddForce(bulletInstance.transform.forward * 2000f);
            CurrentAmmoInMag -= 1;
        }
        
        public void TryReload()
        {
            if (pView.IsMine == false) return;
            if (IsReloading) return;

            if (CurrentAmmoInMag == MaxExtraAmmo || MaxExtraAmmo == 0) return;
            
            StartCoroutine(ReloadCoroutine());
        }
        
        #endregion
       
        private void InitGun()
        {
            if (pView.IsMine == false) return;
            
            if (gunData == null)
            {
                Debug.LogWarning($"{nameof(gunData)} is not set");
                return;
            }

            IsReloading = false;

            CurrentAmmoInMag = MaxAmmoInMag;
            CurrentExtraAmmo = MaxExtraAmmo;
        }

        private IEnumerator FireCoroutine()
        {
            if (pView.IsMine == false) yield break;
            IsFiring = true;

            Fire();
            yield return new WaitForSeconds(FireDelay);
            
            IsFiring = false;
        }
        
        private IEnumerator ReloadCoroutine()
        {
            if (pView.IsMine == false) yield break;
            
            IsReloading = true;
            yield return new WaitForSeconds(TimeToReload);

            // 장전 대기 시간이 끝나고 채울 총알의 갯수
            var ammoToFill = Mathf.Min(CurrentExtraAmmo, MaxAmmoInMag - CurrentAmmoInMag);
            
            CurrentAmmoInMag += ammoToFill;
            CurrentExtraAmmo -= ammoToFill;

            IsReloading = false;
        }
        
        #endregion
    }
}

