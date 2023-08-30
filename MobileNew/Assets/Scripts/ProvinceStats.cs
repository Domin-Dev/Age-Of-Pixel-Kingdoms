using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProvinceStats
{


    public int population;
    public int warriorsLimit;
    public float scienceDevelopment;
    public float incomeInCoins;
    public bool isSea;


    public int unitsCounter;

    public int buildingIndex = -1;

    public Dictionary<int, int> units;

    public List<int> neighbors = new List<int>();
    public void AddNeighbor(int index)
    {
        neighbors.Add(index);
    }
    public ProvinceStats(int population, int warriorsLimit, float scienceDevelopment, float incomeInCoins, bool isSea)
    {
        units = new Dictionary<int, int>();
        this.unitsCounter = 0;
        this.population = population;
        this.warriorsLimit = warriorsLimit;
        this.scienceDevelopment = scienceDevelopment;
        this.incomeInCoins = incomeInCoins;
        buildingIndex = -1;
        this.isSea = isSea;
    }



}
