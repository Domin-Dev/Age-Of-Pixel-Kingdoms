using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Stats", menuName = "Unit Stats")]
public class UnitStats : ScriptableObject
{
    [field: SerializeField] public GameObject unit;


    [field: Space(20f,order = 0)]
    [field:SerializeField] public Sprite sprite { get; private set; }
    [field:SerializeField] public string unitName { get; private set; }



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
    [field: SerializeField] public int movementPointsPrice { get; private set; }

    [field: SerializeField] public int movementSound { get; private set; }
    [field: SerializeField] public int attackSound { get; private set; }


    [field: SerializeField] public UnitType unitType { get; private set; }

    [field: SerializeField] public float battleValue { get; private set; }// E (0,1]

    public enum UnitType
    {
        NormalUnit,
        RangeUnit,
    }
}
