using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Stats", menuName = "Unit Stats")]
public class UnitStats : ScriptableObject
{
    [field: SerializeField] public GameObject unit;


    [field: Space(20f,order = 0)]
    [field:SerializeField] public Sprite sprite { get; private set; }


    [field: Space(20f, order = 0)]
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float lifePoints { get; private set; }
    [field: SerializeField] public float damage { get; private set; }
    [field: SerializeField] public float range { get; private set; }
    [field: SerializeField] public Sprite bullet { get; private set; }
    [field: SerializeField] public float rateOfFire { get; private set; }
    [field: SerializeField] public int turnCost { get; private set; }

    [field: Space(20f, order = 0)]
    [field: SerializeField] public int price { get; private set; }
    [field: SerializeField] public UnitType unitType { get; private set; }

    public enum UnitType
    {
        NormalUnit,
        FastUnit,
        RangeUnit,
    }
}
