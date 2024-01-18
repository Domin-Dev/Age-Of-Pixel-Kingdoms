
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour
{
    public string playerName;
    public bool isComputer;
    public Color playerColor;
    public PlayerStats stats;
    private EnemyManager enemyManager;
    public int index;

    public Player()
    {
        stats = new PlayerStats();
    }
    public void SetUp(string playerName, bool isComputer, Color playerColor, int startCoins, int index)
    {
        if(startCoins >= 0)
        {
            this.stats = new PlayerStats(startCoins, index);
            DontDestroyOnLoad(gameObject);
        }
        this.index = index;
        this.playerName = playerName;
        this.isComputer = isComputer;
        this.playerColor = playerColor;
        if (isComputer)
        {
            enemyManager = gameObject.AddComponent<EnemyManager>();
            enemyManager.SetUp(stats);
        }
    }

    public void EnemyManagerSetUp()
    {
        enemyManager = gameObject.AddComponent<EnemyManager>();
        enemyManager.SetUp(stats);
    }



    public void RunEnemyManager()
    {
       StartCoroutine(enemyManager.NextTurnFunction());
    }

    public void UpdateProvinces()
    {
      enemyManager.UpdateProvinces();
    }
}
