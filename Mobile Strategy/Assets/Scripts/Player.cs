using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Player
{
    public string playerName;

    public bool isComputer;

    public Color playerColor;

    public Player(string playerName,bool isComputer,Color playerColor)
    {
        this.playerName = playerName;
        this.isComputer = isComputer;   
        this.playerColor = playerColor;
    }
}
