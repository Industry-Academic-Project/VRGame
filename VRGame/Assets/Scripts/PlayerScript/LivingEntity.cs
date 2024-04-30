using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float startingHealth = 100f; 
    public float health { get; set; }
    public bool dead { get; set; }
    
    protected virtual void OnEnable() {
        dead = false;
        health = startingHealth;
    }

    
    public void OnDamage(Vector3 hitPoint, Vector3 hitNormal)
    {
        
    }

}
