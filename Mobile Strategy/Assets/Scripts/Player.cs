using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Player
{
    public string playerName { private set; get; }

    public bool isComputer { private set; get; }

    public Color playerColor { private set; get; }

    public PlayerStats stats;
    public int index { private set; get; }

    public Player(string playerName,bool isComputer,Color playerColor,int startCoins, int index)
    {
        this.stats = new PlayerStats(startCoins,index);
        this.index = index;
        this.playerName = playerName;
        this.isComputer = isComputer;   
        this.playerColor = playerColor;
    }
}
