using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

[System.Serializable]
public class StatisticData 
{
    public float value;
    public float limit;
    public float turnIncome;
    public Dictionary<int, Bonus> bonuses;
    public StatisticData(Statistic statistic)
    {
        this.value = statistic.value;
        this.limit = statistic.limit;
        this.turnIncome = statistic.turnIncome;
        this.bonuses = statistic.bonuses;
    }

    public Statistic ToStatistic()
    {
        Statistic statistic = new Statistic();
        statistic.value = this.value;
        statistic.limit = this.limit;
        statistic.turnIncome = this.turnIncome;
        statistic.bonuses = this.bonuses;
        return statistic;
    }
}
