using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform shootingPosition; // 발사 위치
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) // 오른쪽 버튼
        {
            //총알 발사 입력
            Instantiate(bulletPrefab, shootingPosition.position, shootingPosition.rotation);
        }
    }
}
