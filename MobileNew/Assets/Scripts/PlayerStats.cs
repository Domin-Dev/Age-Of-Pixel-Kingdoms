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
}
