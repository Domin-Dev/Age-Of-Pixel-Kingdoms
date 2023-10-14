using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "New BuildingStats", menuName = "Building Stats")]

public class BuildingStats : ScriptableObject
{
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public int price { get; private set; }
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public int turnCost { get; private set; }
    [field: SerializeField] public bool canBuild { get; private set; }
    [field: SerializeField] public int bonusIndex { get; private set; }
    [field: SerializeField] public int movementPointsPrice { get; private set; }
}



