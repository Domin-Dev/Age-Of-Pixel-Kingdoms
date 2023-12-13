using System;
using Unity.Mathematics;
using UnityEngine;

public class Unit : MonoBehaviour
{

    UnitStats.UnitType unitType;
    public int unitIndex;


    public float speedValue;
    public float speed;
    public float maxLifePoints { get; private set; }
    public float damage { get; private set; }
    public float range { get; private set; }
    public float rateOfFire { get; private set; }
    public Sprite bullet { get; private set; }
    //////////////////////////
    private float lifePoints;
    private Transform lifeBar;
    private SpriteRenderer spriteRenderer;
    //////////////////////////
    private float targetPositionX;
    public int multiplier { get; private set; }
    public bool unitIsFriendly { get; private set; }
    public int pathIndex { get; private set; }
    private Action<bool> clearList;
    private Func<Unit> checkPath;
    private Func<float> checkPosition;
    private Action changeSpeed;

    private Animator animator;
    //////////////////////////
    //Attack//
    float attackTimer;
    float timeToAttack;
    bool isReadyToAttack;
    //Hit effect//
    const float timerHitEffect = 0.2f;
    float timeToChange;
    bool IsActive;
    bool lerpIsActive;

    Unit target;


    AudioSource audioSource;
    bool movementIsPlaying;
    int movementSound;
    int attackSound;


    private void Start()
    {
        IsActive = false;
        lerpIsActive = false;
        movementIsPlaying = false;

        isReadyToAttack = true;
        attackTimer = 60 / rateOfFire;

        audioSource = this.GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        lifePoints = maxLifePoints;
        lifeBar = transform.GetChild(0).GetChild(0).transform;
        transform.GetChild(0).gameObject.SetActive(false);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetUp(int index, int pathIndex, bool unitIsFriendly, float targetPositionX, Action<bool> clearList, Func<Unit> checkPath, Func<float> checkPosition)
    {
        UnitStats unitStats = GameAssets.Instance.unitStats[index];
        this.unitType = unitStats.unitType;
        this.unitIndex = index;
        this.speedValue = unitStats.speed;
        this.speed = unitStats.speed;
        this.maxLifePoints = unitStats.lifePoints;
        this.damage = unitStats.damage;
        this.range = unitStats.range;
        this.rateOfFire = unitStats.rateOfFire;
        this.bullet = unitStats.bullet;
        this.movementSound = unitStats.movementSound;
        this.attackSound = unitStats.attackSound;


        this.targetPositionX = targetPositionX;
        this.clearList = clearList;
        this.checkPath = checkPath;
        this.checkPosition = checkPosition;
        this.unitIsFriendly = unitIsFriendly;
        this.pathIndex = pathIndex;

        if (!unitIsFriendly)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
            multiplier = -1;
        }
        else
        {
            multiplier = 1;
        }
    }

    public float positionX;
    private void Update()
    {
        if (lerpIsActive)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.4f, 1.1f, 1), Time.deltaTime * 70);
            if (transform.localScale.x <= 0.75f)
            {
                IsActive = true;
                lerpIsActive = false;
            }
        }

        if (IsActive)
        {
            timeToChange = timeToChange + Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1, 1), Time.deltaTime * 10);
            if (timeToChange >= timerHitEffect)
            {
                IsActive = false;
                timeToChange = 0;
                //    spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (!isReadyToAttack)
        {
            timeToAttack = timeToAttack + Time.deltaTime;
            if (timeToAttack >= attackTimer)
            {
                isReadyToAttack = true;
                timeToAttack = 0;
            }
        }

        positionX = transform.position.x + 0.1f * speed * multiplier * Time.deltaTime;
        Unit unit = checkPath();
        //  float pos = checkPosition() - multiplier;
        if (unit == null)
        {
            animator.SetBool("Idle", false);
            if (Math.Abs(targetPositionX - transform.position.x) > 0.5f)
            {
                if (!movementIsPlaying)
                {
                    movementIsPlaying = true;
                    audioSource.loop = true;
                    audioSource.PlayOneShot(Sounds.instance.GetClip(movementSound));
                }
                //  if (positionX * multiplier > pos * multiplier) positionX = pos - multiplier;           
                transform.position = new Vector3(positionX, transform.position.y, 0f);
                //
            }
            else
            {
                if (changeSpeed != null) changeSpeed();
                Destroy(gameObject);
                clearList(false);
            }
        }
        else
        {
            animator.SetBool("Idle", true);
            if (movementIsPlaying)
            {
                if (unit.unitIsFriendly == unitIsFriendly && unit.speed < speed)
                {
                    speed = unit.speed;
                    unit.changeSpeed = () =>
                    {
                        speed = speedValue;
                        if (changeSpeed != null) changeSpeed();
                    };
                }
                movementIsPlaying = false;
                audioSource.loop = false;
                audioSource.Stop();
            }
            if (unit.unitIsFriendly != unitIsFriendly)
            {
                target = unit;
                Attack();
            }
        }
    }
    private void Attack()
    {
        if (isReadyToAttack)
        {
            isReadyToAttack = false;
            animator.SetTrigger("Attack");
        }
    }
    public void EndOfAnimation()
    {
        if (target != null && target.gameObject != null)
        {
            AudioClip clip = Sounds.instance.GetClip(attackSound);
            if (isActiveAndEnabled) audioSource.PlayOneShot(clip);
            target.Hit(damage);
        }
        target = null;
    }
    public void Shot()
    {
        if (target != null)
        {
            Bullet.NewBullet(transform.GetChild(2).position, target.transform, bullet, () => { EndOfAnimation(); }, transform);
        }
    }
    public void Hit(float damage)
    {
        if (gameObject != null)
        {
            lerpIsActive = true;
            if (transform.GetChild(0).gameObject != null) transform.GetChild(0).gameObject.SetActive(true);
            lifePoints = math.clamp(lifePoints - damage, 0, maxLifePoints);
            if (lifePoints <= 0)
            {
                if (changeSpeed != null) changeSpeed();
                Destroy(gameObject);
                clearList(true);
            }
            else
            {
                lifeBar.localScale = new Vector3(lifePoints / maxLifePoints, 1, 1);
            }
        }
    }

    public void Heal(float value)
    {
        lifePoints = math.clamp(lifePoints + value, 0, maxLifePoints);
        lifeBar.localScale = new Vector3(lifePoints / maxLifePoints, 1, 1);
    }
    public void SpeedBoost(float value, Sprite sprite)
    {
        if (speedValue <= GameAssets.Instance.unitStats[unitIndex].speed)
        {
            speedValue = value * speedValue;
            speed = value * speed;
            AddIcon(sprite);
        }
    }

    public void DamageBoost(float value, Sprite sprite)
    {
        if (damage <= GameAssets.Instance.unitStats[unitIndex].damage)
        {
            damage = value * damage;
            AddIcon(sprite);
        }
    }
    private void AddIcon(Sprite sprite)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        Transform transform1 = transform.GetChild(1);
        SpriteRenderer spriteRen = new GameObject("effect", typeof(RectTransform)).AddComponent<SpriteRenderer>();
        spriteRen.sprite = sprite;
        spriteRen.transform.parent = transform1;
        spriteRen.transform.localScale = Vector3.one;
        spriteRen.sortingOrder = 30;
    }
    private void TargetIsNull()
    {
        target = null;
    }
}
