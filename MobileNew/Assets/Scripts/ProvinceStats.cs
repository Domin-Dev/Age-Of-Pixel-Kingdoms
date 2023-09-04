using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProvinceStats
{

    public int index;
    public int population { private set; get; }
    public int warriorsLimit { private set; get; }
    public float scienceDevelopment { private set; get; }
    public float incomeInCoins { private set; get; }
    public bool isSea;

    public int provinceOwnerIndex  = -1;// -1 == null , 0 is Player, >0 is Computer



    public int unitsCounter;

    public int buildingIndex = -1;

    public Dictionary<int, int> units;

    public List<int> neighbors = new List<int>();
    public void AddNeighbor(int index)
    {
        neighbors.Add(index);
    }
    public ProvinceStats(int index,int population, int warriorsLimit, float scienceDevelopment, float incomeInCoins, bool isSea)
    {
        this.index = index;
        Debug.Log(index);
        this.provinceOwnerIndex = -1;
        units = new Dictionary<int, int>();
        this.unitsCounter = 0;
        this.population = population;
        this.warriorsLimit = warriorsLimit;
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
        population = provinceStats.population;
        warriorsLimit = provinceStats.warriorsLimit;
        scienceDevelopment = provinceStats.scienceDevelopment;
        incomeInCoins = provinceStats.incomeInCoins;
        isSea =provinceStats.isSea;
        provinceOwnerIndex = provinceStats.provinceOwnerIndex;
        unitsCounter = provinceStats.unitsCounter;
        buildingIndex = provinceStats.buildingIndex;
        units = provinceStats.units;
        neighbors = provinceStats.neighbors;
        index = provinceStats.index;
    }
    public void SetNewOwner(int index)
    {
        provinceOwnerIndex = index;
    }

}
