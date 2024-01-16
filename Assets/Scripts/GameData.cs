
[System.Serializable]
public class GameData 
{
    public ProvinceStatsData[] provinces;
    public PlayerData[] playerStats;


    public GameData(ProvinceStats[] provinces, Player[] players)
    {
        this.provinces = new ProvinceStatsData[provinces.Length];
        for (int i = 0; i < provinces.Length; i++)
        {
            this.provinces[i] = new ProvinceStatsData(provinces[i]);
        }
        this.playerStats = new PlayerData [players.Length];
        for (int i = 0; i < players.Length; i++)
        {
             this.playerStats[i] = new PlayerData(players[i]);
        }
    }
    public ProvinceStats[] LoadProvinces()
    {
        ProvinceStats[] map = new ProvinceStats[provinces.Length];
        for (int i = 0; i < provinces.Length; i++)
        {
            map[i] = provinces[i].ToProvinceStats();
        }
        return map;
    }

    public PlayerData[] GetPlayers()
    {
        return playerStats;
    }
}


