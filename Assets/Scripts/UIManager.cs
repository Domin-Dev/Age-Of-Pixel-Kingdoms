
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    [SerializeField] private Transform provinceStatsWindow;



    [SerializeField] private Transform win;
    [SerializeField] public Transform background;
    [SerializeField] private Transform recruitmentWindow;
    [SerializeField] private Transform selectionNumberUnitsWindow;
    [SerializeField] private Transform buildingsWindow;
    [SerializeField] private Transform unitsWindow;
    [SerializeField] private Transform battleWindow;
    [SerializeField] private Transform developmentWindow;
    [SerializeField] private Transform managementWindow;
    [SerializeField] private Transform researchWindow;
    [SerializeField] private Transform pauseWindow;
    [SerializeField] private Transform spellsWindow;

    private Transform groups;

    [SerializeField] private Transform nextTurn;
    [SerializeField] private TextMeshProUGUI turnConter;
    [SerializeField] private Transform details;

    [SerializeField] public TextMeshProUGUI debugText;

    [SerializeField] private Transform topBar;  
    [SerializeField] private Transform bottomBar;  

    private SelectingProvinces selectingProvinces;
    private GameAssets gameAssets;
    private Transform map;

    private TextMeshProUGUI coinCounter;
    private TextMeshProUGUI warriorsCounter;
    private TextMeshProUGUI developmentPointsCounter;
    private TextMeshProUGUI movementPointsCounter;

    private int selectedSpell;

    public void SetUp()
    {
        map = GameObject.FindWithTag("GameMap").transform;
        gameAssets = GameAssets.Instance;
        selectingProvinces = Camera.main.GetComponent<SelectingProvinces>();  
        LoadUnits(gameAssets.recruitUnitContentUI);


        LoadBuildings(-1);

        win.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene(0); });
        spellsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Spells"); Sounds.instance.PlaySound(5); });
        pauseWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Pause"); Sounds.instance.PlaySound(5); });
        provinceStatsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("ProvinceStats"); Sounds.instance.PlaySound(5); });
        recruitmentWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("UnitsRecruitment"); Sounds.instance.PlaySound(5); });
        selectionNumberUnitsWindow.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("SelectionNumberUnits"); Sounds.instance.PlaySound(5); });
        buildingsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Buildings"); Sounds.instance.PlaySound(5); });
        unitsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Units"); Sounds.instance.PlaySound(5); });
        battleWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Battle"); Sounds.instance.PlaySound(5); });
        developmentWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Development"); Sounds.instance.PlaySound(5); });
        groups = developmentWindow.GetChild(1).GetChild(0).GetChild(0).transform;

        researchWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Research"); Sounds.instance.PlaySound(5); });
        managementWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Management"); Sounds.instance.PlaySound(5); });
        pauseWindow.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.Save();  SceneManager.LoadScene(0); });


        bottomBar.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Buildings", 0); Sounds.instance.PlaySound(5); });
        bottomBar.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("UnitsRecruitment", 0); Sounds.instance.PlaySound(5); });
        bottomBar.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.HighlightNeighbors(); Sounds.instance.PlaySound(5); });
        bottomBar.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Development", 0); Sounds.instance.PlaySound(5); });
        bottomBar.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Management", 0); OpenManagement(); Sounds.instance.PlaySound(5);});
        bottomBar.GetChild(6).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Spells", 0); Sounds.instance.PlaySound(5);});
        bottomBar.GetChild(7).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Pause", 0); Sounds.instance.PlaySound(5);}) ; 

        managementWindow.GetChild(2).GetComponent<Slider>().onValueChanged.AddListener((float value) => {GameManager.Instance.humanPlayer.stats.texesIndex = (int)value; UpdateTaxesText(); Sounds.instance.PlaySound(5); });
        managementWindow.GetChild(3).GetComponent<Slider>().onValueChanged.AddListener((float value) => {GameManager.Instance.humanPlayer.stats.researchIndex = (int)value; UpdateResearchText(); Sounds.instance.PlaySound(5); });


        details.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Details"); Sounds.instance.PlaySound(5); });

        ManagerUI(false);

        turnConter.text = "Turn:0";
        nextTurn.GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.NextTurn(); Sounds.instance.PlaySound(5); });

        coinCounter = topBar.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        warriorsCounter = topBar.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        developmentPointsCounter = topBar.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        movementPointsCounter = topBar.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        topBar.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OpenStatsDetails(GameManager.Instance.humanPlayer.stats.coins); Sounds.instance.PlaySound(5); });
        topBar.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OpenStatsDetails(GameManager.Instance.humanPlayer.stats.developmentPoints); Sounds.instance.PlaySound(5); });
        topBar.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { OpenStatsDetails(GameManager.Instance.humanPlayer.stats.warriors); Sounds.instance.PlaySound(5); });
        topBar.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { OpenStatsDetails(GameManager.Instance.humanPlayer.stats.movementPoints); Sounds.instance.PlaySound(5); });

        UpdateCounters();

        LoadResearch();
        LoadSpells();
        OpenManagement();


    }
    public void ManagerUI(bool open)
    {
        for (int i = 0; i < 3; i++)
        {
            bottomBar.GetChild(i).gameObject.SetActive(open);
        }
    }

    public void UpdateTurnCounter()
    {
        turnConter.text = "Turn:" + GameManager.Instance.turn;
    }
    private void LoadUnits(Transform contentUI)
    {
        if (contentUI.childCount == 0)
        {
            int index = 0;
            foreach (UnitStats item in gameAssets.unitStats)
            {
                Transform transform = Instantiate(gameAssets.unitSlotUI, contentUI).transform;
                transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.unitName;
                transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = item.sprite;
                Transform transformStats = transform.GetChild(0).GetChild(2);
                transformStats.GetChild(0).GetComponent<TextMeshProUGUI>().text = "<sprite index=16>" + item.lifePoints.ToString();
                transformStats.GetChild(1).GetComponent<TextMeshProUGUI>().text = "<sprite index=14>" + item.damage.ToString();
                transformStats.GetChild(2).GetComponent<TextMeshProUGUI>().text = "<sprite index=17>" + item.speed.ToString();
                transformStats.GetChild(3).GetComponent<TextMeshProUGUI>().text = "<sprite index=15>" + item.range.ToString();
                transformStats.GetChild(4).GetComponent<TextMeshProUGUI>().text = "<sprite index=18>" + item.rateOfFire.ToString();
                transformStats.GetChild(5).GetComponent<TextMeshProUGUI>().text = "<sprite index=19>" + item.turnCost.ToString();

                int id = index;


                int price = item.price;
                int priceMP = item.movementPointsPrice;
                if (GameManager.Instance.humanPlayer.stats.cheaperRecruitment) price = price - 5;
                if (GameManager.Instance.humanPlayer.stats.movementRecruitment) priceMP = priceMP - 1;

                transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "recruit\n" + price + " <sprite index=21>\n" + priceMP + " <sprite index=23> 1 <sprite index=1>";
                transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "recruit\n" + price + " <sprite index=21>\n" + priceMP + " <sprite index=23> 1 <sprite index=1>";
                transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); selectingProvinces.SelectUnitToRecruit(id); });
                index++;
            }
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.humanPlayer.stats.units.Length; i++)
            {
                bool value = GameManager.Instance.humanPlayer.stats.units[i];
                contentUI.GetChild(i).gameObject.SetActive(value);
                if(value)
                {
                    Transform transform = contentUI.GetChild(i);
                    UnitStats item = gameAssets.unitStats[i];
                    int price = item.price;
                    int priceMP = item.movementPointsPrice;
                    if (GameManager.Instance.humanPlayer.stats.cheaperRecruitment) price = price - 5;
                    if (GameManager.Instance.humanPlayer.stats.movementRecruitment) priceMP = priceMP - 1;

                    transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "recruit\n" + price + " <sprite index=21>\n" + priceMP + " <sprite index=23> 1 <sprite index=1>";
                }

            }
        }
    }
    public void LoadBuildings(int provinceIndex)
    {
        PlayerStats player = GameManager.Instance.humanPlayer.stats;
        if (provinceIndex == -1)
        {
            if (gameAssets.buildingsContentUI.childCount == 0)
            {
                int index = 0;
                foreach (BuildingStats item in gameAssets.buildingsStats)
                {
                    if (item.canBuild)
                    {
                        Transform transform = Instantiate(gameAssets.buildingSlotUI, gameAssets.buildingsContentUI).transform;
                        transform.name = index.ToString();
                        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.name.Substring(1);
                        transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = item.icon;
                        int price = item.price;
                        int priceMP = item.movementPointsPrice;

                        if (GameManager.Instance.humanPlayer.stats.cheaperBuilding) price = price - 50;
                        if (GameManager.Instance.humanPlayer.stats.movementBuilding) priceMP = priceMP - 5;

                        transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Build\n" + price + " <sprite index=21>\n" + priceMP + " <sprite index=23>";
                        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = StringToIcons(item.description);

                        int id = index;
                        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.Build(id); });
                    }
                    index++;
                }
            }
        }
        else
        {
            int buildingIndex = GameManager.Instance.provinces[provinceIndex].buildingIndex;

            Transform transform = gameAssets.buildingsContentUI;

            if (buildingIndex == -1)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (player.CanBuild(int.Parse(transform.GetChild(i).name)))
                    {
                        BuildingStats buildingStats = gameAssets.buildingsStats[int.Parse(transform.GetChild(i).name)];
                        transform.GetChild(i).gameObject.SetActive(true);
                        int price = buildingStats.price;
                        int priceMP = buildingStats.movementPointsPrice; 

                        if (GameManager.Instance.humanPlayer.stats.cheaperBuilding) price = price - 50;
                        if (GameManager.Instance.humanPlayer.stats.movementBuilding) priceMP = priceMP - 5;

                        transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Build\n" + price +" <sprite index=21>\n" + priceMP + " <sprite index=23>";
                        transform.GetChild(i).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                        int id = int.Parse(transform.GetChild(i).name);
                        transform.GetChild(i).GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.Build(id); });
                    }else
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (buildingIndex == int.Parse(transform.GetChild(i).name))
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                        transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "destroy";
                        transform.GetChild(i).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
                        transform.GetChild(i).GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.Destroy(); });
                    }
                    else
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

        }
    }
    private void LoadProvinceUnits(int index)
    {
        int counterIndex = 0;
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];
        Transform parentCounters = GameAssets.Instance.moveUnitContentUI1;

        for (int j = 0; j < gameAssets.unitStats.Length; j++)
        {
            if (provinceStats.units.ContainsKey(j) && provinceStats.units[j] > 0)
            {
                Transform transform;
                if (parentCounters.childCount > counterIndex)
                {
                    transform = parentCounters.GetChild(counterIndex).transform;
                    transform.gameObject.SetActive(true);
                }
                else
                {
                    transform = Instantiate(gameAssets.unitCounterSlotUI, parentCounters).transform;
                }

                transform.GetChild(0).GetComponent<Image>().sprite = gameAssets.unitStats[j].sprite;
                transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = provinceStats.units[j].ToString();
                counterIndex++;
            }

        }

        while(parentCounters.childCount >= counterIndex + 1)
        {
            parentCounters.GetChild(counterIndex).gameObject.SetActive(false);
            counterIndex++;
        }
    }
    private int GetNumberProvince(Transform transform)
    {
        if (transform == gameAssets.moveUnitContentUI1) return 1;
        if (transform == gameAssets.moveUnitContentUI2) return 2;
        return 0;
    }
    public void LoadProvinceUnitCounters(int index,Transform parentCounters,bool isMove)
    {
        ProvinceStats provinceStats;
        if (index >= 0) provinceStats = GameManager.Instance.provinces[index];
        else
        {
            for (int i = 0; i < parentCounters.childCount; i++)
            {
                parentCounters.GetChild(i).gameObject.SetActive(false);
            }
            return;
        }


        if (provinceStats.units != null)
        {
            int counterIndex = 0;
            for (int j = 0; j < gameAssets.unitStats.Length; j++)
            {
                if (provinceStats.units.ContainsKey(j) && provinceStats.units[j] > 0)
                {
                    Transform transform;
                    if (parentCounters.childCount > counterIndex)
                    {
                        transform = parentCounters.GetChild(counterIndex).transform;
                        transform.gameObject.SetActive(true);
                        transform.name = j.ToString();
                    }
                    else
                    {
                        transform = Instantiate(gameAssets.unitCounterSlotUI, parentCounters).transform;
                        transform.name = j.ToString();
                    }

                    transform.GetComponent<Image>().sprite = gameAssets.blueTexture;
                    transform.GetChild(0).GetComponent<Image>().sprite = gameAssets.unitStats[j].sprite;
                    int unitIndex = j;
                    if (isMove)
                    {
                        int provinceNumber = GetNumberProvince(parentCounters);
                        transform.GetComponent<Button>().onClick.RemoveAllListeners();
                        transform.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            selectingProvinces.SelectUnitToMove(unitIndex, provinceNumber);
                        });
                    }
                    transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = provinceStats.units[j].ToString();
                    counterIndex++;
                }

            }

            while(parentCounters.childCount >= counterIndex + 1)
            {
                parentCounters.GetChild(counterIndex).name = "-1";
                parentCounters.GetChild(counterIndex).gameObject.SetActive(false);
                counterIndex++;
            }
        }else
        {
            for (int i = 0; i < parentCounters.childCount; i++)
            {
                parentCounters.GetChild(i).gameObject.SetActive(false);
            }
        }

    }
    private bool IsSea(Transform transform)
    {
       if(transform != null) return GameManager.Instance.provinces[int.Parse(transform.name)].isSea;
       else return false;
    }
    public void OpenUIWindow(string name, int provinceIndex)
    {
        Transform transform = GetWindow(name);
        if (name == "ProvinceStats")
        {
            LoadProvinceStats(transform,provinceIndex);
        }
        else if(name == "Buildings" || name == "UnitsRecruitment" || name =="Units" || name == "Battle" ||
            name == "Development" || name == "Management" || name == "Details"|| name == "Research" || name == "Pause"|| name == "Spells")
        {
            if (name == "Development" || name == "Management" || name == "Spells") CloseUIWindow("ProvinceStats");
            selectingProvinces.ResetNeighbors();
            CloseUIWindow("UnitsRecruitment");
            CloseUIWindow("Buildings");
            CloseUIWindow("Units");
            CloseUIWindow("Battle");
            CloseUIWindow("Details");
            CloseUIWindow("Management");
            CloseUIWindow("Development");
            CloseUIWindow("Research");
            CloseUIWindow("Pause");
            CloseUIWindow("Spells");
        }

        if(name == "Buildings")
        {
            if(IsSea(selectingProvinces.selectedProvince)) return;
            LoadBuildings(int.Parse(selectingProvinces.selectedProvince.name));
        }
        if(name == "Development")
        {
            UpdateResearch();
        }
        if(name == "UnitsRecruitment")
        {
            LoadUnits(gameAssets.recruitUnitContentUI); 
        }


        if (transform != null)
        { 
            transform.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Wrong window name!");
        }
    }
    private void LoadProvinceStats(Transform transform, int provinceIndex)
    {

        if (GameManager.Instance.CanBeShow(provinceIndex))
        {
            ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
            transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex.ToString() + " " + CountUnits(provinceIndex);
            TextMeshProUGUI textMeshProUGUI = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            if (provinceStats.provinceOwnerIndex != -1)
            {
                textMeshProUGUI.text = "Player " + provinceStats.provinceOwnerIndex.ToString() + "  <color=red>" + provinceStats.lifePoints.ToString() + "</color> <sprite index=16>";
                textMeshProUGUI.color = GameManager.Instance.GetPlayerColor(provinceStats.provinceOwnerIndex);
            }
            else
            {
                textMeshProUGUI.text = "No owner" + "  <color=red> " + provinceStats.lifePoints.ToString() + "</color> <sprite index=16>";
                textMeshProUGUI.color = Color.grey;
            }
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = provinceStats.population.ToString();
            Button button = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { OpenStatsDetails(provinceStats.population); Sounds.instance.PlaySound(5); });

            if (provinceStats.buildingIndex == -1)
            {
                transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = gameAssets.noBuilding;
                transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "no building";
                transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                BuildingStats buildingStats = GameAssets.Instance.buildingsStats[provinceStats.buildingIndex];
                transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buildingStats.icon;
                transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = buildingStats.name.Substring(1);
                transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = StringToIcons(buildingStats.description);
            }

            //    transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=green>+" + Math.Round(provinceStats.developmentPoints.CountIncome(),2).ToString() + "<sprite index=2/>";
            //   transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=green>+" + Math.Round(provinceStats.coins.CountIncome(),2).ToString() + "<sprite index=2/>";
            //   transform.GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + provinceStats.movementPoints.value.ToString();
            //    transform.GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + provinceStats.warriors.ToString();
            LoadProvinceUnitCounters(provinceIndex, gameAssets.unitCounterContentUI.transform, false);
        }else
        {
            transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex.ToString();
            TextMeshProUGUI textMeshProUGUI = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = "???" + "  <color=red>???</color> <sprite index=16>";
            textMeshProUGUI.color = Color.grey;
            
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "???";
            Button button = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { Sounds.instance.PlaySound(4); });

            transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = gameAssets.noBuilding;
            transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "???";
            transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "???";
   

            LoadProvinceUnitCounters(-1, gameAssets.unitCounterContentUI.transform, false);
        }
    }
    public void LoadUnitsMove(int provinceIndex1, int provinceIndex2, bool isUpdate)
    {
        if (!isUpdate) OpenUIWindow("Units", 0);
        LoadProvinceUnitCounters(provinceIndex1, gameAssets.moveUnitContentUI1, true);

        int x = GameManager.Instance.provinces[provinceIndex1].unitsCounter;

        selectingProvinces.moveAll1.GetComponentInChildren<TextMeshProUGUI>().text = x.ToString() + Icons.GetIcon("MovementPoint");
        selectingProvinces.moveHalf1.GetComponentInChildren<TextMeshProUGUI>().text = (x/2).ToString() + Icons.GetIcon("MovementPoint");
        unitsWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex1.ToString();

        LoadProvinceUnitCounters(provinceIndex2, gameAssets.moveUnitContentUI2,true);

        x = GameManager.Instance.provinces[provinceIndex2].unitsCounter;

        selectingProvinces.moveAll2.GetComponentInChildren<TextMeshProUGUI>().text = x.ToString() + Icons.GetIcon("MovementPoint");
        selectingProvinces.moveHalf2.GetComponentInChildren<TextMeshProUGUI>().text = (x / 2).ToString() + Icons.GetIcon("MovementPoint");
        unitsWindow.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex2.ToString();
    }
    public void LoadUnitsAttack(int yourProvinceIndex, int enemyProvinceIndex,Action action, bool youAttack)
    {
        OpenUIWindow("Battle", 0);
       

        if (action != null)
        {
            battleWindow.GetChild(0).GetChild(0).gameObject.SetActive(false);
            background.gameObject.SetActive(true);
            GameManager.Instance.readyToNextTurn = false;
            GameManager.Instance.StopNewTurn();
        }
        else battleWindow.GetChild(0).GetChild(0).gameObject.SetActive(true);

        battleWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + yourProvinceIndex.ToString();
        battleWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color=#"+ GameManager.Instance.humanPlayer.playerColor.ToHexString() +">" + GameManager.Instance.humanPlayer.playerName;
        LoadProvinceUnitCounters(yourProvinceIndex, gameAssets.AttackUnitContentUI1, false);




        int aggressorProvinceIndex;
        int defenderProvinceIndex;

        if(youAttack)
        {
            aggressorProvinceIndex = yourProvinceIndex;
            defenderProvinceIndex = enemyProvinceIndex;
        }else
        {
            aggressorProvinceIndex = enemyProvinceIndex;
            defenderProvinceIndex = yourProvinceIndex;
        }

        Button button = battleWindow.GetChild(2).GetChild(1).GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {if(action != null) GameManager.Instance.readyToNextTurn = true;  GameManager.Instance.Battle(yourProvinceIndex,enemyProvinceIndex,youAttack); });

        button =  battleWindow.GetChild(2).GetChild(0).GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { if (action != null) { GameManager.Instance.readyToNextTurn = true; action(); } GameManager.Instance.selectingProvinces.AutoBattle(true,aggressorProvinceIndex, defenderProvinceIndex);});


        int index = GameManager.Instance.provinces[enemyProvinceIndex].provinceOwnerIndex;
        battleWindow.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + enemyProvinceIndex.ToString();
        if(index != -1) battleWindow.GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "<color=#" + GameManager.Instance.botsList[index - 1].playerColor.ToHexString() + ">" + GameManager.Instance.botsList[index -1].playerName;
        LoadProvinceUnitCounters(enemyProvinceIndex, gameAssets.AttackUnitContentUI2, false);
    }
    public void CloseUIWindow(string name)
    {
        Transform transform = GetWindow(name);
        if(name == "ProvinceStats")
        {
            selectingProvinces.ResetNeighbors();
            selectingProvinces.ClearSelectedProvince();
            CloseUIWindow("UnitsRecruitment");
            CloseUIWindow("SelectionNumberUnits");
            CloseUIWindow("Units");
            CloseUIWindow("Buildings");
            CloseUIWindow("Details");
            CloseUIWindow("Development");
            CloseUIWindow("Management");
            CloseUIWindow("Research");
            CloseUIWindow("Pause");
        }
        else if(name == "UnitsRecruitment")
        {
            CloseUIWindow("SelectionNumberUnits");
            selectingProvinces.ResetUnits();
        } 
        else if(name == "SelectionNumberUnits")
        {
            selectingProvinces.ResetUnits();
        }
        else if(name == "Units")
        {
            CloseUIWindow("SelectionNumberUnits");
            selectingProvinces.ResetUnits();
        }else if(name == "Battle")
        {
            background.gameObject.SetActive(false);
        }


        if (transform != null)
        {
            transform.gameObject.SetActive(false);
        }else
        {
            Debug.Log("Wrong window name!");
        }
    }
    public Transform GetWindow(string name)
    {
        switch (name)
        {
            case "ProvinceStats": return provinceStatsWindow;
            case "UnitsRecruitment": return recruitmentWindow;
            case "SelectionNumberUnits": return selectionNumberUnitsWindow;
            case "Buildings": return buildingsWindow;
            case "Units": return unitsWindow;
            case "Battle": return battleWindow;
            case "Details": return details;
            case "Development": return developmentWindow;
            case "Management": return managementWindow;
            case "Research": return researchWindow;
            case "Pause": return pauseWindow;
            case "Spells": return spellsWindow;
            case "Win": return win;
        }
        return null;
    }
    public Transform GetSelectionNumberUnitsWindowWindow()
    {
        return selectionNumberUnitsWindow;
    }
    public Transform GetUnitsWindow()
    {
        return unitsWindow;
    }
    public void OpenNumberSelection()
    {
        selectionNumberUnitsWindow.gameObject.SetActive(true);
    }
    public void OpenTurnDetails(string text)
    {
        details.gameObject.SetActive(true);
        details.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Turn:" + GameManager.Instance.turn.ToString();        
        details.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
        UpdateCounters();
    }
    public void UpdateCounters()
    {
        coinCounter.text = GameManager.Instance.humanPlayer.stats.coins.ToString();
        warriorsCounter.text = GameManager.Instance.humanPlayer.stats.warriors.ToString();
        developmentPointsCounter.text = GameManager.Instance.humanPlayer.stats.developmentPoints.ToString();
        movementPointsCounter.text = GameManager.Instance.humanPlayer.stats.movementPoints.ToString();
    }
    private void OpenStatsDetails(Statistic statistic)
    {
        OpenUIWindow("Details",0);
        details.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Details";
        details.GetChild(1).GetComponent<TextMeshProUGUI>().text = statistic.GetDetails();
    }
    private void OpenManagement()
    {
        string details = "";
        details += "Turn:" + GameManager.Instance.turn + Icons.GetIcon("Turn")+ "\n";
        details += "Population: " + GameManager.Instance.humanPlayer.stats.GetPopulation() + Icons.GetIcon("Population") + "\n";

        float value = GameManager.Instance.humanPlayer.stats.GetNumberOfProvinces();
        details += "Provinces: " + value;
        int index = GameManager.Instance.humanPlayer.stats.texesIndex;
        managementWindow.GetChild(2).GetComponent<Slider>().value = index;
        if (GameManager.Instance.humanPlayer.stats.taxManagement)
        {
            managementWindow.GetChild(2).GetComponent<Slider>().interactable = true;
            managementWindow.GetChild(2).GetChild(1).gameObject.SetActive(false);
            managementWindow.GetChild(2).GetChild(1).GetComponent<Image>().sprite = null;
        }
        else
        {
            managementWindow.GetChild(2).GetComponent<Slider>().interactable = false;
            managementWindow.GetChild(2).GetChild(1).gameObject.SetActive(true);
            managementWindow.GetChild(2).GetChild(1).GetComponent<Image>().sprite = gameAssets.locked;
        }

        if (GameManager.Instance.humanPlayer.stats.researchManagement)
        {
            managementWindow.GetChild(3).GetComponent<Slider>().interactable = true;
            managementWindow.GetChild(3).GetChild(1).gameObject.SetActive(false);
            managementWindow.GetChild(3).GetChild(1).GetComponent<Image>().sprite = null;
        }
        else
        {
            managementWindow.GetChild(3).GetComponent<Slider>().interactable = false;
            managementWindow.GetChild(3).GetChild(1).gameObject.SetActive(true);
            managementWindow.GetChild(3).GetChild(1).GetComponent<Image>().sprite = gameAssets.locked;
        }



        UpdateTaxesText();
        UpdateResearchText();

        index = GameManager.Instance.humanPlayer.stats.researchIndex;
        managementWindow.GetChild(3).GetComponent<Slider>().value = index;
        managementWindow.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = details; 
    }
    private void UpdateTaxesText()
    {
            int index = GameManager.Instance.humanPlayer.stats.texesIndex;
            GameManager.Instance.GetValuesByTaxesIndex(index, out float coins, out float people);
            GameManager.Instance.humanPlayer.stats.ChangeCoinsMultiplier(coins);
            GameManager.Instance.humanPlayer.stats.ChangePopulationMultiplier(people);
            int population = (int)GameManager.Instance.humanPlayer.stats.GetPopulation();
            managementWindow.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Taxes: ( " + GetColorString((float)Math.Round(population * coins, 2)) + Icons.GetIcon("Coin") + "  " + GetColorString((float)Math.Round(people * population, 2)) + Icons.GetIcon("Population") + " )" + Icons.GetIcon("Turn");
            UpdateCounters();
    }
    private void UpdateResearchText()
    {
        int index = GameManager.Instance.humanPlayer.stats.researchIndex;
        GameManager.Instance.GetValuesByResearchIndex(index, out float coins,out float development);
        GameManager.Instance.humanPlayer.stats.ChangeDevelopmentMultiplier(development);
        GameManager.Instance.humanPlayer.stats.ChangeDevelopmentCoinsMultiplier(coins);

        int population = (int)GameManager.Instance.humanPlayer.stats.GetPopulation();
        managementWindow.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Research: ( " + GetColorString((float)Math.Round(population * coins,2)) + Icons.GetIcon("Coin") + "  " + GetColorString((float)Math.Round(development * population,2)) + Icons.GetIcon("DevelopmentPoint") +" )" + Icons.GetIcon("Turn");
        UpdateCounters();
    }
    private string GetColorString(float value)
    {
        if(value >= 0)
        {
            return "<color=green>+" + value + "</color>";
        }
        else
        {
            return "<color=red>" + value + "</color>";
        }
    }       
    private void LoadResearch()
    {
        int length = gameAssets.research.GetLength(1);
        if(groups.GetChild(0).childCount == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Research research = gameAssets.research[i, j];
                    GameObject obj = Instantiate(gameAssets.researchUI, groups.GetChild(i).transform);
                    int index = i * 100 + j;
                    obj.GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); OpenResearch(index); });
                    obj.transform.GetChild(0).GetComponent<Image>().sprite = research.image;
                    obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = research.name;
                    obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = research.price.ToString() + Icons.GetIcon("DevelopmentPoint");
                    obj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = StringToIcons(research.description);
                }
            }
            UpdateResearch();
        }
    }


    public void LoadSpells()
    {
        int length = gameAssets.spells.Length;
        Transform parent = spellsWindow.GetChild(1).GetChild(0).GetChild(0);
        selectedSpell = 0;


        for (int i = 0; i < 3; i++)
        {
            Transform spellTransform = spellsWindow.GetChild(2).GetChild(i);
          
            int index = i;
            if (GameManager.Instance.humanPlayer.stats.selectedSpells[i] >= 0)
            {
                Spell spell = gameAssets.spells[GameManager.Instance.humanPlayer.stats.selectedSpells[i]];
                if (selectedSpell == index)spellTransform.transform.GetComponent<Image>().sprite = gameAssets.blueTexture;
                else spellTransform.transform.GetComponent<Image>().sprite = gameAssets.brownTexture;
                
                spellTransform.GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); SelectSpellSlot(index);});
                spellTransform.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = spell.spellName;
                spellTransform.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = spell.description;
                spellTransform.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spell.icon;
            } 
            else if (GameManager.Instance.humanPlayer.stats.selectedSpells[i] == -1)
            {
                if (selectedSpell == index) spellTransform.transform.GetComponent<Image>().sprite = gameAssets.blueTexture;
                else spellTransform.transform.GetComponent<Image>().sprite = gameAssets.brownTexture;

                spellTransform.GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); SelectSpellSlot(index);});
                spellTransform.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "no selected spell";
                spellTransform.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                spellTransform.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = gameAssets.empty;
            }
            else
            {
                spellTransform.transform.GetComponent<Image>().sprite = gameAssets.blackTexture;
                spellTransform.GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(4); });
                spellTransform.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "locked";
                spellTransform.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                spellTransform.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = gameAssets.locked;
            }
        }



        if (parent.childCount == 0)
        {
            for (int i = 0; i < length; i++)
            {
                Spell spell = gameAssets.spells[i];
                GameObject obj = Instantiate(gameAssets.spell, parent);
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = spell.spellName + "  " + spell.price + Icons.GetIcon("DevelopmentPoint");
                obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = spell.description;
                obj.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spell.icon;
                
                int index = i;
                if (GameManager.Instance.humanPlayer.stats.spells[i])
                {
                    obj.GetComponent<Image>().sprite = gameAssets.brownTexture;
                    obj.GetComponent<Button>().onClick.AddListener(() => { SelectSpell(index); });
                }
                else
                {
                    obj.GetComponent<Image>().sprite = gameAssets.blackTexture;
                    obj.GetComponent<Button>().onClick.AddListener(() => { OpenSpellBuying(index); });
                }

            }
        }else
        {
            for (int i = 0; i < length; i++)
            {

                Transform obj = parent.GetChild(i);
                int index = i;
                if (GameManager.Instance.humanPlayer.stats.spells[i])
                {
                    obj.GetComponent<Image>().sprite = gameAssets.brownTexture;
                    obj.GetComponent<Button>().onClick.RemoveAllListeners();
                    obj.GetComponent<Button>().onClick.AddListener(() => { SelectSpell(index); });
                }
                else
                {
                    obj.GetComponent<Image>().sprite = gameAssets.blackTexture;
                    obj.GetComponent<Button>().onClick.RemoveAllListeners();
                    obj.GetComponent<Button>().onClick.AddListener(() => { OpenSpellBuying(index); });
                }

            }
        }
    }

    private void SelectSpell(int index)
    {
        int[] array = GameManager.Instance.humanPlayer.stats.selectedSpells;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == index)
            {
                Sounds.instance.PlaySound(4);
                return;
            }
        }


        Spell spell = gameAssets.spells[index];
        Transform spellTransform = spellsWindow.GetChild(2).GetChild(selectedSpell);

        spellTransform.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = spell.spellName;
        spellTransform.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = spell.description;
        spellTransform.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spell.icon;
        GameManager.Instance.humanPlayer.stats.selectedSpells[selectedSpell] = index;
        Sounds.instance.PlaySound(5);
    }

    private void OpenSpellBuying(int index)
    {
       if (GameManager.Instance.humanPlayer.stats.developmentPoints.CanAfford(gameAssets.spells[index].price))
       { 
            OpenUIWindow("Research", 0);
            Spell spell = gameAssets.spells[index];
            researchWindow.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = spell.spellName;
            researchWindow.GetChild(1).GetComponent<TextMeshProUGUI>().text = StringToIcons(spell.description);
            researchWindow.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Research\n" + spell.price.ToString() + Icons.GetIcon("DevelopmentPoint");
            researchWindow.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            researchWindow.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                BuySpell(index);
            });
       }
       else
       {
            Sounds.instance.PlaySound(4);
       }
    }
    private void SelectSpellSlot(int number)
    {
        if(selectedSpell != -1)
        {
            spellsWindow.GetChild(2).GetChild(selectedSpell).GetComponent<Image>().sprite = gameAssets.brownTexture;
        }
        selectedSpell = number;
        spellsWindow.GetChild(2).GetChild(selectedSpell).GetComponent<Image>().sprite = gameAssets.blueTexture;
    }
    private void UpdateResearch()
    {
        bool[,] list = GameManager.Instance.humanPlayer.stats.research;
        int length = gameAssets.research.GetLength(1);
        for (int i = 0; i < 4; i++)
        {
            bool isFirst = false; 
            for (int j = 0; j < length; j++)
            {
                if (list[i,j] == true)
                {
                    groups.GetChild(i).GetChild(j + 1).GetComponent<Image>().sprite = gameAssets.blueTexture;
                }
                else
                {
                    if(isFirst == false)
                    {
                        isFirst = true;
                        groups.GetChild(i).GetChild(j + 1).GetComponent<Image>().sprite = gameAssets.brownTexture;
                    }
                    else
                    {
                        groups.GetChild(i).GetChild(j + 1).GetComponent<Image>().sprite = gameAssets.blackTexture;
                    }
                }

            }
        }
    }
    private void OpenResearch(int index)
    {
        if (CanBuyResearch(index))
        {
            OpenUIWindow("Research", 0);
            Research research = gameAssets.research[index / 100, index % 100];
            researchWindow.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = research.name;
            researchWindow.GetChild(1).GetComponent<TextMeshProUGUI>().text = StringToIcons(research.description);
            researchWindow.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Research\n" + research.price.ToString() + Icons.GetIcon("DevelopmentPoint");
            researchWindow.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            researchWindow.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                BuyResearch(index);
            });
        }
        else
        {
            Sounds.instance.PlaySound(4);
        }
    }
    private void BuySpell(int index)
    {
        Spell spell = gameAssets.spells[index];
        if (GameManager.Instance.humanPlayer.stats.developmentPoints.CanAfford(spell.price))
        {
            Transform obj = spellsWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(index);
            obj.GetComponent<Image>().sprite = gameAssets.brownTexture;
            obj.GetComponent<Button>().onClick.RemoveAllListeners();
            obj.GetComponent<Button>().onClick.AddListener(() => { SelectSpell(index); });

            Sounds.instance.PlaySound(3);
            GameManager.Instance.humanPlayer.stats.developmentPoints.Subtract(spell.price);
            GameManager.Instance.humanPlayer.stats.spells[index] = true;
            SelectSpell(index);
            CloseUIWindow("Research"); 
        }
        else
        {
            Alert.Instance.OpenAlert("No Development Points");
        }
    }
    private bool CanBuyResearch(int index)
    {
        if(index % 100 == 0)
        {
            return !GameManager.Instance.humanPlayer.stats.research[index / 100, index % 100];
        }
        else
        {
            return !GameManager.Instance.humanPlayer.stats.research[index / 100, index % 100] && GameManager.Instance.humanPlayer.stats.research[index / 100, index % 100 -1]; ;
        }
    }
    private void BuyResearch(int index)
    {
        Research research = gameAssets.research[index / 100, index % 100];

        if (GameManager.Instance.humanPlayer.stats.developmentPoints.CanAfford(research.price))
        {
            if (CanBuyResearch(index))
            {
                Sounds.instance.PlaySound(3);
                GameManager.Instance.humanPlayer.stats.research[index / 100, index % 100] = true;
                GameManager.Instance.humanPlayer.stats.developmentPoints.Subtract(research.price);            
                BonusManager.AddPlayerBonus(GameManager.Instance.humanPlayer.stats, GameAssets.Instance.research[index / 100, index % 100].researchID);
                CloseUIWindow("Research");
            }
        }
        else
        {
            Alert.Instance.OpenAlert("No Development Points");
        }
    }
    private string StringToIcons(string str)
    {
        bool isIcon = false;
        bool isColor = false;
        string icon = "";
        string stringToReturn = "";
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (c == '#')
            {
                stringToReturn += "\n";
                continue;
            }
            if(c == '+')
            {
                stringToReturn += "<color=green>+";
                isColor = true;
                continue;
            }
            if(c == '-')
            {
                stringToReturn += "<color=red>-";
                isColor = true;
                continue;
            }
            if (c == ' ' && isColor) { 

                stringToReturn += "</color>";
                isColor = false;
                continue;
            }
            if (isIcon)
            {
                if (c == ']')
                {
                    isIcon = false;
                    stringToReturn += Icons.GetIcon(icon);
                    icon = "";
                }
                else icon += c;
            }
            else
            {
                if (c == '[') isIcon = true;
                else stringToReturn += c;
            }
        }
        return stringToReturn;
    }

    public void OpenChest(int index)
    {
        Debug.Log("chest!!!");
        Sounds.instance.PlaySound(17);
        OpenUIWindow("Research", 0);
        GameManager.Instance.ClearChest(index);
        researchWindow.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Chest";
        researchWindow.GetChild(1).GetComponent<TextMeshProUGUI>().text = Chest.OpenChest(GameManager.Instance.humanPlayer.stats);
        researchWindow.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "collect";
        researchWindow.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
        UpdateCounters();
        researchWindow.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            CloseUIWindow("Research");
        });
    }

    private float CountUnits(int index)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];
        float value = 0;
        if (provinceStats.units != null)
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
}
