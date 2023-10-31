using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyManager
{
    private PlayerStats playerStats;
    private int index;
    private EnemySettings settings;

    List<int> provinces = new List<int>();
    List<float2> lastScan;
    List<float3> neighbors;

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
        neighbors = new List<float3>();
    }



    public void NextTurn()
    {
        lastScan = Scanning();
        if (provinces.Count > 1)
        {

        }
        foreach (float2 i in lastScan) 
        {
            if(i.y > 0.5f)
            {
               Defense((int)i.x, i.y);
            }
        }

        foreach (var item in neighbors)
        {
            if(item.y == 0)
            {
                Attack((int)item.x);
            }
        }

        //   Recruit();
       //  if (index == 2)// Move();
      //  GameManager.Instance.pathFinding.FindPath(0, 21);
    }
    private bool Recruit(int provinceIndex, float battlePower)
    {
        int[] units = new int[powerUnits.Length];
        while (battlePower > 0f)
        {
              int index = UnityEngine.Random.Range(0, units.Length);
              battlePower -= powerUnits[index];
              units[index]++;
        }
       return GameManager.Instance.selectingProvinces.AIRecruitArray(provinceIndex, units, playerStats);
    }

    private void Defense(int provinceIndex, float battlePower)
    {
        if (!Recruit(provinceIndex, battlePower) && battlePower > 1f)
        {
            ProvinceStats province = GameManager.Instance.provinces[provinceIndex];
            for (int i = 0; i < province.neighbors.Count; i++)
            {
                ProvinceStats provinceStats = GameManager.Instance.provinces[province.neighbors[i]];
                if(provinceStats.provinceOwnerIndex == index)
                {
                    if(FindScan(provinceStats.index) < 1f)
                    {
                        Move(provinceStats.index,provinceIndex, battlePower);
                    }
                }
            }
        }
    }
    private void Attack(int target)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[target];
        int value = -1;
        int maxUnits = 0;
        foreach (int item in provinceStats.neighbors)
        {
            ProvinceStats province = GameManager.Instance.provinces[item];
            if (province.provinceOwnerIndex == index)
            {
                if (province.unitsCounter >= maxUnits)
                {
                    value = province.index; ;
                    maxUnits = province.unitsCounter;
                }
            }
        }
        GameManager.Instance.selectingProvinces.AutoBattle(false, value,target);
    }
    private void Move(int from,int to, float battlePower)
    {
        List<float2> unitsFrom = new List<float2>();
        ProvinceStats province = GameManager.Instance.provinces[from];
        if (unitsFrom != null)
        { 
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
               if(province.units.ContainsKey(i)) unitsFrom.Add(new float2(i, province.units[i]));
            }

            int[] units = new int[powerUnits.Length];
            while (battlePower > 0f)
            {
                int indexList = UnityEngine.Random.Range(0, unitsFrom.Count);
                int index = (int)unitsFrom[indexList].x;

                battlePower -= powerUnits[index];
                units[index]++;
                float2 float2 = unitsFrom[indexList];
                float2.y--;
                unitsFrom[indexList] = float2;
                if(float2.y <= 0)
                {
                    unitsFrom.RemoveAt(indexList);
                }
            }
            GameManager.Instance.selectingProvinces.AIMoveArray(units, from, to);
        }
    }

    private float FindScan(int index)
    {
        foreach (float2 item in lastScan)
        {
            if (item.x == index)
                return item.y;
        }
        return 0;
    }
    private List<float2> Scanning()
    {
        ProvinceStats[] allProvinces = GameManager.Instance.provinces;
        List<float2> scan = new List<float2>();
        neighbors.Clear();
        foreach (int item in provinces)
        {
            float2 value = new float2((float)item, 0);
            for (int i = 0; i < allProvinces[item].neighbors.Count; i++)
            {
                int provinceIndex = allProvinces[item].neighbors[i];
                int owner = allProvinces[provinceIndex].provinceOwnerIndex;
                if (owner != index)
                {
                    float power = CountUnits(allProvinces[provinceIndex]);
                    if (owner != -1) value.y += power + 0.5f;
                    neighbors.Add(new float3(provinceIndex,power,item));
                }

                for (int j = 0; j < allProvinces[provinceIndex].neighbors.Count;j++)
                {
                    int provinceIndex2 = allProvinces[provinceIndex].neighbors[j];
                    owner = allProvinces[provinceIndex2].provinceOwnerIndex;
                    if (owner != -1 && owner != index)
                    {
                        value.y += 0.3f * CountUnits(allProvinces[provinceIndex2]);
                    }
                }
            }
            value.y -= CountUnits(allProvinces[item]);
            scan.Add(value);
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
