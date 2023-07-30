using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float maxLifePoints;
    [SerializeField] public float Damage;
    [SerializeField] public float Range;



    private float lifePoints;


    private Vector2 TargetPosition;
    private int multiplier;
    Action clearList;



    private void Start()
    {
        lifePoints = maxLifePoints;
    }

    public void SetUp(bool unitIsFriendly, Vector2 targetPosition, Action action)
    {
        this.TargetPosition = targetPosition;
        this.clearList = action;

        if(!unitIsFriendly)
        {
            multiplier = -1;
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            multiplier = 1;
        }
    }
    private void FixedUpdate()
    {
        Debug.Log(Math.Abs(TargetPosition.x - transform.position.x));
        if (Math.Abs(TargetPosition.x - transform.position.x) > 0.2f) transform.position = transform.position + new Vector3(0.1f, 0, 0) * speed * multiplier;
        else
        {
            Destroy(gameObject);
            clearList();
        }
    }
}
