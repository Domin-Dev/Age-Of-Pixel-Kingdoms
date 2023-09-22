
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            map = GameObject.FindWithTag("GameMap").transform;
        }
        else
        {
            Destroy(this);
        }
    }
    [SerializeField] private Transform provinceStatsWindow;

    [SerializeField] private Transform recruitmentWindow;
    [SerializeField] private Transform selectionNumberUnitsWindow;
    [SerializeField] private Transform buildingsWindow;
    [SerializeField] private Transform unitsWindow;
    [SerializeField] private Transform battleWindow;

    [SerializeField] private Transform nextTurn;
    [SerializeField] private TextMeshProUGUI turnConter;
    [SerializeField] private Transform details;


    [SerializeField] private Transform topBar;  
    [SerializeField] private Transform bottomBar;  

    private SelectingProvinces selectingProvinces;
    private GameAssets gameAssets;
    private Transform map;

    private TextMeshProUGUI coinCounter;
    private TextMeshProUGUI warriorsCounter;
    private TextMeshProUGUI developmentPointsCounter;
    private TextMeshProUGUI movementPointsCounter;


    private void Start()
    {
        gameAssets = GameAssets.Instance;
        selectingProvinces = Camera.main.GetComponent<SelectingProvinces>();  
        LoadUnits(gameAssets.recruitUnitContentUI,false,0);
        // LoadUnits(gameAssets.moveUnitContentUI1,true);

        LoadBuildings(-1);

        provinceStatsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("ProvinceStats"); });
        recruitmentWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("UnitsRecruitment"); });
        selectionNumberUnitsWindow.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("SelectionNumberUnits"); });
        buildingsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Buildings"); });
        unitsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Units"); });
        battleWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Battle"); });

        bottomBar.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Buildings", 0); });
        bottomBar.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("UnitsRecruitment", 0); });
        bottomBar.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.HighlightNeighbors(); });

        details.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Details");});

        ManagerUI(false);

        turnConter.text = "Turn:0";
        nextTurn.GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.NextTurn(turnConter); });

        coinCounter = topBar.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        warriorsCounter = topBar.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        developmentPointsCounter = topBar.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        movementPointsCounter = topBar.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        UpdateCounters();
    }
    public void ManagerUI(bool open)
    {
        for (int i = 0; i < 3; i++)
        {
            bottomBar.GetChild(i).gameObject.SetActive(open);
        }
    }
    private void LoadUnits(Transform ContentUI, bool isMove, int provinceNumber)
    {
        int index = 0;
        foreach (UnitStats item in gameAssets.unitStats)
        {
            Transform transform = Instantiate(gameAssets.unitSlotUI, ContentUI).transform;
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.name; 
            transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = item.sprite;
           
            Transform transformStats = transform.GetChild(0).GetChild(2);
            transformStats.GetChild(0).GetComponent<TextMeshProUGUI>().text = "<sprite index=20>" + item.lifePoints.ToString();
            transformStats.GetChild(1).GetComponent<TextMeshProUGUI>().text = "<sprite index=18>" + item.damage.ToString();
            transformStats.GetChild(2).GetComponent<TextMeshProUGUI>().text = "<sprite index=17>" + item.speed.ToString();
            transformStats.GetChild(3).GetComponent<TextMeshProUGUI>().text = "<sprite index=19>" + item.range.ToString();
            transformStats.GetChild(4).GetComponent<TextMeshProUGUI>().text = "<sprite index=22>" + item.rateOfFire.ToString();
            transformStats.GetChild(5).GetComponent<TextMeshProUGUI>().text = "<sprite index=23>" + item.turnCost.ToString();

            int id = index;
            if (!isMove)
            {
                transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "recruit\n" + item.price + " <sprite index=21>\n" + item.movementPointsPrice +" <sprite index=23> 1 <sprite index=1>";

                transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.SelectUnitToRecruit(id); });
            }
            index++;
        }
    }
    public void LoadBuildings(int provinceIndex)
    {
        if (provinceIndex == -1)
        {
            int index = 0;
            foreach (BuildingStats item in gameAssets.buildingsStats)
            {
                if (item.canBuild)
                {
                    Transform transform = Instantiate(gameAssets.buildingSlotUI, gameAssets.buildingsContentUI).transform;
                    transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.name;
                    transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = item.icon;
                    transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Build\n" + item.price + "<sprite index=21>";
                    transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = item.description;

                    int id = index;
                    transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.Build(id); });
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
                    BuildingStats buildingStats = gameAssets.buildingsStats[i];
                    transform.GetChild(i).gameObject.SetActive(true);
                    transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Build\n" + buildingStats.price + "<sprite index=21>";
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (buildingIndex == i)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                        transform.GetChild(i).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "destroy";

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
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];

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
        else if(name == "Buildings" || name == "UnitsRecruitment" || name =="Units" || name == "Battle")
        {
            selectingProvinces.ResetNeighbors();
            CloseUIWindow("UnitsRecruitment");
            CloseUIWindow("Buildings");
            CloseUIWindow("Units");
            CloseUIWindow("Battle");
            CloseUIWindow("Details");
        }

        if(name == "Buildings")
        {
            if(IsSea(selectingProvinces.selectedProvince)) return;
            LoadBuildings(int.Parse(selectingProvinces.selectedProvince.name));
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

        ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
        transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex.ToString();
        TextMeshProUGUI textMeshProUGUI = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        if (provinceStats.provinceOwnerIndex != -1)
        {
            textMeshProUGUI.text = "Player " + provinceStats.provinceOwnerIndex.ToString() + "  <color=red>" + provinceStats.lifePoints.ToString() + "</color> <sprite index=16>";
            textMeshProUGUI.color = Color.red;
        }
        else
        {
            textMeshProUGUI.text = "No owner" + "  <color=red> " + provinceStats.lifePoints.ToString() + "</color> <sprite index=16>";
            textMeshProUGUI.color = Color.grey;
        }

        transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = provinceStats.population.ToString() + "<sprite index=2/>";
        transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=green>+" + provinceStats.developmentPoints.value.ToString() + "<sprite index=2/>";
        transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=green>+" + ((int)provinceStats.population.value * 0.1f).ToString() + "<sprite index=2/>";
        transform.GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + provinceStats.movementPoints.value.ToString();
        transform.GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + provinceStats.warriors.ToString();


        LoadProvinceUnitCounters(provinceIndex, gameAssets.unitCounterContentUI.transform, false);
    }
    public void LoadUnitsMove(int provinceIndex1,int  provinceIndex2,bool isUpdate)
    {   
        if(!isUpdate)OpenUIWindow("Units", 0);
        LoadProvinceUnitCounters(provinceIndex1, gameAssets.moveUnitContentUI1,true);
        unitsWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex1.ToString();
       

        LoadProvinceUnitCounters(provinceIndex2, gameAssets.moveUnitContentUI2,true);
        unitsWindow.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex2.ToString();
    }
    public void LoadUnitsAttack(int yourProvinceIndex, int enemyProvinceIndex)
    {
        OpenUIWindow("Battle", 0);
        battleWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + yourProvinceIndex.ToString();
        LoadProvinceUnitCounters(yourProvinceIndex, gameAssets.AttackUnitContentUI1, false);

        Button button = battleWindow.GetChild(2).GetChild(0).GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { GameManager.Instance.Battle(yourProvinceIndex,enemyProvinceIndex,true); });

        button =  battleWindow.GetChild(2).GetChild(1).GetComponentInChildren<Button>(); 

        battleWindow.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + enemyProvinceIndex.ToString();
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
        }


        if (transform != null)
        {
            transform.gameObject.SetActive(false);
        }else
        {
            Debug.Log("Wrong window name!");
        }
    }
    private Transform GetWindow(string name)
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
    }
    public void UpdateCounters()
    { 
        coinCounter.text = GameManager.Instance.humanPlayer.coins.ToString();
        warriorsCounter.text = GameManager.Instance.humanPlayer.warriors.ToString();
        developmentPointsCounter.text = GameManager.Instance.humanPlayer.developmentPoints.value.ToString();
        movementPointsCounter.text = GameManager.Instance.humanPlayer.movementPoints.ToString();
    }

}
