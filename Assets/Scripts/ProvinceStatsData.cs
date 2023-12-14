using System.Collections.Generic;

[System.Serializable]
public class ProvinceStatsData 
{
    public StatisticData lifePoints;
    public StatisticData population;
    public StatisticData warriors;
    public StatisticData movementPoints;

    public int index;
    public bool isSea;
    public int provinceOwnerIndex;
    public bool chest;

    public int unitsCounter;
    public int buildingIndex;
    public Dictionary<int, int> units;
    public List<int> neighbors = new List<int>();

    public ProvinceStatsData(ProvinceStats provinceStats)
    {
        this.lifePoints = new StatisticData(provinceStats.lifePoints);
        this.population = new StatisticData(provinceStats.population);
        this.warriors = new StatisticData(provinceStats.warriors);
        this.movementPoints = new StatisticData(provinceStats.movementPoints);

        this.index = provinceStats.index;
        this.isSea = provinceStats.isSea;
        this.provinceOwnerIndex = provinceStats.provinceOwnerIndex;

        this.unitsCounter = provinceStats.unitsCounter;
        this.buildingIndex = provinceStats.buildingIndex;
        this.units = provinceStats.units;
        this.neighbors = provinceStats.neighbors;
        this.chest = provinceStats.chest;
    }

    public ProvinceStats ToProvinceStats()
    {
        ProvinceStats provinceStats = new ProvinceStats();
        provinceStats.lifePoints = this.lifePoints.ToStatistic();
        provinceStats.population = this.population.ToStatistic();
        provinceStats.warriors = this.warriors.ToStatistic();
        provinceStats.movementPoints = this.movementPoints.ToStatistic();

        provinceStats.index = this.index;
        provinceStats.isSea = this.isSea;
        provinceStats.provinceOwnerIndex = this.provinceOwnerIndex;

        provinceStats.unitsCounter = this.unitsCounter;
        provinceStats.buildingIndex = this.buildingIndex;
        provinceStats.units = this.units;
        provinceStats.neighbors = this.neighbors;
        provinceStats.chest = this.chest;
        return provinceStats;
    }
}
