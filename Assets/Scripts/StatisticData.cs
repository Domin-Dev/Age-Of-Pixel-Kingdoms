using System.Collections.Generic;

[System.Serializable]
public class StatisticData 
{
    public float value;
    public float limit;
    public float turnIncome;
    public Dictionary<int, BonusData> bonuses;

    public string description;
    public string icon;
    public StatisticData(Statistic statistic)
    {
        this.value = statistic.value;
        this.limit = statistic.limit;
        this.turnIncome = statistic.turnIncome;
        bonuses = new Dictionary<int, BonusData>();
        if (statistic.bonuses != null)
        {
            foreach (var item in statistic.bonuses)
            {
                BonusData bonusData = new BonusData(item.Value);
                bonuses.Add(item.Key, bonusData);
            }
        }
        this.description = statistic.description;
        this.icon = statistic.icon;
    }
  
    public Statistic ToStatistic(PlayerStats playerStats)
    {
        Statistic statistic = new Statistic();
        statistic.value = this.value;
        statistic.limit = this.limit;
        statistic.turnIncome = this.turnIncome;
        statistic.bonuses = new Dictionary<int, Bonus>();
        if (this.bonuses != null)
        {
            foreach (var item in bonuses)
            {
                Bonus bonus = item.Value.ToBonus();
                Bonus newBonus = BonusManager.GetBonus(bonus.bonusIndex,playerStats,null);
                newBonus.multiplier = bonus.multiplier;
                newBonus.bonusIndex = bonus.bonusIndex;
                statistic.bonuses.Add(item.Key,newBonus);
            }
        }
        statistic.description = this.description;
        statistic.icon = this.icon;
        return statistic;
    }

    public Statistic ToStatistic(ProvinceStats provinceStats)
    {
        Statistic statistic = new Statistic();
        statistic.value = this.value;
        statistic.limit = this.limit;
        statistic.turnIncome = this.turnIncome;
        statistic.bonuses = new Dictionary<int, Bonus>();
        if (this.bonuses != null)
        {
            foreach (var item in bonuses)
            {
                Bonus bonus = item.Value.ToBonus();
                Bonus newBonus = BonusManager.GetBonus(bonus.bonusIndex,null,provinceStats);
                newBonus.multiplier = bonus.multiplier;
                newBonus.bonusIndex = bonus.bonusIndex;
                statistic.bonuses.Add(item.Key, newBonus);
            }
        }
        statistic.description = this.description;
        statistic.icon = this.icon;
        return statistic;
    }
}
