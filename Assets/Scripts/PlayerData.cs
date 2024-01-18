using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public bool isComputer;
    public float[] playerColor = new float[4];
    public PlayerStatsData stats;
    public int index;
    public PlayerData(Player player)
    {
        this.playerName = player.name;
        this.isComputer = player.isComputer;
        playerColor[0] = player.playerColor.r;
        playerColor[1] = player.playerColor.g;
        playerColor[2] = player.playerColor.b;
        playerColor[3] = player.playerColor.a;
        this.stats = new PlayerStatsData(player.stats);
        this.index = player.index;
    }

    public Player ToPlayer()
    {
        Player player = new Player();
        this.stats.ToPlayerStats(ref player.stats);
        Color color = new Color(playerColor[0], playerColor[1], playerColor[2], playerColor[3]);
        player.SetUp(playerName, isComputer, color, -1, index);
        return player;
    }
}
