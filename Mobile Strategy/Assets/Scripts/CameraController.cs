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
        Application.targetFrameRate = 60;
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
        lastValue = 0f;
    }


    float lastValue;
    private void Update()
    {
        if (target == null)
        {
            if (Input.GetMouseButtonDown(0) && !MouseIsOverUI())
            {
                startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (selectingProvinces != null) selectingProvinces.SelectingProvince();
            }

            if (Input.GetMouseButton(0) && !MouseIsOverUI() && Input.touchCount <= 1)
            {
                endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 position = transform.position + startPosition - endPosition;
                transform.position = new Vector3(Mathf.Clamp(position.x, -0.5f, Limit.x), Mathf.Clamp(position.y, -0.5f, Limit.y), -10);
            }

            if (Input.touchCount >= 2)
            {
                float Value = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                if (lastValue == 0)
                {
                    lastValue = Value;
                }
                else
                {
                    Debug.Log(Value + "  " + lastValue);
                    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + (lastValue - Value) * 0.004f, 0.5f, 3);
                    lastValue = Value;
                }
            }
            else if (lastValue != 0)
            {
                lastValue = 0;
            }
        }
    }

    private bool MouseIsOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;   
    }

}
