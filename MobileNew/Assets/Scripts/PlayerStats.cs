using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int coins {private set; get; }
    public int income { private set; get; }

    public PlayerStats(int coins) 
    {
         this.coins = coins;
    }
    public void Subtract(int value)
    {
        coins = Mathf.Clamp(coins - value, 0, int.MaxValue);
        UIManager.Instance.UpdateCounters();
    }

    public bool CanAfford(int value)
    {
        if (coins >= value)
            return true;
        else 
            return false;
    }
}
