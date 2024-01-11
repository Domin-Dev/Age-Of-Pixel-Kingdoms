using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.Progress;

public class EnemyManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private int index;
    private EnemySettings settings;

    List<int> provinces = new List<int>();
    List<float2> lastScan;
    List<float3> neighbors;

    bool done;

    float[] powerUnits;
    public void SetUp(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
        this.index = playerStats.index;
        powerUnits = new float[GameAssets.Instance.unitStats.Length];
        for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
        {
            powerUnits[i] = GameAssets.Instance.unitStats[i].battleValue;
        }
        neighbors = new List<float3>();
        done = false;
    }

    public IEnumerator NextTurnFunction()
    {
        UpdateProvinces();
        lastScan = Scanning();
        foreach (float2 i in lastScan)
        {
            if (i.y > 0.5f)
            {
                Defense((int)i.x, i.y);
             //   yield return new WaitUntil(() => done);                
            }
        }

        if (playerStats.warriors.value / playerStats.warriors.limit < 0.2f)
        {
            Recruit(provinces[UnityEngine.Random.Range(0, provinces.Count)], 3f);
          //  yield return new WaitUntil(() => done);
        }

        foreach (var item in neighbors)
        {
            if (index == 1) Debug.Log(item);
            if (item.y < 3f && item.x != -1)
            {
               StartCoroutine(Attack((int)item.x));
           //    yield return new WaitUntil(() => done);
            }
        }
        GameManager.Instance.ready = true;
        yield return 0;
    }
    private bool Recruit(int provinceIndex, float battlePower)
    {
      //  done = false;
        int[] units = new int[powerUnits.Length];
        while (battlePower > 0f)
        {
              int index = UnityEngine.Random.Range(0, units.Length);
              battlePower -= powerUnits[index];
              units[index]++;
        }
      //  GameManager.Instance.cameraController.SetProvince(GameManager.Instance.map.GetChild(provinceIndex), () => { done = true; });
        return GameManager.Instance.selectingProvinces.AIRecruitArray(provinceIndex, units, playerStats);
    }

    private void Defense(int provinceIndex, float battlePower)
    {
        float value = battlePower;
        if (!Recruit(provinceIndex, battlePower) && battlePower > 1f)
        {
            ProvinceStats province = GameManager.Instance.provinces[provinceIndex];
            for (int i = 0; i < province.neighbors.Count; i++)
            {
                ProvinceStats provinceStats = GameManager.Instance.provinces[province.neighbors[i]];
                if(provinceStats.provinceOwnerIndex == index)
                {
                    float battleValue = FindScan(provinceStats.index);
                    if (battleValue < 0f)
                    {
                        value += battleValue;
                        Move(provinceStats.index,provinceIndex, battlePower);
                    }
                    else 
                    {
                        for (int j = 0; j < provinceStats.neighbors.Count; j++)
                        {
                            ProvinceStats neighbor = GameManager.Instance.provinces[provinceStats.neighbors[j]];
                            float neighborValue = FindScan(neighbor.index);
                            if (neighborValue < 0f)
                            {
                                value += battleValue;
                                Move(neighbor.index, provinceStats.index, battlePower);
                            }
                        }
                    }
                }

                if(value <= 0f)
                {
                    break;
                }              
            }
        }
    }
    IEnumerator Attack(int target)
    {
        float enemyPower = CountUnits(GameManager.Instance.provinces[target]);
        ProvinceStats provinceStats = GameManager.Instance.provinces[target];
        if (provinceStats.provinceOwnerIndex != index)
        {
            done = false;
            int value = -1;
            float maxPower = 0;
            foreach (int item in provinceStats.neighbors)
            {
                ProvinceStats province = GameManager.Instance.provinces[item];
                if (province.provinceOwnerIndex == index)
                {
                    float currentPower = CountUnits(province);
                    if (currentPower >= maxPower)
                    {
                        value = province.index;
                        maxPower = currentPower;
                    }
                }
            }

            if (enemyPower > maxPower * 1.1f || maxPower == 0)
            {
                Recruit(value, enemyPower - maxPower);
                yield return new WaitForSeconds(1);
            }

            RemoveNeighbor(index);
            if (maxPower > 0)
            {
                GameManager.Instance.cameraController.SetProvince(GameManager.Instance.map.GetChild(target), () => { done = true; });
                if (maxPower >= enemyPower * 0.9)
                {
                    GameManager.Instance.selectingProvinces.AutoBattle(false, value, target);
                    Debug.Log(CheckPower(target));
                }
                
            }else
            {
                done = true;
            }
        }else
        {
            done = true;
        }
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
                if (unitsFrom.Count > 0)
                {
                    int indexList = UnityEngine.Random.Range(0, unitsFrom.Count);
                    int index = (int)unitsFrom[indexList].x;

                    battlePower -= powerUnits[index];
                    units[index]++;
                    float2 float2 = unitsFrom[indexList];
                    float2.y--;
                    unitsFrom[indexList] = float2;
                    if (float2.y <= 0)
                    {
                        unitsFrom.RemoveAt(indexList);
                    }
                }
                else
                {
                    break;
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

    private void RemoveNeighbor(int index)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].x == index)
            {
                float3 float3 = neighbors[i];
                float3.x = -1;
                neighbors[i] = float3;
                return;
            }
        }
    }

    private float CheckPower(int indexProvince)
    {
        ProvinceStats[] allProvinces = GameManager.Instance.provinces;

        float value = 0;
        for (int i = 0; i < allProvinces[indexProvince].neighbors.Count; i++)
        {
            int provinceIndex = allProvinces[indexProvince].neighbors[i];
            int owner = allProvinces[provinceIndex].provinceOwnerIndex;

            if (owner != index)
            {
                float power = CountUnits(allProvinces[provinceIndex]);
                if (owner != -1) value += power + 0.5f;
            }

            for (int j = 0; j < allProvinces[provinceIndex].neighbors.Count; j++)
            {
                int provinceIndex2 = allProvinces[provinceIndex].neighbors[j];
                owner = allProvinces[provinceIndex2].provinceOwnerIndex;
                if (owner != -1 && owner != index)
                {
                    value += 0.3f * CountUnits(allProvinces[provinceIndex2]);
                }
            }
        }
        value -= CountUnits(allProvinces[indexProvince]);
        return value;
    }
}
