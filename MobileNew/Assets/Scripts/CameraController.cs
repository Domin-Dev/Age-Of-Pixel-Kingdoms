using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour

{
   [HideInInspector] public Vector3 Limit;

    Vector3 startPosition;
    Vector3 endPosition;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if(Input.GetMouseButton(0))
        {
            Debug.Log("jest ");
            endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position + startPosition - endPosition;
            transform.position = new Vector3(Mathf.Clamp(position.x,0,Limit.x),Mathf.Clamp(position.y,0,Limit.y), -10);
        }
    }


}
