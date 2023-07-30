using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] SpriteRenderer target;

    public Vector3 Limit;

    Vector3 startPosition;
    Vector3 endPosition;

    void Start()
    {
        if (target != null)
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = target.bounds.size.x / target.bounds.size.y;

            if (screenRatio >= targetRatio)
            {
                Camera.main.orthographicSize = target.bounds.size.y / 2;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                Camera.main.orthographicSize = target.bounds.size.y / 2 * differenceInSize;
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if(Input.touchCount > 0)
        {
            Debug.Log("goog");
        }

        if(Input.GetMouseButton(0))
        {;
            endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position + startPosition - endPosition;
            transform.position = new Vector3(Mathf.Clamp(position.x,0,Limit.x),Mathf.Clamp(position.y,0,Limit.y), -10);
        }
    }



}
