using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


[System.Serializable]
public class Player : MonoBehaviour
{
    public string playerName { private set; get; }

    public bool isComputer { private set; get; }

    public Color playerColor { private set; get; }

    public PlayerStats stats;
    private EnemyManager enemyManager;
    public int index { private set; get; }
    public void SetUp(string playerName, bool isComputer, Color playerColor, int startCoins, int index)
    {
        this.stats = new PlayerStats(startCoins, index);
        this.index = index;
        this.playerName = playerName;
        this.isComputer = isComputer;
        this.playerColor = playerColor;
        if (isComputer) enemyManager = new EnemyManager(stats);
        DontDestroyOnLoad(gameObject);
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
