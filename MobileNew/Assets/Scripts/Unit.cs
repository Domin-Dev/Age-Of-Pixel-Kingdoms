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

    public float speed { get; private set; }
    public float maxLifePoints { get; private set; }
    public float damage { get; private set; }
    public float range { get; private set; }
    public float rateOfFire { get; private set; }   


    //////////////////////////

    private float lifePoints;
    private Transform lifeBar;
    private SpriteRenderer spriteRenderer;

    //////////////////////////

    private float targetPositionX;
    public int multiplier { get; private set; }
    public bool unitIsFriendly { get; private set; }
    public int pathIndex { get; private set; }
    private Action clearList;
    private Func<Unit, Unit> checkPath;
    private Animator animator;

    //////////////////////////

    //Attack//
    float attackTimer;
    float timeToAttack;
    bool isReadyToAttack;



    //Hit effect//
    const float timerHitEffect = 0.1f;
    float timeToChange;
    bool IsActive;
    bool lerpIsActive;



    private void Start()
    {
        IsActive = false;
        lerpIsActive = false;

        isReadyToAttack = true;
        attackTimer = 60 / rateOfFire;


        animator = GetComponent<Animator>();
        lifePoints = maxLifePoints;
        lifeBar = transform.GetChild(0).GetChild(0).transform;
        transform.GetChild(0).gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUp(UnitStats unitStats,int pathIndex,bool unitIsFriendly, float targetPositionX, Action clearList, Func<Unit, Unit> checkPath)
    {
        this.speed = unitStats.speed;
        this.maxLifePoints = unitStats.lifePoints;
        this.damage = unitStats.damage;
        this.range = unitStats.range;
        this.rateOfFire = unitStats.rateOfFire;
        
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
        if(lerpIsActive)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.4f, 1.1f, 1), Time.deltaTime * 70);
            if (transform.localScale.x <= 0.75f)
            {
                IsActive = true;
                lerpIsActive = false;
            }
        }

        if(IsActive)
        {
            timeToChange = timeToChange + Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1, 1), Time.deltaTime * 7);
            if (timeToChange >= timerHitEffect)
            {
                
                IsActive =false;
                timeToChange = 0;
                spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if(!isReadyToAttack)
        {
            timeToAttack = timeToAttack + Time.deltaTime;
            if(timeToAttack >= attackTimer)
            {
                isReadyToAttack = true;
                timeToAttack = 0;
            }
        }



        Unit unit = checkPath(this);
        if (unit == null)
        {
            animator.SetBool("Idle", false);
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
            animator.SetBool("Idle", true);
            if (unit.unitIsFriendly != unitIsFriendly)
            {
                Attack(unit);
            }        
        }
    }
    private void Attack(Unit unit)
    {
        if (isReadyToAttack)
        {
            isReadyToAttack = false;
            animator.SetTrigger("Attack");
            unit.Hit(damage);
        }
    }
    private void Hit(float damage)
    {
        lerpIsActive = true;
        //spriteRenderer.material = new Material(Shader.Find("Shader Graphs/Unit"));      
        transform.GetChild(0).gameObject.SetActive(true);
        lifePoints = math.clamp(lifePoints - damage, 0, maxLifePoints);
        lifeBar.localScale = new Vector3(lifePoints/maxLifePoints, 1, 1);
        if(lifePoints <= 0)
        {
            Destroy(gameObject);
            clearList();
        }        
    }
}
