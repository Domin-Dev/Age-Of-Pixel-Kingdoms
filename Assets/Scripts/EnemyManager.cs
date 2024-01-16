
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private int index;
    private EnemySettings settings;
    private PathFinding pathFinding;

    List<int> provinces = new List<int>();
    List<float2> lastScan;
    List<float3> neighbors;
    List<float2> safeProvinces;

    bool done;

    float[] powerUnits;
    public void SetUp(PlayerStats playerStats)
    {
        this.pathFinding = new PathFinding();
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
        Research();
        if(UnityEngine.Random.Range(0,2) == 1) Building();

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
           int value = UnityEngine.Random.Range(0, provinces.Count);
            if(value < provinces.Count) Recruit(provinces[value], 3f);
          //  yield return new WaitUntil(() => done);
        }

        foreach (var item in neighbors)
        {
            //if (index == 1) Debug.Log(item);
            if (item.x != -1)
            {
               StartCoroutine(Attack((int)item.x));
               if (GameManager.Instance.provinces[(int)item.x].provinceOwnerIndex == index)
                {
                    float power = CheckPower((int)item.x);
                    if (power > 0.3f)
                    {
                        if (!Recruit((int)item.x, power)) 
                        GetStrongNeighbor((int)item.x, power);
                    }
               }
           //    yield return new WaitUntil(() => done);
            }
        }

        if(playerStats.movementPoints.value > 0)
        {
            safeProvinces = CheckSafeProvinces();
            for (int i = 0; i < safeProvinces.Count; i++)
            {
                float2 fromProvince = safeProvinces[i];
                if (safeProvinces[i].y < 0)
                {
                    ProvinceStats province = GameManager.Instance.provinces[(int)fromProvince.x];
                    float minPower = float.MinValue;
                    int provinceIndex = 0;
                    foreach (int item in province.neighbors)
                    {
                        float value = CheckPower(item);
                        if(value > minPower)
                        {
                            minPower = value;
                            provinceIndex = item;
                        }
                    }

                    if (minPower > 0) Move((int)fromProvince.x, provinceIndex, -fromProvince.y);
                    else
                    { 
                        int helpProvince = NeedHelp((int)fromProvince.x);
                        
                        if (helpProvince > 0)
                        {

                            int next = pathFinding.FindPath((int)fromProvince.x, helpProvince)[0];
                            Move((int)fromProvince.x, next, -fromProvince.y);
                        }
                    }
                }

                if(playerStats.movementPoints.value == 0)
                {
                    break;
                }
            }
        }


        GameManager.Instance.ready = true;
        yield return 0;
    }
    private bool Recruit(int provinceIndex, float battlePower)
    {
        //  done = false;
        int[] units = LoadUnits();
        while (battlePower > 0f)
        {
              int index = UnityEngine.Random.Range(0, units.Length);
              battlePower -= powerUnits[index];
              units[index]++;
        }
      //  GameManager.Instance.cameraController.SetProvince(GameManager.Instance.map.GetChild(provinceIndex), () => { done = true; });
        return GameManager.Instance.selectingProvinces.AIRecruitArray(provinceIndex, units, playerStats);
    }

    private int[] LoadUnits()
    {
        int value = 0;
        for (int i = 0; i < playerStats.units.Length; i++)
        {
            if (playerStats.units[i])
            {
                value = i;
            }
        }
        return new int[value + 1];
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
        int movementPoints = (int)GameManager.Instance.GetPlayerStats(index).movementPoints.value;

        if (movementPoints > 0 && from != to)
        {
            if (unitsFrom != null && province.unitsCounter > 0)
            {
                for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
                {
                    if (province.units.ContainsKey(i) && province.units[i] > 0) unitsFrom.Add(new float2(i, province.units[i]));
                }

                int[] units = new int[powerUnits.Length];
                while (battlePower > 0f)
                {
                    if (unitsFrom.Count > 0 && movementPoints > 0)
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
                        movementPoints --;
                    }
                    else
                    {
                        break;
                    }
                }
                GameManager.Instance.selectingProvinces.AIMoveArray(units, from, to);
            }
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
            bool hasNeighbors = false;


            for (int i = 0; i < allProvinces[item].neighbors.Count; i++)
            {
                int provinceIndex = allProvinces[item].neighbors[i];
                int owner = allProvinces[provinceIndex].provinceOwnerIndex;

                if (owner != index)
                {
                    hasNeighbors = true;
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
            if (hasNeighbors) value.y += 0.5f;
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
        bool hasNeighbors = false;

        for (int i = 0; i < allProvinces[indexProvince].neighbors.Count; i++)
        {
            int provinceIndex = allProvinces[indexProvince].neighbors[i];
            int owner = allProvinces[provinceIndex].provinceOwnerIndex;

            if (owner != index)
            {
                hasNeighbors = true;
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
        if (hasNeighbors) value += 0.5f;
        value -= CountUnits(allProvinces[indexProvince]);
        return value;
    }

    private void GetStrongNeighbor(int index, float needPower)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];
        foreach (int item in provinceStats.neighbors)
        {
            ProvinceStats province = GameManager.Instance.provinces[item];
            if(province.provinceOwnerIndex == index)
            {
                float value = CheckPower(item);
                if(value < -0.5) { 
                    Move(item, index, -value);
                    needPower = needPower + value;
                }
                
                if(needPower < 0)
                {
                    return;
                }
            }
        }

    }

    private List<float2> CheckSafeProvinces()
    {
        List<float2> list = new List<float2>();
        UpdateProvinces();
        foreach (int item in provinces)
        {
            ProvinceStats province = GameManager.Instance.provinces[item];
            bool isSafe = true;
            foreach (int neighbor in province.neighbors)
            {
                if (GameManager.Instance.provinces[neighbor].provinceOwnerIndex != playerStats.index)
                {
                    isSafe = false;
                    break;
                }
            }
            if (isSafe) list.Add(new float2(province.index,CheckPower(province.index)));
        }
        return list;
    }

    private int NeedHelp(int index)
    {

        float max = float.MinValue;
        int provinceIndex = -1;
        foreach (float2 item in lastScan)
        {
            if(item.y > max && index != (int)item.x)
            {              
                max = item.y;
                provinceIndex = (int)item.x;
            }
        }
        return provinceIndex;
    }

    private void Research()
    {
        float developmentPoints = playerStats.developmentPoints.value;
        int researchIndex = GetResearchPath();
        if(researchIndex >= 0) BuyResearch(researchIndex);
    }

    private int GetResearchPath()
    {
        List<int> paths = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (!playerStats.research[i, 5])
            {
                paths.Add(i);
                if (i == 0)
                {
                    paths.Add(i);
                    paths.Add(i);
                }
            }
        }


        int value = UnityEngine.Random.Range(0,paths.Count);
        if(paths.Count > 0) return paths[value];
        else return -1;
    }

    private void BuyResearch(int pathIndex)
    {
        int researchIndex = 0;
        for (int i = 0;i < 6;i++)
        {
            if (!playerStats.research[pathIndex, i])
            {
                researchIndex = i;
                break;
            }
        }
        Research research = GameAssets.Instance.research[pathIndex,researchIndex];
        if (playerStats.developmentPoints.CanAfford(research.price))
        {
            playerStats.research[pathIndex, researchIndex] = true;
            playerStats.developmentPoints.Subtract(research.price);
            BonusManager.AddPlayerBonus(playerStats, research.researchID);      

            string text = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    text += playerStats.research[i, j].ToString() + " | ";
                }
                text += "\n";
            }
         //   Debug.Log(text);
        }
    }

    private void Building()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < playerStats.buildingsPermit.Length; i++)
        {
            if (playerStats.buildingsPermit[i]) list.Add(i);
        }

        if (list.Count > 0)
        {
            int provinceIndex = GetSafeProvince();
            if (provinceIndex >= 0) Build(list[UnityEngine.Random.Range(0,list.Count)],provinceIndex);
        }
    }


    private void Build(int index, int provinceIndex)
    {
        BuildingStats buildingStats = GameAssets.Instance.buildingsStats[index];
        int price = buildingStats.price;
        int priceMP = buildingStats.movementPointsPrice;

        if (playerStats.cheaperBuilding) price = price - 50;
        if (playerStats.movementBuilding) priceMP = priceMP - 5;

        if (playerStats.coins.CanAfford(price))
        {
            if (playerStats.movementPoints.CanAfford(priceMP))
            {
                ProvinceStats provinceStats = GetProvince(provinceIndex);
                provinceStats.buildingIndex = index;
                Sounds.instance.PlaySound(1);
                BonusManager.SetBonus(provinceStats, buildingStats.bonusIndex);

                GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(priceMP);
                GameManager.Instance.humanPlayer.stats.coins.Subtract(price);
                
                Transform province = GameManager.Instance.map.GetChild(provinceIndex).transform;
                Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
                transform.position = province.position + new Vector3(0, 0.08f, 0);
                transform.parent = GameManager.Instance.buildings;
                transform.GetComponent<SpriteRenderer>().sprite = buildingStats.icon;
                transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }

    private int GetSafeProvince()
    {
        foreach (float2 item in lastScan)
        {
            if (item.y <= 0 && GetProvince((int) item.x).buildingIndex == -1) return (int)item.x;
        }

        foreach (float2 item in lastScan)
        {
            if ( GetProvince((int)item.x).buildingIndex == -1) return (int)item.x;
        }

        return -1;
    }
}
