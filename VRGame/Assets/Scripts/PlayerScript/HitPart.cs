using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPart : MonoBehaviour
{
    public int hitDamage = 20;
    
    public void OnDamage() {
        // 데미지만큼 체력 감소
        Enemy EnemyScript = GetComponentInParent<Enemy>();
        EnemyScript.health -= hitDamage;
        if (EnemyScript.health <= 0)
        {
            EnemyScript.dead = true;
        }
    }

}
