using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProvinceStats
{

    public Statistic lifePoints;
    public Statistic population;
    public Statistic warriors;
    public Statistic movementPoints;

    public int index;
    public bool isSea;
    public int provinceOwnerIndex  = -1;// -1 == null , 0 is Player, >0 is Computer

    public bool chest;
    public int unitsCounter;

    public int buildingIndex = -1;

    [SerializeField]public Dictionary<int, int> units;

    [SerializeField]public List<int> neighbors = new List<int>();
    public void AddNeighbor(int index)
    {
        neighbors.Add(index);
    }
    public ProvinceStats(int index, bool isSea)
    {
        this.index = index;
        this.provinceOwnerIndex = -1;
        this.unitsCounter = 0;
        this.units = new Dictionary<int, int>();
        buildingIndex = -1;
        this.isSea = isSea;
    }  
    public ProvinceStats()
    {

    }
    public void CopyData(ProvinceStats provinceStats)
    {
        population = new Statistic(Random.Range(50, 70), "Population");

        lifePoints = new Statistic(10, "LifePoint");
        warriors = new Statistic(5, "Warrior");
        movementPoints = new Statistic(2,"MovementPoint");


        isSea = provinceStats.isSea;
        provinceOwnerIndex = provinceStats.provinceOwnerIndex;
        unitsCounter = provinceStats.unitsCounter;
        buildingIndex = provinceStats.buildingIndex;
        neighbors = provinceStats.neighbors;
        index = provinceStats.index;

        GameManager.Instance.GetValuesByTaxesIndex(GameManager.Instance.humanPlayer.stats.texesIndex,out float coins, out float people);
        population.AddBonus(-11, this);
        population.bonuses[-11].multiplier = people;
        population.AddBonus(-12, this);


        if (provinceOwnerIndex == -1 && !isSea)
        {
            chest = Random.Range(1, 11) > 8;
            if (!chest)
            {
                if (Random.Range(1, 16) == 10) buildingIndex = 3;
                else if (Random.Range(1, 16) == 10) buildingIndex = 5;
            }
        }


        if (!provinceStats.isSea && provinceStats.provinceOwnerIndex == -1)
        {
            this.units = new Dictionary<int, int>();
          //  if (Random.Range(0, 1) != 0)
           // {
                for (int i = 0; i < 4; i++)
                {
                    int number = Random.Range(0, 3);
                    int unitIndex = Random.Range(0, GameAssets.Instance.unitStats.Length);
                    if (units.ContainsKey(unitIndex))
                    {
                        units[unitIndex] = units[unitIndex] + number;
                    }
                    else
                    {
                        units.Add(unitIndex, number);
                    }

                    unitsCounter = unitsCounter + number;
                }
            //    GameManager.Instance.UpdateUnitCounter(this.index);
           // }
        }
    }
    public void SetNewOwner(int index)
    {
        int lastOwner = provinceOwnerIndex;
        provinceOwnerIndex = index;
        if (lastOwner > 0) GameManager.Instance.botsList[lastOwner - 1].UpdateProvinces();
        if (index == 0)
        {
            GameManager.Instance.UpdateNeighbors(this.index);
            GameManager.Instance.humanPlayer.stats.warriors.limit += warriors.value;
            GameManager.Instance.humanPlayer.stats.movementPoints.limit += movementPoints.value;
            GameManager.Instance.GetValuesByTaxesIndex(GameManager.Instance.humanPlayer.stats.texesIndex, out float coins, out float people);
            population.bonuses[-11].multiplier = people;
            if(buildingIndex != -1) BonusManager.SetBonus(this, GameAssets.Instance.buildingsStats[buildingIndex].bonusIndex);
            UIManager.Instance.UpdateCounters();
        }
        else
        {
            GameManager.Instance.UpdateUnitCounter(this.index);
            GameManager.Instance.botsList[index - 1].stats.warriors.limit += warriors.value;
            GameManager.Instance.botsList[index -1].stats.movementPoints.limit += movementPoints.value;
            GameManager.Instance.GetValuesByTaxesIndex(GameManager.Instance.botsList[index - 1].stats.texesIndex, out float coins, out float people);
            if (buildingIndex != -1) BonusManager.SetBonus(this, GameAssets.Instance.buildingsStats[buildingIndex].bonusIndex);
            population.bonuses[-11].multiplier = people;
        }

        if (lastOwner >= 0)
        {
            GameManager.Instance.CheckPLayer();
            if (lastOwner == 0)
            {            
                GameManager.Instance.UpdateNeighbors(this.index);
                GameManager.Instance.humanPlayer.stats.warriors.limit -= warriors.value;
                GameManager.Instance.humanPlayer.stats.movementPoints.limit -= movementPoints.value;
                UIManager.Instance.UpdateCounters();
            }
            else
            {             
                GameManager.Instance.botsList[lastOwner - 1].stats.warriors.limit -= warriors.value;
                GameManager.Instance.botsList[lastOwner - 1].stats.movementPoints.limit -= movementPoints.value;
            }
        }
    }
    public void NextTurn()
    {
        population.NextTurn();
    }

    public void Clear()
    {
        chest = false;
        buildingIndex = -1;
        units = new Dictionary<int, int>();
        unitsCounter = 0;
        GameManager.Instance.UpdateUnitCounter(this.index);
    }
}
