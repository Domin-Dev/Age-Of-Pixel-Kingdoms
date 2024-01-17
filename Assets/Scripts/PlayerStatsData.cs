
[System.Serializable]
public class PlayerStatsData 
{
    public StatisticData coins;
    public StatisticData warriors;
    public StatisticData developmentPoints;
    public StatisticData movementPoints;

    public bool[] buildingsPermit;
    public bool[,] research;
    public bool[] units;

    public bool[] spells;
    public int[] selectedSpells;

    public int texesIndex;
    public int researchIndex;

    public int index { private set; get; }

    public bool taxManagement;
    public bool researchManagement;

    public bool cheaperBuilding;
    public bool cheaperRecruitment;

    public bool movementBuilding;
    public bool movementRecruitment;

    public PlayerStatsData(PlayerStats playerStats)
    {
        this.coins = new StatisticData(playerStats.coins);
        this.warriors = new StatisticData(playerStats.warriors);
        this.developmentPoints = new StatisticData(playerStats.developmentPoints);
        this.movementPoints = new StatisticData(playerStats.movementPoints);

        this.buildingsPermit = playerStats.buildingsPermit;
        this.research = playerStats.research;
        this.units = playerStats.units;

        this.texesIndex = playerStats.texesIndex;
        this.researchIndex = playerStats.researchIndex;
        this.spells = playerStats.spells;
        this.selectedSpells = playerStats.selectedSpells;
        this.taxManagement = playerStats.taxManagement;
        this.researchManagement = playerStats.researchManagement;

        this.cheaperBuilding = playerStats.cheaperBuilding;
        this.cheaperRecruitment = playerStats.cheaperRecruitment;
        this.movementBuilding = playerStats.movementBuilding;
        this.movementRecruitment = playerStats.movementRecruitment;
        
    }

    public PlayerStats ToPlayerStats()
    {
        PlayerStats playerStats = new PlayerStats();
        playerStats.coins = coins.ToStatistic(playerStats);
        playerStats.warriors = warriors.ToStatistic(playerStats);
        playerStats.developmentPoints = developmentPoints.ToStatistic(playerStats);
        playerStats.movementPoints = movementPoints.ToStatistic(playerStats);

        playerStats.buildingsPermit = buildingsPermit;
        playerStats.research = research;
        playerStats.units = units;

        playerStats.texesIndex = texesIndex;
        playerStats.researchIndex = researchIndex;

        playerStats.index = index;
        playerStats.spells = spells;
        playerStats.selectedSpells = selectedSpells;

        playerStats.taxManagement = taxManagement;
        playerStats.researchManagement = researchManagement;

        playerStats.cheaperBuilding = cheaperBuilding;
        playerStats.cheaperRecruitment =  cheaperRecruitment;
        playerStats.movementBuilding = movementBuilding;
        playerStats.movementRecruitment = movementRecruitment;

        return playerStats;
    }
}
