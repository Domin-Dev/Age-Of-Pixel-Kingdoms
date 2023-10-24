using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.Progress;

public class EnemyManager
{
    private PlayerStats playerStats;
    private int index;

    List<int> provinces = new List<int>();

    public EnemyManager(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
        this.index = playerStats.index;
    }



    public void NextTurn()
    {
        Recruit(provinces[0]);
      if(index == 3)  Scanning();
    }
    private void Recruit(int provinceIndex)
    {
        GameManager.Instance.AIRecruit(provinceIndex, 3, 10, playerStats);
    }

    private List<int2> Scanning()
    {
        ProvinceStats[] allProvinces = GameManager.Instance.provinces;
        List<int2> scan = new List<int2>();
        foreach (int item in provinces)
        {
            int2 value = new int2(item, 0);
            for (int i = 0; i < allProvinces[item].neighbors.Count; i++)
            {
                int provinceIndex = allProvinces[item].neighbors[i];
                int owner = allProvinces[provinceIndex].provinceOwnerIndex;
                if (owner != -1 && owner != index)
                {
                    value.y += allProvinces[provinceIndex].unitsCounter;
                }
                for (int j = 0; j < allProvinces[i].neighbors.Count;j++)
                {
                    provinceIndex = allProvinces[i].neighbors[j];
                    owner = allProvinces[provinceIndex].provinceOwnerIndex;
                    if (owner != -1 && owner != index)
                    {
                        value.y += allProvinces[provinceIndex].unitsCounter;
                    }
                    Debug.Log(provinceIndex);
                }
            }
            scan.Add(value);
        }
        foreach (int2 item in scan)
        {
     //       Debug.Log(item + " " + index);
        }
        return scan;
    }
    public void UpdateProvinces()
    {
        foreach (ProvinceStats item in GameManager.Instance.provinces)
        {
            if (item.provinceOwnerIndex == playerStats.index)
            {
                provinces.Add(item.index);
            }
        }
    }
}
