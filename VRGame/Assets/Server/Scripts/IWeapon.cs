using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public interface IWeapon
    {
        /// <summary> 발사 시도를 위한 메서드 </summary>
        void TryFire();
        
        /// <summary> 발사 간격 </summary>
        float FireDelay { get; }
        
        /// <summary> 무기가 주는 데미지 </summary>
        float Damage { get; }
    }
}
