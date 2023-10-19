using System;
using System.Diagnostics;
using UnityEngine;

public static class BonusManager
{
    public static void SetBonus(ProvinceStats provinceStats,int indexBonus)
    {
        switch (indexBonus)
        {
            case 0:
                provinceStats.lifePoints.AddBonus(0,new Bonus("Capital", 20,Bonus.bonusType.Disposable));
                provinceStats.warriors.AddBonus(1,new Bonus("Capital", 5,Bonus.bonusType.Disposable));
                break;
            case 1:
                provinceStats.population.AddBonus(2, new Bonus("ko", 10, Bonus.bonusType.Income));
                break;
            case 2:
             //   provinceStats.developmentPoints.AddBonus(3, new Bonus("ko", 5, Bonus.bonusType.Income));
                break;

        }
        UpdateLimits(provinceStats.provinceOwnerIndex);
    }
    public static void RemoveBonus(ProvinceStats provinceStats, int indexBonus)
    {
        switch (indexBonus)
        {
            case 0:
                provinceStats.lifePoints.RemoveBonus(0);
                provinceStats.warriors.RemoveBonus(1);
                break;
            case 1:
                provinceStats.population.RemoveBonus(2);
                break;
            case 2:
                provinceStats.population.RemoveBonus(3);
                break;
        }
        UpdateLimits(provinceStats.provinceOwnerIndex);
    }

    public static void AddPlayerBonus(PlayerStats playerStats,int bonusIndex)
    {
        
        switch(bonusIndex)
        {
            case 0:
                playerStats.buildingsPermit[0] = true;
                playerStats.CanBuild(0);
                break;
        }
    }

    private static void UpdateLimits(int index)
    {
        if (index == 0) GameManager.Instance.humanPlayer.stats.warriors.UpdateLimit();
        else GameManager.Instance.botsList[index - 1].stats.warriors.UpdateLimit();
    }
}
