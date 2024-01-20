using System;

[System.Serializable]

public static class BonusManager 
{
    public static void SetBonus(ProvinceStats provinceStats,int indexBonus)
    {
        PlayerStats playerStats = GameManager.Instance.GetPlayerStats(provinceStats.provinceOwnerIndex);

        if (provinceStats.provinceOwnerIndex != -1)
        {
            switch (indexBonus)
            {
                case 0:
                         provinceStats.lifePoints.AddBonus(-20, provinceStats);
                         provinceStats.warriors.AddBonus(-21, provinceStats);
                    if (!provinceStats.population.bonuses.ContainsKey(-30)) provinceStats.population.AddBonus(-30, provinceStats);
                    break;
                case 1:
                    if (!playerStats.coins.bonuses.ContainsKey(-22))
                        playerStats.coins.AddBonus(-22, playerStats);
                    break;
                case 2:
                     if (!provinceStats.population.bonuses.ContainsKey(-23))
                        provinceStats.population.AddBonus(-23, provinceStats);
                   break;
                case 3:
                    if (!playerStats.developmentPoints.bonuses.ContainsKey(-24))
                        playerStats.developmentPoints.AddBonus(-24,playerStats);
                    break;
                case 4:
                     if (!provinceStats.lifePoints.bonuses.ContainsKey(-25))
                        provinceStats.lifePoints.AddBonus(-25,provinceStats);

                    if (!playerStats.warriors.bonuses.ContainsKey(-26))
                        playerStats.warriors.AddBonus(-26, playerStats);
                    break; 
                case 5:
                    if (!playerStats.coins.bonuses.ContainsKey(-27))
                        playerStats.coins.AddBonus(-27, playerStats);
                    break; 
                case 6:
                    if (!playerStats.coins.bonuses.ContainsKey(-28))
                        playerStats.coins.AddBonus(-28,playerStats);
                    break;
                case 7:
                    if (!playerStats.developmentPoints.bonuses.ContainsKey(-29))
                        playerStats.developmentPoints.AddBonus(-29, playerStats);
                    break;


            }
        }
        if (provinceStats.provinceOwnerIndex == 0) UIManager.Instance.UpdateCounters();
        if (provinceStats.provinceOwnerIndex != -1) UpdateLimits(provinceStats.provinceOwnerIndex);
       
    }

    public static Bonus GetBonus(int index, PlayerStats playerStats, ProvinceStats provinceStats)
    {
        switch (index)
        {
            case -1:
                return new Bonus("Base income", 100f, Bonus.bonusType.Income);
            case -2:
                return new Bonus("Taxes", (float multiplier) => { return playerStats.GetPopulation() * multiplier; }, (float multiplier) => { return playerStats.GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 0.2f);
            case -3:
                return new Bonus("Research funding", (float multiplier) => { return playerStats.GetPopulation() * multiplier; }, (float multiplier) => { return playerStats.GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, -0.02f);
            case -4:
                return new Bonus("Units Cost", (float multiplier) => { return -playerStats.GetTurnWarriosCost(); }, (float multiplier) => { return ""; }, 0f);
            case -5:
                return new Bonus("Base Value", 20, Bonus.bonusType.IncreaseLimit);
            case -6:
                return new Bonus("Provinces", (float multiplier) => { return playerStats.GetWarriors(); }, (float multiplier) => { return ""; });
            case -7:
                return new Bonus("Base income", 30f, Bonus.bonusType.Income);
            case -8:
                return new Bonus("Research", (float multiplier) => { return playerStats.GetPopulation() * multiplier; }, (float multiplier) => { return playerStats.GetPopulation().ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 0.09f);
            case -9:
                return new Bonus("Base Value", 30, Bonus.bonusType.IncreaseLimit);
            case -10:
                return new Bonus("Provinces", (float multiplier) => { return playerStats.GetMovementPoints(); }, (float multiplier) => { return ""; });
            case -11:
                return new Bonus("Taxes", (float multiplier) => { return (int)provinceStats.population.value * multiplier; }, (float multiplier) => { return ((int)provinceStats.population.value).ToString() + Icons.GetIcon("Population") + " x " + multiplier; }, 100);
            case -12:
                return new Bonus("Base income", (float multiplier) => { return 0.1f; }, (float multiplier) => { return ""; }, 0f);
            case -13:
                return new Bonus("Barracks upgrade", 5, Bonus.bonusType.IncreaseLimit);
            case -14:
                return new Bonus("development of education", 1, Bonus.bonusType.Income);
            case -15:
                return new Bonus("development of education", 10, Bonus.bonusType.Income);
            case -16:
                return new Bonus("new tax", 10, Bonus.bonusType.Income);
            case -17:
                return new Bonus("trading", 100, Bonus.bonusType.Income);
            case -18:
                return new Bonus("better management", 5, Bonus.bonusType.IncreaseLimit);
            case -19:
                return new Bonus("better management", 20, Bonus.bonusType.IncreaseLimit);
            case -20:
                return new Bonus("Capital", 20, Bonus.bonusType.Disposable);
            case -21:
                return new Bonus("Capital", 10, Bonus.bonusType.Disposable);
            case -22:
                return new Bonus("Workshops", (float multiplier) => { return playerStats.CountBuildings(1) * multiplier; }, (float multiplier) => { return playerStats.CountBuildings(1).ToString() + " x " + multiplier; }, 10f);
            case -23:
                return new Bonus("farm", 5, Bonus.bonusType.Income);
            case -24:
                return new Bonus("Universities", (float multiplier) => { return playerStats.CountBuildings(2) * multiplier; }, (float multiplier) => { return playerStats.CountBuildings(2).ToString() + " x " + multiplier; }, 5f);
            case -25:
                return new Bonus("Castle", 10, Bonus.bonusType.Disposable);
            case -26:
                return new Bonus("Castles", (float multiplier) => { return playerStats.CountBuildings(4) * multiplier; }, (float multiplier) => { return playerStats.CountBuildings(4).ToString() + " x " + multiplier; }, 5f);
            case -27:
                return new Bonus("Universities", (float multiplier) => { return playerStats.CountBuildings(6) * multiplier; }, (float multiplier) => { return playerStats.CountBuildings(6).ToString() + " x " + multiplier; }, 5f);
            case -28:
                return new Bonus("gold mines", (float multiplier) => { return playerStats.CountBuildings(3) * multiplier; }, (float multiplier) => { return playerStats.CountBuildings(3).ToString() + " x " + multiplier; }, 3f);
            case -29:
                return new Bonus(5, "Ancient ruins", playerStats.GetFunc(5), playerStats.GetStringFunc(5), 3f);
            case -30:
                return new Bonus("Capital", 1, Bonus.bonusType.Income);
        }
        return null;
    }

    public static void RemoveBonus(ProvinceStats provinceStats, int indexBonus)
    {
        switch (indexBonus)
        {
            case 0:
                provinceStats.lifePoints.RemoveBonus(-20);
                provinceStats.warriors.RemoveBonus(-21);
                provinceStats.population.RemoveBonus(-30);
                break;
            case 1:
                break;
            case 2:
                provinceStats.population.RemoveBonus(-23);
                break; 
            case 3:
                break; 
            case 4:
                provinceStats.lifePoints.RemoveBonus(-25);
                break;
        }
        UpdateLimits(provinceStats.provinceOwnerIndex);
    }

    public static  void AddPlayerBonus(PlayerStats playerStats,int bonusIndex)
    {
        switch(bonusIndex)
        {
            case 0:
                playerStats.warriors.AddBonus(-13,playerStats);
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
                playerStats.developmentPoints.AddBonus(-14,playerStats);
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
                playerStats.developmentPoints.AddBonus(-15,playerStats);
                UpdateLimits(playerStats.index);
                break;
            case 11:
                playerStats.selectedSpells[2] = -1;
                UIManager.Instance.LoadSpells();
                UpdateLimits(playerStats.index);
                break;

            case 12:
                playerStats.coins.AddBonus(-16,playerStats);
                UpdateLimits(playerStats.index);
                break;
            case 13:
                playerStats.buildingsPermit[1] = true;
                UpdateLimits(playerStats.index);
                break;
            case 14:
                playerStats.cheaperBuilding = true;
                break;
            case 15:
                playerStats.cheaperRecruitment = true;
                break;
            case 16:
                playerStats.buildingsPermit[6] = true;
                break;
            case 17:
                playerStats.coins.AddBonus(-17,playerStats);
                break;

            case 18:
                playerStats.movementPoints.AddBonus(-18,playerStats);
                break;
            case 19:
                playerStats.buildingsPermit[4] = true;
                break;
            case 20:
                playerStats.movementRecruitment = true;
                break;     
            case 21:
                playerStats.taxManagement = true;
                break; 
            case 22:
                playerStats.movementBuilding = true;
                break;
            case 23:
                playerStats.movementPoints.AddBonus(-19, playerStats);
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
