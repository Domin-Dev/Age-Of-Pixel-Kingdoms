using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class PlayerStats
{
    public Statistic coins;
    public Statistic warriors;
    public Statistic developmentPoints;

    public Statistic movementPoints;

    public bool[] buildingsPermit;
    public bool[,] research;

    public int income { private set; get; }

    public int texesIndex;
    public int researchIndex;
    public PlayerStats(int coins)
    {
        this.texesIndex = 2;
        this.coins = new Statistic((float)coins, 0f, () => { UIManager.Instance.UpdateCounters(); }, "Coin");
        this.coins.AddBonus(-100, new Bonus("Base income", 100f, Bonus.bonusType.Income));
        this.coins.AddBonus(-200, new Bonus("Population", (float multiplier) => { return GetPopulation() * multiplier; }, (float multiplier) => { return GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 0.2f));
        this.coins.SetDescription("Coins are used for \nunit recruitment and construction.");

        this.warriors = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); }, 0, "Warrior");
        this.warriors.AddBonus(-100, new Bonus("Base Value", 20, Bonus.bonusType.IncreaseLimit));
        this.warriors.AddBonus(-200, new Bonus("Provinces", (float multiplier) => { return GetWarriors(); }, (float multiplier) => { return ""; }));

        this.developmentPoints = new Statistic(0f, 0f, () => { UIManager.Instance.UpdateCounters(); }, "DevelopmentPoint");
        this.developmentPoints.AddBonus(-100, new Bonus("Base income", 10f, Bonus.bonusType.Income));
        this.developmentPoints.AddBonus(-200, new Bonus("Population", (float multiplier) => { return GetPopulation() * multiplier; }, (float multiplier) => { return GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 0.09f));

        this.movementPoints = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); }, 0, "MovementPoint");
        this.movementPoints.AddBonus(-100,new Bonus("Base Value",30,Bonus.bonusType.IncreaseLimit));
        this.movementPoints.AddBonus(-200, new Bonus("Provinces", (float multiplier) => { return GetMovementPoints(); }, (float multiplier) => { return ""; })) ;
        this.buildingsPermit = new bool[GameAssets.Instance.buildingsStats.Length];
        this.research = new bool[4,GameAssets.Instance.research.GetLength(0)];
    }   
    public float GetNumberOfProvinces()
    {
        int number = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                number++;
            }
        }
        return number;
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
    public float GetWarriors()
    {
        int warriors = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                warriors += (int)GameManager.Instance.provinces[i].warriors.value;
            }
        }
        return warriors;
    }
    public float GetMovementPoints()
    {
        int movementPoints = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                movementPoints += (int)GameManager.Instance.provinces[i].movementPoints.value;
            }
        }
        return movementPoints;
    }
    public void ChangeCoinsMultiplier(float coinsMultiplier)
    {
        coins.bonuses[-200].multiplier = coinsMultiplier;
    }
    public void ChangeDevelopmentMultiplier(float developmentMultiplier)
    {
        developmentPoints.bonuses[-200].multiplier = developmentMultiplier;
    }
    public void ChangePopulationMultiplier(float populationMultiplier)
    {
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                GameManager.Instance.provinces[i].population.bonuses[-100].multiplier = populationMultiplier;
            }
        }
    }
    public bool CanBuild(int index)
    {
        Debug.Log(buildingsPermit[index]);
        return buildingsPermit[index];
    }
}
