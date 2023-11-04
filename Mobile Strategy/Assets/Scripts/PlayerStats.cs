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
    public bool[] units;

    public int texesIndex;
    public int researchIndex;
    public int index;

    public PlayerStats()
    {

    }
    public PlayerStats(int coins, int index)
    {
        this.index = index;
        this.texesIndex = 2;
        this.coins = new Statistic((float)coins, 0f, () => { UIManager.Instance.UpdateCounters(); }, "Coin");
        this.coins.AddBonus(-100, new Bonus("Base income", 100f, Bonus.bonusType.Income));
        this.coins.AddBonus(-200, new Bonus("Taxes", (float multiplier) => { return GetPopulation() * multiplier; }, (float multiplier) => { return GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 0.2f));
        this.coins.AddBonus(-300,new Bonus("Research funding", (float multiplier) => { return GetPopulation() * multiplier; }, (float multiplier) => { return GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, -0.02f));
        this.coins.AddBonus(-400, new Bonus("Units Cost", (float multiplier) => { return -GetTurnWarriosCost(); }, (float multiplier) => { return ""; }, 0f));
        this.coins.SetDescription("Coins are used for \nunit recruitment and construction.");

        this.warriors = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); }, 0, "Warrior");
        this.warriors.AddBonus(-100, new Bonus("Base Value", 20, Bonus.bonusType.IncreaseLimit));
        this.warriors.AddBonus(-200, new Bonus("Provinces", (float multiplier) => { return GetWarriors(); }, (float multiplier) => { return ""; }));


        this.developmentPoints = new Statistic(0f, 0f, () => { UIManager.Instance.UpdateCounters(); }, "DevelopmentPoint");
        this.developmentPoints.AddBonus(-100, new Bonus("Base income", 10f, Bonus.bonusType.Income));
        this.developmentPoints.AddBonus(-200, new Bonus("Research", (float multiplier) => { return GetPopulation() * multiplier; }, (float multiplier) => { return GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 0.09f));

        this.movementPoints = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); }, 0, "MovementPoint");
        this.movementPoints.AddBonus(-100,new Bonus("Base Value",30,Bonus.bonusType.IncreaseLimit));
        this.movementPoints.AddBonus(-200, new Bonus("Provinces", (float multiplier) => { return GetMovementPoints(); }, (float multiplier) => { return ""; })) ;
        this.buildingsPermit = new bool[GameAssets.Instance.buildingsStats.Length];
        this.research = new bool[4,GameAssets.Instance.research.GetLength(1)];
        this.units = new bool[GameAssets.Instance.unitStats.Length];
        this.units[0] = true;

        movementPoints.UpdateLimit();
        warriors.UpdateLimit();
        movementPoints.value = movementPoints.limit;
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
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
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
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
            {
                warriors += (int)GameManager.Instance.provinces[i].warriors.value;
            }
        }
        return warriors;
    }

    public float GetTurnWarriosCost()
    {
        int cost = 0;
        int unitsNumber = GameAssets.Instance.unitStats.Length;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            ProvinceStats province = GameManager.Instance.provinces[i];
            if (province.provinceOwnerIndex == index && province.unitsCounter > 0 && province.units !=null)
            {
                for (int j = 0; j < unitsNumber; j++)
                {
                    if(province.units.ContainsKey(j))
                    {
                        cost += province.units[j] * GameAssets.Instance.unitStats[j].turnCost;
                    }
                }
            }
        }
        return cost;
    }
    public float GetMovementPoints()
    {
        int movementPoints = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
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
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
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
