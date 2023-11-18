
using UnityEngine;

[CreateAssetMenu(fileName = "New Research", menuName = "Research")]
public class Research : ScriptableObject
{
    [field: SerializeField] public string name;
    [field: SerializeField] public string description;
    [field: SerializeField] public Sprite image;
    [field: SerializeField] public int researchID;

    [field: SerializeField] public int price;
}
