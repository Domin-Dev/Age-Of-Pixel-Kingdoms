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

    private const float timer = 1f;
    private float currentTime = 0;

    private Dictionary<int, int> units;
    public int[] typesArray = new int[3];

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
                    SendUnit(Random.Range(0, 5), Random.Range(1, 4));
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
                int value = units[i];
                int index = (int)GameAssets.Instance.unitStats[i].unitType;
                typesArray[index] += value;
            }
        }
    }
    private void SendUnitByTyp(int[] typeArray,int pathIndex)
    {
        for (int index = 0; index < typeArray.Length; index++)
        {
            if (typesArray[typeArray[index]] > 0)
            {
                for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
                {
                    if (unitsManager.enemyUnits.ContainsKey(i) && unitsManager.enemyUnits[i] > 0 && (int)GameAssets.Instance.unitStats[i].unitType == typeArray[index])
                    {
                        SendUnit(i, pathIndex);
                        return;
                    }
                }
            }
        }
    }
    public void SendDefenders(int pathIndex)
    {
        int[] array1 = { 0, 1, 2 };
        SendUnitByTyp(array1,pathIndex);
        if(CountUnits(pathIndex) > 2)
        {
            int[] array2 = { 0, 1, 2 };
            SendUnitByTyp(array2, pathIndex);
        }
    } 
    private int CountUnits(int index)
    {
        List<Unit> units = unitsManager.GetPath(index);
        int number = 0;
        foreach (Unit unit in units)
        {
            if(!unit.unitIsFriendly)
            {
                number++;
            }
        }
        return number;
    }



    public void CheckPaths()
    {
        for (int i = 1; i < 5; i++)
        {
            List<Unit> units = unitsManager.GetPath(i);
            int enemy = 0;
            int your = 0;
            foreach (Unit unit in units)
            {
                if (!unit.unitIsFriendly)
                {
                    enemy++;
                }
                else
                {
                    your++;
                }
            }
            if(your > enemy && unitsManager.enemyUnitCount > 6)
            {
                SendDefenders(i);
            }else if(your > enemy && (enemy == 0 || your > enemy + 2))
            {
                SendDefenders(i);
            }

        }
    }
    public void SendUnit(int unitIndex,int pathIndex)
    {
        if (units.ContainsKey(unitIndex))
        {
            if(unitsManager.EnemyCreateUnit(unitIndex, pathIndex))
            {
                typesArray[(int)GameAssets.Instance.unitStats[unitIndex].unitType]--;
            }
        }
    }

}
