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
                provinceStats.lifePoints.AddBonus(0,new Bonus("Castle",10,Bonus.bonusType.Disposable));
                provinceStats.warriors.AddBonus(1,new Bonus("Castle",5,Bonus.bonusType.Disposable));
                GameManager.Instance.humanPlayer.warriors.UpdateLimit();
                break;
            case 1:
                provinceStats.population.AddBonus(2, new Bonus("ko", 10, Bonus.bonusType.Income));
                break;
            case 2:
             //   provinceStats.developmentPoints.AddBonus(3, new Bonus("ko", 5, Bonus.bonusType.Income));
                break;

        }
    }

    public static void RemoveBonus(ProvinceStats provinceStats, int indexBonus)
    {
        switch (indexBonus)
        {
            case 0:
                provinceStats.lifePoints.RemoveBonus(0);
                provinceStats.warriors.RemoveBonus(1);
                GameManager.Instance.humanPlayer.warriors.UpdateLimit();
                break;
            case 1:
                provinceStats.population.RemoveBonus(2);
                break;
            case 2:
                provinceStats.population.RemoveBonus(3);
                break;
        }
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
}
