
public static class BonusManager
{
    public static void SetBonus(ProvinceStats provinceStats,int indexBonus)
    {
        switch (indexBonus)
        {
            case 0:
                provinceStats.lifePoints.AddBonus(0,new Bonus("Capital", 20,Bonus.bonusType.Disposable));
                provinceStats.warriors.AddBonus(1,new Bonus("Capital", 10,Bonus.bonusType.Disposable));
                break;
            case 1:
                provinceStats.population.AddBonus(2, new Bonus("ko", 10, Bonus.bonusType.Income));
                break;
            case 2:
             //   provinceStats.developmentPoints.AddBonus(3, new Bonus("ko", 5, Bonus.bonusType.Income));
                break;

        }
      if(provinceStats.provinceOwnerIndex != -1) UpdateLimits(provinceStats.provinceOwnerIndex);
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
                playerStats.warriors.AddBonus(0, new Bonus("Barracks upgrade", 5, Bonus.bonusType.IncreaseLimit));
                UpdateLimits(playerStats.index);
                break;
            case 1:
                playerStats.units[1] = true;
                UpdateLimits(playerStats.index);
                break; 
            case 2:
                playerStats.units[2] = true;
                UpdateLimits(playerStats.index);
                break; 
            case 3:
                playerStats.units[3] = true;
                UpdateLimits(playerStats.index);
                break;
            case 4:
                playerStats.units[4] = true;
                UpdateLimits(playerStats.index);
                break;
            case 5:
                playerStats.units[5] = true;
                UpdateLimits(playerStats.index);
                break;

            case 6:
                playerStats.movementPoints.AddBonus(0, new Bonus("better management", 5, Bonus.bonusType.IncreaseLimit));
                UpdateLimits(playerStats.index);
                break;
            case 7:
                playerStats.buildingsPermit[2] = true;
                UpdateLimits(playerStats.index);
                break;  
            case 8:
                playerStats.researchManagement = true;
                UpdateLimits(playerStats.index);
                break;
            case 9:
                playerStats.selectedSpells[1] = -1;
                UIManager.Instance.LoadSpells();
                UpdateLimits(playerStats.index);
                break;
            case 10:
                playerStats.selectedSpells[1] = -1;
                UpdateLimits(playerStats.index);
                break;
            case 11:
                playerStats.selectedSpells[2] = -1;
                UIManager.Instance.LoadSpells();
                UpdateLimits(playerStats.index);
                break;
            case 12:
                playerStats.coins.AddBonus(12, new Bonus("new tax", 10, Bonus.bonusType.Income));
                UpdateLimits(playerStats.index);
                break;







        }
        if(playerStats.index == 0) UIManager.Instance.UpdateCounters();
    }
    public static void UpdateLimits(int index)
    {
        if (index == 0)
        {
            GameManager.Instance.humanPlayer.stats.warriors.UpdateLimit();
            GameManager.Instance.humanPlayer.stats.movementPoints.UpdateLimit();
        }
        else
        {
            GameManager.Instance.botsList[index - 1].stats.warriors.UpdateLimit();
            GameManager.Instance.botsList[index - 1].stats.movementPoints.UpdateLimit();
        }
    }
}
