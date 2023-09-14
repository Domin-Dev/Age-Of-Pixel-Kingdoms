using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public Statistic coins;
    public Statistic warriors;

    public int income { private set; get; }

    public PlayerStats(int coins) 
    {
        this.coins = new Statistic((float income) => { return 100 + GetPopulation() * 0.1f;},(float)coins,0f, () => { UIManager.Instance.UpdateCounters(); });
        this.warriors = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); },50);
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
        return population;
    }

}
