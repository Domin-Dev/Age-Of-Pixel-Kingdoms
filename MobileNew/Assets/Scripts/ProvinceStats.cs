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

    public int unitsCounter;

    public Building building;

    public Dictionary<int, int> units;

    public string[] neighbors;
    public void SetUp(Object[] neighbors)
    {
       this.neighbors = new string[neighbors.Length];
       for (int i = 0;i < neighbors.Length;i++)
       {
            this.neighbors[i] = neighbors[i].name;
       }
    }
    public ProvinceStats(int population, int warriorsLimit, float scienceDevelopment, float incomeInCoins, Building building)
    {
        units = new Dictionary<int, int>();
        this.unitsCounter = 0;
        this.population = population;
        this.warriorsLimit = warriorsLimit;
        this.scienceDevelopment = scienceDevelopment;
        this.incomeInCoins = incomeInCoins;
        this.building = building;
    }

    public enum Building
    {
        None,
        fortification,
        university,
        workshop,
    }

}
