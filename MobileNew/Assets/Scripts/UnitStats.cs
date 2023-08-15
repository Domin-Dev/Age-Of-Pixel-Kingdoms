using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Stats", menuName = "Units")]
public class UnitStats : ScriptableObject
{
    [SerializeField] public GameObject unit;


    [Space(20f,order = 0)]
    [SerializeField] public Sprite sprite;

    [Space(20f, order = 0)]
    [SerializeField] public float speed;
    [SerializeField] public float lifePoints;
    [SerializeField] public float damage;
    [SerializeField] public float range;
    [SerializeField] public float rateOfFire;
    [SerializeField] public float turnCost;

    [Space(20f, order = 0)]
    [SerializeField] public float price;
}
