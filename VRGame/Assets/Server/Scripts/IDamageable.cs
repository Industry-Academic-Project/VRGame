using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    public interface IDamageable
    {
        /// <summary> 현재 체력 </summary>
        float Health { get; set; }
        
        /// <summary> 이 오브젝트가 데미지를 받게 함. (Health -= amount) </summary>
        /// <param name="amount"> 현재 체력에서 닳게 할 양 </param>
        void GetDamage(float amount);
        
        /// <summary> 죽은 후 실행하는 메서드 </summary>
        void OnDeath();
    }
}
