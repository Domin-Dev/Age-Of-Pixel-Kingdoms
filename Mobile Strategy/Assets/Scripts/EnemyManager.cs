using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyManager
{
    private PlayerStats playerStats;
    private int index;

    List<int> provinces = new List<int>();
    List<float2> lastScan;

    float[] powerUnits;
    public EnemyManager(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
        this.index = playerStats.index;
        powerUnits = new float[GameAssets.Instance.unitStats.Length];
        for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
        {
            powerUnits[i] = GameAssets.Instance.unitStats[i].battleValue;
        }
    }



    public void NextTurn()
    {
        Recruit(provinces[0], 0f);
        lastScan = Scanning();
        foreach (float2 i in lastScan) 
        { 
            if(i.y > 2f)
            {
                Recruit((int)i.x, i.y);
            }
        }
        //   Recruit();
       //  if (index == 2)// Move();
      //  GameManager.Instance.pathFinding.FindPath(0, 21);
    }
    private void Recruit(int provinceIndex, float battlePower)
    {
        int[] units = new int[powerUnits.Length];
        while (battlePower > 0f)
        {
            int index = UnityEngine.Random.Range(0, units.Length);
            battlePower -= powerUnits[index];
            units[index]++;
        }
        int selectedIndex = provinceIndex;
        int[] ints = { 1, 1, 1, 1, 1, 1 };
        GameManager.Instance.selectingProvinces.AIRecruitArray(provinces[0], ints, playerStats);
    }
    private void Move()
    {
        float maxValue = float.MinValue;
        int selectedIndex = provinces[0];
        foreach (float2 i in lastScan)
        {
            if (i.y > maxValue)
            {
                maxValue = i.y;
                selectedIndex = (int)i.x;
            }
        }
        GameManager.Instance.selectingProvinces.AIMove(provinces[0], provinces[1],1,3);
    }
    private List<float2> Scanning()
    {
        ProvinceStats[] allProvinces = GameManager.Instance.provinces;
        List<float2> scan = new List<float2>();
        foreach (int item in provinces)
        {
            float2 value = new float2((float)item, 0);
            for (int i = 0; i < allProvinces[item].neighbors.Count; i++)
            {
                int provinceIndex = allProvinces[item].neighbors[i];
                int owner = allProvinces[provinceIndex].provinceOwnerIndex;
                if (owner != -1 && owner != index)
                {
                    value.y += CountUnits(allProvinces[provinceIndex]) + 3f;
                }
                for (int j = 0; j < allProvinces[provinceIndex].neighbors.Count;j++)
                {
                    int provinceIndex2 = allProvinces[provinceIndex].neighbors[j];
                    owner = allProvinces[provinceIndex2].provinceOwnerIndex;
                    if (owner != -1 && owner != index)
                    {
                        value.y += 0.4f * CountUnits(allProvinces[provinceIndex2]);
                    }
                }
            }
            value.y -= CountUnits(allProvinces[item]);
            scan.Add(value);
        }

        foreach (var item in scan)
        {
            Debug.Log(item);
        }
        return scan;
    }
    private float CountUnits(ProvinceStats provinceStats)
    {
        float value = 0;
        if(provinceStats.units !=null)
        {
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
                if (provinceStats.units.ContainsKey(i))
                {
                    value += GameAssets.Instance.unitStats[i].battleValue * provinceStats.units[i];
                }
            }
        }
        return value;
    }
    public void UpdateProvinces()
    {
        provinces.Clear();
        foreach (ProvinceStats item in GameManager.Instance.provinces)
        {
            if (item.provinceOwnerIndex == playerStats.index)
            {
                provinces.Add(item.index);
            }
        }
    }
    private ProvinceStats GetProvince(int index)
    {
        return GameManager.Instance.provinces[index];
    }
}
