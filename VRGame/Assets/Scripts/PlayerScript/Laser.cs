using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private RaycastHit Collided_object;
    private GameObject currentObject; 
    
    public float raycastDistance = 100f; 

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);
 
        if (Physics.Raycast(transform.position, transform.forward, out Collided_object, raycastDistance))
        {
            lineRenderer.SetPosition(1, Collided_object.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (transform.forward * raycastDistance));
        }
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.tag == "Wall")
    //     {
    //         lineRenderer.SetPosition(0, transform.position);
    //         lineRenderer.SetPosition(1, other.transform.position);
    //     }
    //
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.tag == "Wall")
    //     {
    //         lineRenderer.SetPosition(1, transform.position);
    //     }
    // }
}
