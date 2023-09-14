using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public Statistic coins;
    public int income { private set; get; }

    public PlayerStats(int coins) 
    {
         this.coins = new Statistic((float income) => { return GetPopulation();},(float)coins,0f, () => { UIManager.Instance.UpdateCounters(); });
    }

    public float GetPopulation()
    {
        int population = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                population += (int)GameManager.Instance.provinces[i].population.value;
            }
        }
        Debug.Log(population);
        return population;
    }

}
