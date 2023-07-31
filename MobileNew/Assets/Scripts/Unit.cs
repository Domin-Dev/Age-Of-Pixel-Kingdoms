using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{
    //////////////////////////

    [SerializeField] public float speed;
    [SerializeField] public float maxLifePoints;
    [SerializeField] public float damage;
    [SerializeField] public float range;

    //////////////////////////

    private float lifePoints;

    //////////////////////////

    private float targetPositionX;
    public int multiplier { get; private set; }
    public bool unitIsFriendly { get; private set; }
    public int pathIndex { get; private set; }
    private Action clearList;
    private Func<Unit, Unit> checkPath;
    private Animator animator;

    //////////////////////////


    private void Start()
    {
        animator = GetComponent<Animator>();
        lifePoints = maxLifePoints;
    }

    public void SetUp(int pathIndex,bool unitIsFriendly, float targetPositionX, Action clearList, Func<Unit, Unit> checkPath)
    {
        this.targetPositionX = targetPositionX;
        this.clearList = clearList;
        this.checkPath = checkPath;
        this.unitIsFriendly = unitIsFriendly;
        this.pathIndex = pathIndex; 

        if(!unitIsFriendly)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            multiplier = -1;
        }else
        {
            multiplier = 1;
        }
        
    }
    private void Update()
    {
        Unit unit = checkPath(this);
        if (unit == null)
        {

            if (Math.Abs(targetPositionX - transform.position.x) > 0.1f)
            {
                transform.position = transform.position + new Vector3(0.1f, 0, 0) * speed * multiplier * Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
                clearList();
            }
        }
        else
        {
            Attack(unit);


        }
    }

    private void Attack(Unit unit)
    { 
        animator.SetTrigger("Attack");
        unit.Hit(damage);

    }

    private void Hit(float damage)
    {
        lifePoints = math.clamp(lifePoints - damage, 0, maxLifePoints);
        if(lifePoints <= 0)
        {
            Destroy(gameObject);
            clearList();
        }        
    }
}
