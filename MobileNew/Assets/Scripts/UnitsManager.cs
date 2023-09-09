
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UnitsManager : MonoBehaviour
{
    public List<Unit> path1 { get; private set; } = new List<Unit>();
    public List<Unit> path2 { get; private set; } = new List<Unit>();
    public List<Unit> path3 { get; private set; } = new List<Unit>();
    public List<Unit> path4 { get; private set; } = new List<Unit>();

    UnitStats[] unitStats;

    public Dictionary<int, int> yourUnits;
    public Dictionary<int, int> enemyUnits;

    public int yourUnitCount { get; private set; }
    public int enemyUnitCount { get; private set; }

    public int yourPoints { get; private set; }
    public int enemyPoints { get; private set; }


    int SelectedUnitIndex = -1;

    private Transform paths;

    public static UnitsManager Instance { private set; get; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        unitStats = GameAssets.Instance.unitStats;
        GameManager.Instance.GetUnits(out yourUnits,out enemyUnits);
        paths = GameObject.FindWithTag("Paths").transform;

        GameAssets.Instance.battleBar.value = 0.5f;

        if (yourUnits != null)
        {
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
                if (yourUnits.ContainsKey(i) && yourUnits[i] > 0)
                {
                    yourUnitCount += yourUnits[i];
                    Transform transform = Instantiate(GameAssets.Instance.BattleConter, GameAssets.Instance.BattleUnits.transform).transform;
                    transform.name = i.ToString();
                    transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GameAssets.Instance.unitStats[i].sprite;
                    transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = yourUnits[i].ToString();
                    int index = i;
                    transform.GetComponent<Button>().onClick.AddListener(() => { Instance.SetUnitIndex(index); });
                }
            }
        }
        if(enemyUnits != null)
        {
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
                if (enemyUnits.ContainsKey(i) && enemyUnits[i] > 0)
                {
                    enemyUnitCount += enemyUnits[i];
                }
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && SelectedUnitIndex != -1)
        {
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (RaycastHit2D raycastHit in Physics2D.RaycastAll(worldMousePosition, Vector3.zero))
            {
                if(raycastHit.collider.CompareTag("Path"))
                {
                  if(CanSpawn(raycastHit.collider.transform,false) && yourUnits[SelectedUnitIndex]> 0)
                     CreateUnit(SelectedUnitIndex, raycastHit.collider.transform);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {

            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (RaycastHit2D raycastHit in Physics2D.RaycastAll(worldMousePosition, Vector3.zero))
            {
                if (raycastHit.collider.CompareTag("Path"))
                {
                    if (CanSpawn(raycastHit.collider.transform, true))
                        EnemyCreateUnit(0, raycastHit.collider.transform);
                }
            }
        }
    }
    public void SetUnitIndex(int index)
    {
        if (yourUnits[index] > 0)
        {
            for (int i = 0; i < GameAssets.Instance.BattleUnits.childCount; i++)
            {
                if (GameAssets.Instance.BattleUnits.GetChild(i).name == index.ToString())
                {
                    GameAssets.Instance.BattleUnits.GetChild(i).GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;
                }
                else if (GameAssets.Instance.BattleUnits.GetChild(i).name == SelectedUnitIndex.ToString())
                {
                    GameAssets.Instance.BattleUnits.GetChild(i).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
                }
            }
            SelectedUnitIndex = index;
        }
    }
    private void ClearSelectedUnit()
    {
        for (int i = 0; i < GameAssets.Instance.BattleUnits.childCount; i++)
        {
            if (GameAssets.Instance.BattleUnits.GetChild(i).name == SelectedUnitIndex.ToString())
            {
                GameAssets.Instance.BattleUnits.GetChild(i).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
            }
        }
        SelectedUnitIndex = -1;
    }
    private void UpdateUnitsUI(int index)
    {
        for (int i = 0; i < GameAssets.Instance.BattleUnits.childCount; i++)
        {
            if (GameAssets.Instance.BattleUnits.GetChild(i).name == index.ToString())
            {
                GameAssets.Instance.BattleUnits.GetChild(i).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = yourUnits[index].ToString();
                break;
            }
        }
    }

    private bool CanSpawn(Transform pathTransform, bool isEnemy)
    {
        int childIndex;
        if (isEnemy) childIndex = 1;
        else childIndex = 0;

        int index = int.Parse(pathTransform.name);
        List<Unit> path = GetPath(index);
        foreach (Unit unit in path)
        {
            if (Vector2.Distance(unit.transform.position, pathTransform.GetChild(childIndex).position) < 0.8f)
            {
                return false;
            }
        }
        return true;
    }
    private void CreateUnit(int unitindex,Transform pathTransform)
    {
        yourUnits[unitindex]--;
        yourUnitCount--;
        UpdateUnitsUI(unitindex);

        int path = int.Parse(pathTransform.name);
        List<Unit> units = GetPath(path);

        Unit unit = Instantiate(unitStats[unitindex].unit, pathTransform.GetChild(0).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
        unit.SetUp (unitindex,path,true, pathTransform.GetChild(1).position.x + 0.3f, (bool isDead) => { if (!isDead) UnitCame(false,unitindex); units.Remove(unit); }, (unit) => { return CheckPath(unit);});
        units.Add(unit);
    }
    private void EnemyCreateUnit(int unitindex, Transform pathTransform)
    {
        if (CanSpawn(pathTransform, true) && enemyUnits[unitindex] > 0)
        {
            enemyUnits[unitindex]--;
            enemyUnitCount--;

            int path = int.Parse(pathTransform.name);
            List<Unit> units = GetPath(path);

            Unit unit = Instantiate(unitStats[unitindex].unit, pathTransform.GetChild(1).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
            unit.SetUp(unitindex, path, false, pathTransform.GetChild(0).position.x, (bool isDead) => { if (!isDead) UnitCame(true, unitindex); units.Remove(unit); }, (unit) => { return CheckPath(unit); });
            units.Add(unit);
        }
    }

    private Unit CheckPath(Unit selectedUnit)
    {
        List<Unit> units = GetPath(selectedUnit.pathIndex);
        Unit friendlyUnit = null;
        again:
        {
            foreach (Unit unit in units)
            {

                if (unit != selectedUnit)
                {
                    float selectedUnitPosX = selectedUnit.positionX;
                    float unitPosX = unit.transform.position.x;


                    float distance = Mathf.Abs(selectedUnitPosX - unitPosX);


                    if (unit.unitIsFriendly != selectedUnit.unitIsFriendly)
                    {
                        if (distance <= 1f && distance <= selectedUnit.range) return unit;
                        else if(friendlyUnit != null && distance <= selectedUnit.range) return unit;
                    }
                    else if(friendlyUnit == null) 
                    {
                        if (distance <= 1f && selectedUnitPosX * selectedUnit.multiplier < unitPosX * unit.multiplier)
                        {
                            friendlyUnit = unit;
                            goto again;
                        }
                    }
                }

            }
        }

        if (friendlyUnit != null) return friendlyUnit;
        return null;
    }
    private List<Unit> GetPath(int index)
    {
        switch (index)
        {
            case 1: return path1;
            case 2: return path2;
            case 3: return path3;
            case 4: return path4;
        }
        return path1;
    }
    public void UnitCame(bool isAI,int index)
    {
        if (!isAI)
        {
            yourUnitCount++;
            yourUnits[index]++;
            UpdateUnitsUI(index);
            yourPoints++;
        }else
        {
            enemyPoints++;
            enemyUnitCount++;
          //  enemyUnits[index]++;
        }
        UpdateBattleBar();
    }
    private void UpdateBattleBar()
    {

       float value = 0.5f;
       if(yourPoints + enemyPoints > 0)  value = (float)yourPoints / (float)(yourPoints + enemyPoints);
       GameAssets.Instance.battleBar.value = value;
        Debug.Log(yourPoints + " " + enemyPoints + "  " + (float)value);
    }

}
