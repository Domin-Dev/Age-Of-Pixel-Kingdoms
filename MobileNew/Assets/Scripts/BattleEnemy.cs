using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemy : MonoBehaviour
{
    UnitsManager unitsManager;
    private void Start()
    {
        unitsManager = GetComponent<UnitsManager>();
    }

    private const float timer = 1f;
    private float currentTime = 0;

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

    public void SendUnits()
    {
        for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
        {
            if(unitsManager.enemyUnits.ContainsKey(i) && unitsManager.enemyUnits[i] > 0)
            {
                  
            }
        } 
    }

}
