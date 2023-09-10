using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : MonoBehaviour
{
    UnitsManager unitsManager;
    private void Start()
    {
        unitsManager = GetComponent<UnitsManager>();
        units = unitsManager.enemyUnits;
        CheckUnits();
    }

    private const float timer = 2f;
    private float currentTime = 0;

    private Dictionary<int, int> units;
    private int[] typesArray = new int[3];

    //  NormalUnit = 0
    //  FastUnit = 1
    //  RangeUnit = 2

    Queue<int> pathIndex = new Queue<int>();
    Queue<int> unitIndex = new Queue<int>();


    private void Update()
    {
        if (unitsManager.enemyUnitCount > 8)
        {
            if(currentTime < timer)
            {
                currentTime += Time.deltaTime;
                if(currentTime >= timer) 
                {
                    currentTime = 0;

                }
            }
        }
    }

    private void CheckUnits()
    {
        for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
        {
            if(units.ContainsKey(i) && units[i] > 0)
            {
                typesArray[(int)GameAssets.Instance.unitStats[i].unitType] += units[i];
            }
        }
    }
    private int GetUnitByTyp(int typeIndex)
    {
        if(typesArray[typeIndex] > 0)
        {
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
                if (unitsManager.enemyUnits.ContainsKey(i) && unitsManager.enemyUnits[i] > 0 && (int)GameAssets.Instance.unitStats[i].unitType == typeIndex)
                {
                    return i;
                }
            }
        }

        if (typeIndex == 0 && typesArray[1] > 0) return GetUnitByTyp(1);
        else if (typeIndex == 1 && typesArray[0] > 0) return GetUnitByTyp(0);
        else if (typeIndex == 2) return GetUnitByTyp(0);
        




        return -1;
    }



    public void SendDefenders(int pathIndex)
    {
        GetUnitByTyp(0);
    }

    public void SendUnit(int unitIndex,int pathIndex)
    {
        unitsManager.EnemyCreateUnit(unitIndex, pathIndex);
    }

}
