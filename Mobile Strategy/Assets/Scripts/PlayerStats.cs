using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public Statistic coins;
    public Statistic warriors;
    public Statistic developmentPoints;

    public Statistic movementPoints;


    public int income { private set; get; }


    public PlayerStats(int coins) 
    {
        this.coins = new Statistic((float)coins,0f, () => { UIManager.Instance.UpdateCounters(); },"Coin");
        this.coins.AddBonus(-200, new Bonus("Population", () => { return GetPopulation() * 0.1f; }));
            
        this.coins.AddBonus(-100, new Bonus("Base income",100f, Bonus.bonusType.Income));
        this.warriors = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); },50,"Warrior");
        this.developmentPoints = new Statistic(0f, 0f, () => { UIManager.Instance.UpdateCounters(); },"DevelopmentPoint");
        this.movementPoints = new Statistic(30, () => { UIManager.Instance.UpdateCounters(); }, 30, "MovementPoint");
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

    public void UpdateWarriors()
    {
        int warriors = 50;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                warriors += (int)GameManager.Instance.provinces[i].warriors.value;
            }
        }
        this.warriors.limit = warriors;
    }
}
