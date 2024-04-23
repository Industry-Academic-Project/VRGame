using UnityEngine;

namespace Server
{
    public interface IGun : IWeapon
    {
        /// <summary> 장전 중복 방지를 위한 프로퍼티 </summary> 
        bool IsReloading { get; set; }
        
        /// <summary> 장전 실행 </summary>
        void TryReload();
        
        /// <summary> 발사 딜레이를 위한 프로퍼티 </summary>
        bool IsFiring { get; set; }
        
        
        /// <summary> 장전하기까지 걸리는 시간 </summary>
        float TimeToReload { get; }
        
        /// <summary> 현재 탄창에 있는 총알 개수 </summary>
        int CurrentAmmoInMag { get; set; }
        
        /// <summary> 탄창에 들어갈 수 있는 최대 총알 개수 </summary>
        int MaxAmmoInMag { get; }
        
        /// <summary> 현재 (장전할 때 쓰이는)여분 총알 개수 </summary>
        int CurrentExtraAmmo { get; set; }
        
        /// <summary> 최대 여분 총알 개수 </summary>
        int MaxExtraAmmo { get; }
        
        /// <summary> 총알 프리팹 </summary>
        GameObject Bullet { get; }
    }
}