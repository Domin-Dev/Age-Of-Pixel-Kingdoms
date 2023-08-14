using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{

    [SerializeField] SpriteRenderer target;

    public Vector3 Limit;

    Vector3 startPosition;
    Vector3 endPosition;

    SelectingProvinces selectingProvinces;


    void Start()
    {
        selectingProvinces = GetComponent<SelectingProvinces>();

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
        if(Input.GetMouseButtonDown(0) && !MouseIsOverUI())
        {
            startPosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
           if(selectingProvinces != null) selectingProvinces.SelectingProvince();
        }
        
        if(Input.touchCount > 0)
        {
            Debug.Log("goog");
        }

        if(Input.GetMouseButton(0) && !MouseIsOverUI())
        { 
            endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position + startPosition - endPosition;
            transform.position = new Vector3(Mathf.Clamp(position.x,0,Limit.x),Mathf.Clamp(position.y,0,Limit.y), -10);
        }
    }

    private bool MouseIsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}
