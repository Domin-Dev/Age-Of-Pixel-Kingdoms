using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Stats", menuName = "Units")]
public class UnitStats : ScriptableObject
{
    [SerializeField] public float speed;
    [SerializeField] public float lifePoints;
    [SerializeField] public float damage;
    [SerializeField] public float range;
    [SerializeField] public float rateOfFire;
    [SerializeField] public float turnCost;

    [SerializeField] public float price;
}
