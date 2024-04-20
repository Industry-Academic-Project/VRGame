using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server
{
    [CreateAssetMenu(fileName = "Gun Data", menuName = "Scriptable Object/Gun Data", order = int.MaxValue)]
    public class GunData : ScriptableObject
    {
        /// <summary> 탄창에 들어가는 최대 총알 개수 </summary>
        public int maxAmmoInMag;
        
        /// <summary> 최대 여분 총알 개수 </summary>
        public int maxExtraAmmo;
        
        /// <summary> 장전하는 데 걸리는 시간 </summary>
        public float timeToReload;
        
        /// <summary> 1분에 발사하는 총알 개수, 60 / rpm을 해서 발사 간격을 구할 수 있음. </summary>
        public float rpm;
        
        /// <summary> IDamagable을 상속받은 오브젝트에게 가하는 데미지 </summary>
        public float damage;
    }
}
