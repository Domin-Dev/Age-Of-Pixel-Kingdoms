
using UnityEngine;


[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    [field: SerializeField] public string spellName { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    [field: SerializeField] public GameObject spell { get; private set; }
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public float timeToReload { get; private set; }
}
