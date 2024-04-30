using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    private void Update()
    {
        if (dead)
        {
            Debug.Log("Dead");
            Destroy(gameObject);
        }
    }
}
