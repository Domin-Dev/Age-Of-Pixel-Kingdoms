using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ProvinceStats
{

    public Statistic lifePoints;
    public Statistic population;
    public Statistic warriors;


    public int index;
    public float scienceDevelopment { private set; get; }
    public float incomeInCoins { private set; get; }
    public bool isSea;

    public int provinceOwnerIndex  = -1;// -1 == null , 0 is Player, >0 is Computer



    public int unitsCounter;

    public int buildingIndex = -1;

    [SerializeField]public Dictionary<int, int> units;

    [SerializeField]public List<int> neighbors = new List<int>();
    public void AddNeighbor(int index)
    {
        neighbors.Add(index);
    }
    public ProvinceStats(int index, float scienceDevelopment, float incomeInCoins, bool isSea)
    {
        this.index = index;
        this.provinceOwnerIndex = -1;
        this.unitsCounter = 0;
        this.units = new Dictionary<int, int>();
        this.scienceDevelopment = scienceDevelopment;
        this.incomeInCoins = incomeInCoins;
        buildingIndex = -1;
        this.isSea = isSea;
    }  
    public ProvinceStats()
    {

    }
    public void CopyData(ProvinceStats provinceStats)
    {
        population = new Statistic((float turnIncome) => { return turnIncome; },Random.Range(100, 120), 0.5f,null);
        lifePoints = new Statistic(10);
        warriors = new Statistic(5);


        scienceDevelopment = provinceStats.scienceDevelopment;
        incomeInCoins = provinceStats.incomeInCoins;
        isSea =provinceStats.isSea;
        provinceOwnerIndex = provinceStats.provinceOwnerIndex;
        unitsCounter = provinceStats.unitsCounter;
        buildingIndex = provinceStats.buildingIndex;
        neighbors = provinceStats.neighbors;
        index = provinceStats.index;

        if(!provinceStats.isSea && provinceStats.provinceOwnerIndex == -1)
        {
            this.units = new Dictionary<int, int>();
            if (Random.Range(0, 4) != 0)
            {
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
                GameManager.Instance.UpdateUnitCounter(this.index);
            }
        }
    }
    public void SetNewOwner(int index)
    {
        if (index == 0) GameManager.Instance.humanPlayer.warriors.limit += warriors.value;
        provinceOwnerIndex = index;
    }

    public void NextTurn()
    {
        population.NextTurn();
    }
}
