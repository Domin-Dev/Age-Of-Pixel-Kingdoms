
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnitsManager : MonoBehaviour
{
    bool isEnd = false;
    public List<Unit> path1 { get; private set; } = new List<Unit>();
    public List<Unit> path2 { get; private set; } = new List<Unit>();
    public List<Unit> path3 { get; private set; } = new List<Unit>();
    public List<Unit> path4 { get; private set; } = new List<Unit>();

    UnitStats[] unitStats;

    public Dictionary<int, int> yourUnits;
    public Dictionary<int, int> enemyUnits;
    public int yourUnitCount { get; private set; }
    public int enemyUnitCount { get; private set; }

    private int yourHP;
    private int maxYourHP;


    private int enemyHP;
    private int maxEnemyHP;

    Color yourColor;
    Color enemyColor;


    int SelectedUnitIndex = -1;

    private Transform paths;
    public static UnitsManager Instance { private set; get; }

    private int startYourUnits;
    private int startEnemyUnits;

    private BattleEnemy battleEnemy;
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
        GameManager.Instance.GetProvinceHP(out maxYourHP, out maxEnemyHP);
        yourHP = maxYourHP;
        enemyHP = maxEnemyHP;

        UpdateBattleBars();
        isEnd = false;

        unitStats = GameAssets.Instance.unitStats;
        GameManager.Instance.GetUnits(out yourUnits,out enemyUnits);
        paths = GameObject.FindWithTag("Paths").transform;

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
        startEnemyUnits = enemyUnitCount;
        startYourUnits = yourUnitCount;
        battleEnemy = GetComponent<BattleEnemy>();

    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            yourColor = GameManager.Instance.GetPlayerColor(0);
            enemyColor = GameManager.Instance.GetPlayerColor(GameManager.Instance.GetEnemyIndex());
        }
    }
    private void Update()
    {
        if (!isEnd)
        {
            if (Input.GetMouseButtonDown(0) && SelectedUnitIndex != -1)
            {
                Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                foreach (RaycastHit2D raycastHit in Physics2D.RaycastAll(worldMousePosition, Vector3.zero))
                {
                    if (raycastHit.collider.CompareTag("Path"))
                    {
                        if (CanSpawn(raycastHit.collider.transform, false) && yourUnits[SelectedUnitIndex] > 0)
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
                //        if (CanSpawn(raycastHit.collider.transform, true))
                //            EnemyCreateUnit(0, raycastHit.collider.transform);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) EnemyCreateUnit(0, 1,true);
            if (Input.GetKeyDown(KeyCode.Alpha2)) EnemyCreateUnit(1, 1, true);
            if (Input.GetKeyDown(KeyCode.Alpha3)) EnemyCreateUnit(2, 1, true);
            if (Input.GetKeyDown(KeyCode.Alpha4)) EnemyCreateUnit(3, 1,true);
            if (Input.GetKeyDown(KeyCode.Alpha5)) EnemyCreateUnit(4, 1, true);
            if (Input.GetKeyDown(KeyCode.Alpha6)) EnemyCreateUnit(5, 1, true);
        }
    }
    public void SetUnitIndex(int index)
    {
        if (yourUnits[index] > 0)
        {
            Sounds.instance.PlaySound(5);
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
    private void CreateUnit(int unitindex, Transform pathTransform)
    {
        if (Time.timeScale > 0)
        {
            Sounds.instance.PlaySound(6);
            yourUnits[unitindex]--;
            yourUnitCount--;
            UpdateUnitsUI(unitindex);
            int path = int.Parse(pathTransform.name);
            List<Unit> units = GetPath(path);

            Unit unit = Instantiate(unitStats[unitindex].unit, pathTransform.GetChild(0).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
            unit.SetUp(unitindex, path, true, pathTransform.GetChild(1).position.x + 0.3f, (bool isDead) => { units.Remove(unit); if (!isDead) UnitCame(false, unitindex); CheckUnits(); }, (unit) => { return CheckPath(unit); });
            units.Add(unit);

            SetUnitColor(yourColor, unit.GetComponent<SpriteRenderer>());
            battleEnemy.CheckPaths();
        }
    }
    public bool EnemyCreateUnit(int unitindex, int pathIndex, bool debug)
    {
        Transform pathTransform = paths.GetChild(pathIndex - 1);
        if ((CanSpawn(pathTransform, true) && enemyUnits[unitindex] > 0)|| debug)
        {
            if(!debug)
            {
                enemyUnits[unitindex]--;
                enemyUnitCount--;
            }

            int path = int.Parse(pathTransform.name);
            List<Unit> units = GetPath(path);

            Unit unit = Instantiate(unitStats[unitindex].unit, pathTransform.GetChild(1).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
            unit.SetUp(unitindex, path, false, pathTransform.GetChild(0).position.x, (bool isDead) => { units.Remove(unit);if (!isDead) UnitCame(true, unitindex); CheckUnits(); battleEnemy.CheckPaths(); }, (unit) => { return CheckPath(unit); });
            units.Add(unit);
            SetUnitColor(enemyColor, unit.GetComponent<SpriteRenderer>());
            return true;
        }
        return false;
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
    public List<Unit> GetPath(int index)
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
            enemyHP--;
        }else
        {
            yourHP--;
            enemyUnitCount++;
            enemyUnits[index]++;
            battleEnemy.typesArray[(int)GameAssets.Instance.unitStats[index].unitType]++;
        }
        UpdateBattleBars();
    }
    private void UpdateBattleBars()
    {
        GameAssets.Instance.battleEnemyBar.GetComponentInChildren<Slider>().value = (float)((float)enemyHP / (float)maxEnemyHP);
        GameAssets.Instance.battleEnemyBar.GetComponentInChildren<TextMeshProUGUI>().text = enemyHP.ToString() + "/" + maxEnemyHP.ToString();

        GameAssets.Instance.battleYourBar.GetComponentInChildren<Slider>().value = (float)((float)yourHP /(float)maxYourHP);
        GameAssets.Instance.battleYourBar.GetComponentInChildren<TextMeshProUGUI>().text = yourHP.ToString() + "/" + maxYourHP.ToString();
        if (yourHP <= 0 || enemyHP <= 0)
        {
           BattleEnd(yourHP <= 0);
        }
    }
    private void BattleEnd(bool winnerIsEnemy)
    {
        Time.timeScale = 0f;
        isEnd = true;
        CountUnits();

        Transform infoWindow = GameAssets.Instance.battleInfo;
        infoWindow.gameObject.SetActive(true);
        GameManager.Instance.SetUnitsConters(yourUnitCount, enemyUnitCount);

        if (winnerIsEnemy)
        {
            infoWindow.GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "You lost";
            GameManager.Instance.SetBattleResult(false);
        }
        else
        {
            infoWindow.GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "you Won";
            GameManager.Instance.SetBattleResult(true);
        }

        infoWindow.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = yourUnitCount.ToString() + " <sprite index=0>\n" +
            (startYourUnits - yourUnitCount).ToString() + " <sprite index=20>";
        infoWindow.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = enemyUnitCount.ToString() + " <sprite index=0>\n" +
            (startEnemyUnits - enemyUnitCount).ToString() + " <sprite index=20>";
        infoWindow.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); SceneManager.LoadScene(2); });
    }
    private void CountUnits()
    {
        for (int i = 1; i < 5; i++)
        {
            List<Unit> units = GetPath(i);
            foreach (Unit unit in units)
            {
                if(unit.unitIsFriendly)
                {
                    yourUnitCount++;
                    yourUnits[unit.unitIndex]++;
                }
                else
                {
                    enemyUnitCount++;
                    enemyUnits[unit.unitIndex]++;
                }
            }
        }
    }
    private void CheckUnits()
    {
        int your = yourUnitCount;
        int enemy = enemyUnitCount;
        if (your <= 0 || enemy <= 0)
        {
            for (int i = 1; i < 5; i++)
            {
                List<Unit> units = GetPath(i);
                foreach (Unit unit in units)
                {
                    if (unit.unitIsFriendly)
                    {
                        your++;
                    }
                    else
                    {
                        enemy++;
                    }
                }
            }
            
            if(enemy > 0 &&  your <= 0)
            {
                BattleEnd(true);
            }
            else if(your > 0 && enemy <= 0)
            {
                BattleEnd(false);

            }else if(enemy == 0 && your == 0)
            {
                if (enemyHP != yourHP) BattleEnd(enemyHP >= yourHP);
                else BattleEnd(GameManager.Instance.youAttack);
            }
            
        }   
    }
    private void SetUnitColor(Color color,SpriteRenderer spriteRenderer)
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_Color", color);
        materialPropertyBlock.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}
