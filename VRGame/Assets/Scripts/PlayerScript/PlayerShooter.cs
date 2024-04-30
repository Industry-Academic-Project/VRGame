using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform rFirePosition;

    public Transform lFirePosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Instantiate(bulletPrefab, rFirePosition.position, rFirePosition.rotation);
        }
        
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Instantiate(bulletPrefab, lFirePosition.position, lFirePosition.rotation);
        }
    }
}
