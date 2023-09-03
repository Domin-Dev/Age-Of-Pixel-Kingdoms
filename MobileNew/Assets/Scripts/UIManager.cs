
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(this);
        }
    }

    [SerializeField] private Transform provinceStatsWindow;


    [SerializeField] private Transform recruitmentWindow;
    [SerializeField] private Transform selectionNumberUnitsWindow;
    [SerializeField] private Transform buildingsWindow;
    [SerializeField] private Transform unitsWindow;


    [SerializeField] private Transform topBar;  
    [SerializeField] private Transform bottomBar;  

    private SelectingProvinces selectingProvinces;
    private GameAssets gameAssets;

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



        bottomBar.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Buildings", 0); });
        bottomBar.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("UnitsRecruitment", 0); });
        bottomBar.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.HighlightNeighbors(); });
        ManagerUI(false);
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
                transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "recruit\n" + item.price + "<sprite index=21>";
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
                    transform.GetChild(i).gameObject.SetActive(true);
                }    
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Debug.Log("I");
                    if (buildingIndex == i)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
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
            ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
            transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex.ToString();
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = provinceStats.population.ToString();
            LoadProvinceUnitCounters(provinceIndex,gameAssets.unitCounterContentUI.transform,false);
        }
        else if(name == "Buildings" || name == "UnitsRecruitment" || name =="Units")
        {
            selectingProvinces.ResetNeighbors();
            CloseUIWindow("UnitsRecruitment");
            CloseUIWindow("Buildings");
            CloseUIWindow("Units");
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

    public void LoadUnitsMove(int provinceIndex1,int  provinceIndex2,bool isUpdate)
    {   
        if(!isUpdate)OpenUIWindow("Units", 0);
        LoadProvinceUnitCounters(provinceIndex1, gameAssets.moveUnitContentUI1,true);
        unitsWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex1.ToString();
       

        LoadProvinceUnitCounters(provinceIndex2, gameAssets.moveUnitContentUI2,true);
        unitsWindow.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex2.ToString();
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

}
