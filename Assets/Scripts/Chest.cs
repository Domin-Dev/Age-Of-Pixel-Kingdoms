using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Chest
{
    public static string OpenChest(PlayerStats playerStats)
    {
        int value = Random.Range(1, 101);
        if(Check(value,1,20))
        {
            playerStats.coins.Add(400);
            return "+ 400" + Icons.GetIcon("Coin");
        }
        else if (Check(value, 21, 30))
        {
            playerStats.coins.Add(1000);
            return "+ 1000" + Icons.GetIcon("Coin");
        }
        else if (Check(value, 31, 35))
        {
            playerStats.coins.Add(2000);
            return "+ 2000" + Icons.GetIcon("Coin");
        }
        else if (Check(value, 36, 55))
        {
            playerStats.developmentPoints.Add(50);
            return "+ 50" + Icons.GetIcon("DevelopmentPoint");
        }
        else if (Check(value, 56, 65))
        {
            playerStats.developmentPoints.Add(150);
            return "+ 150" + Icons.GetIcon("DevelopmentPoint");
        }
        else if (Check(value, 66, 70))
        {
            playerStats.developmentPoints.Add(250);
            return "+ 250" + Icons.GetIcon("DevelopmentPoint");
        }
        else if(Check(value, 71, 90))
        {
            List<int> list = new List<int>();
            for (int i = 0; i < playerStats.spells.Length; i++)
            {
                if (!playerStats.spells[i]) list.Add(i);
            }

            if(list.Count != 0)
            {
                int id =list[Random.Range(0, list.Count)];
                playerStats.spells[id] = true;
                Spell spell = GameAssets.Instance.spells[id];
                UIManager.Instance.LoadSpells();
                return "unlocked spell:" + spell.spellName;
            }
            else
            {
                playerStats.coins.Add(400);
                return "+ 400" + Icons.GetIcon("Coin");
            }
        }
        else if(Check(value,91,100))
        {
            playerStats.developmentPoints.Add(300);
            return "+ 300" + Icons.GetIcon("DevelopmentPoint");
        }

        return "";
    }

    private static bool Check(int value, int min, int max)
    {
        if (value >= min && value <= max)
        {
            return true;
        }
        else
            return false;
    }
}
