
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ProvinceStats[] provinces;
    public int numberOfProvinces;
    public List<Player> playerList;

    public PlayerStats humanPlayer;


    public Transform map;
    public Transform buildings;
    public SelectingProvinces selectingProvinces;

    private int yourProvinceIndex;
    private int enemyProvinceIndex;
    public bool youAttack { private set; get; }

    public int turn { private set; get; } = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            map = GameObject.FindGameObjectWithTag("GameMap").transform;
            buildings = GameObject.FindGameObjectWithTag("Buildings").transform;

            selectingProvinces = FindObjectOfType<SelectingProvinces>();

            ProvinceStats[] array = Resources.Load<MapStats>("Maps/World").provinces;
            provinces = new ProvinceStats[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                provinces[i] = new ProvinceStats();
                provinces[i].CopyData(array[i]);
                ProvinceStats provinceStats = provinces[i];
                if (provinceStats.provinceOwnerIndex != -1)
                {
                    selectingProvinces.ChangeProvinceColor(map.GetChild(i).GetComponent<SpriteRenderer>(), Color.red);
                }
                if (provinceStats.buildingIndex != -1)
                {
                    BonusManager.SetBonus(provinceStats, provinceStats.buildingIndex);
                    Transform province = map.GetChild(provinceStats.index).transform;
                    Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
                    transform.position = province.position + new Vector3(0, 0.08f, 0);
                    transform.parent = buildings;
                    transform.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.buildingsStats[provinceStats.buildingIndex].icon;
                    transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
                }
            }
            numberOfProvinces = Resources.Load<MapStats>("Maps/World").numberOfProvinces;

            humanPlayer = new PlayerStats(10000);
            humanPlayer.movementPoints.UpdateLimit();
            humanPlayer.warriors.UpdateLimit();
            humanPlayer.movementPoints.value = humanPlayer.movementPoints.limit;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
        Time.timeScale = 1.0f;
    }


    private void LoadPlayers()
    {
        playerList.Add(new Player("xd", true, Color.cyan));
        playerList.Add(new Player("xd", true, Color.cyan));
        playerList.Add(new Player("xd", true, Color.cyan));
    }
    public void UpdateUnitCounter(int index)
    {
        selectingProvinces.UpdateUnitNumber(map.GetChild(index).transform);
    }
    public void Battle(int yourProvinceIndex, int enemyProvinceIndex, bool youAttack)
    {
        this.youAttack = youAttack;
        this.yourProvinceIndex = yourProvinceIndex;
        this.enemyProvinceIndex = enemyProvinceIndex;
        SceneManager.LoadScene(1);
    }
    public void GetUnits(out Dictionary<int, int> yourUnits, out Dictionary<int, int> enemyUnits)
    {
        yourUnits = provinces[yourProvinceIndex].units;
        enemyUnits = provinces[enemyProvinceIndex].units;
    }
    public void GetProvinceHP(out int yourProvinceHP, out int enemyProvinceHP)
    {
        if (youAttack)
        {
            yourProvinceHP = 10;
            enemyProvinceHP = (int)provinces[enemyProvinceIndex].lifePoints.value;
        }
        else
        {
            yourProvinceHP = (int)provinces[yourProvinceIndex].lifePoints.value;
            enemyProvinceHP = 10;
        }

    }

    public void SetUnitsConters(int your, int enemy)
    {
        GameManager.Instance.humanPlayer.warriors.Subtract(provinces[yourProvinceIndex].unitsCounter - your);
        provinces[yourProvinceIndex].unitsCounter = your;
        provinces[enemyProvinceIndex].unitsCounter = enemy;
    }
    public void SetBattleResult(bool isWin)
    {
        if (isWin)
        {
            ProvinceStats provinceStats = provinces[enemyProvinceIndex];
            provinceStats.SetNewOwner(0);
            provinceStats.units = new Dictionary<int, int>();
            provinceStats.unitsCounter = 0;
        }
        else
        {

        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            UpdateMap();
        }
    }
    public void UpdateMap()
    {
        map = GameObject.FindGameObjectWithTag("GameMap").transform;
        buildings = GameObject.FindGameObjectWithTag("Buildings").transform;
        selectingProvinces = FindObjectOfType<SelectingProvinces>();
        humanPlayer.movementPoints.limit = 30;

        humanPlayer.movementPoints.UpdateLimit();
        humanPlayer.warriors.UpdateLimit();

        for (int i = 0; i < provinces.Length; i++)
        {
            ProvinceStats provinceStats = provinces[i];
            if (provinceStats.provinceOwnerIndex != -1)
            {
                selectingProvinces.ChangeProvinceColor(map.GetChild(i).GetComponent<SpriteRenderer>(), Color.red);
            }

            if (provinceStats.buildingIndex != -1)
            {
                Transform province = map.GetChild(provinceStats.index).transform;
                Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
                transform.position = province.position + new Vector3(0, 0.08f, 0);
                transform.parent = buildings;
                transform.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.buildingsStats[provinceStats.buildingIndex].icon;
                transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
            UpdateUnitCounter(i);
        }
    }

    public void NextTurn(TextMeshProUGUI text)
    {
        turn++;

        humanPlayer.movementPoints.Set(humanPlayer.movementPoints.limit);

        float startDevelopmentPoints = (float)Math.Round(humanPlayer.developmentPoints.value, 2);
        float developmentPointsIncome = (float)Math.Round(humanPlayer.developmentPoints.NextTurn(),2);

        float startCoins = (int)humanPlayer.coins.value;
        float coinsIncome = humanPlayer.coins.NextTurn();

        float startPopulation = humanPlayer.GetPopulation();
        float populationIncome = 0;


        text.text = "Turn:" + turn;
        UIManager.Instance.CloseUIWindow("ProvinceStats");

        for (int i = 0; i < provinces.Length; i++)
        {
            float value = provinces[i].population.NextTurn();
          //  provinces[i].developmentPoints.NextTurn(); 
            if (provinces[i].provinceOwnerIndex == 0) populationIncome += value;
        }

        string stats = startCoins + " <sprite index=21/>   ";
        if (coinsIncome >= 0) stats += "<color=green>+"+ coinsIncome +"</color>";
        else stats += "<color=red>" + coinsIncome + "</color>";

        stats += "\n";

        stats +=  startPopulation + " <sprite index=1/>   ";
        if (populationIncome >= 0) stats += "<color=green>+" + populationIncome + "</color>";
        else stats += "<color=red>" + populationIncome + "</color>";

        stats += "\n";

        stats += startDevelopmentPoints + " <sprite index=22/>   ";
        if (developmentPointsIncome >= 0) stats += "<color=green>+" + developmentPointsIncome + "</color>";
        else stats += "<color=red>" + developmentPointsIncome + "</color>";

        UIManager.Instance.OpenTurnDetails(stats);
    }


    /// 0,1,2,3,4
    public void GetValuesByTaxesIndex(int index, out float coinsIncome,out float peopleIncome)
    {
        coinsIncome = 0;
        peopleIncome = 0;
        switch (index)
        {
            case 0:  
                coinsIncome = -0.05f;
                peopleIncome = 1f;
                break; 
            case 1:  
                coinsIncome = 0.00f;
                peopleIncome = 0.75f;
                break; 
            case 2:  
                coinsIncome = 0.05f;
                peopleIncome = 0.5f;
                break;
            case 3:  
                coinsIncome = 0.1f;
                peopleIncome = 0;
                break;           
            case 4:  
                coinsIncome = 0.15f;
                peopleIncome = -0.25f;
                break; 
        }
    }
    public void GetValuesByResearchIndex(int index, out float coinsIncome,out float developmentIncome)
    {
        coinsIncome = 0;
        developmentIncome = 0;
        switch (index)
        {
            case 0:  
                coinsIncome = -0.05f;
                developmentIncome = 1f;
                break; 
            case 1:  
                coinsIncome = 0.00f;
                developmentIncome = 0.75f;
                break; 
            case 2:  
                coinsIncome = 0.05f;
                developmentIncome = 0.5f;
                break;
            case 3:  
                coinsIncome = 0.1f;
                developmentIncome = 0;
                break;           
            case 4:  
                coinsIncome = 0.15f;
                developmentIncome = -0.25f;
                break; 
        }
    }
}

