
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
        LoadUnits(gameAssets.recruitUnitContentUI,false);
        LoadUnits(gameAssets.moveUnitContentUI,true);

        LoadBuildings();
        provinceStatsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("ProvinceStats"); });
        recruitmentWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("UnitsRecruitment"); });
        selectionNumberUnitsWindow.GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("SelectionNumberUnits"); });
        buildingsWindow.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { CloseUIWindow("Buildings"); });


        bottomBar.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Buildings", 0); });
        bottomBar.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("UnitsRecruitment", 0); });
        bottomBar.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { OpenUIWindow("Units", 0); });
    }
    private void LoadUnits(Transform ContentUI, bool isMove)
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
            }else
            {
                transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Move";
    //            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.SelectUnitToRecruit(id); });
            }
            index++;
        }
    }
    private void LoadBuildings()
    {
        int index = 0;
        foreach (BuildingStats item in gameAssets.buildingsStats)
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

    private void LoadProvinceUnits(int index)
    {
        int counterIndex = 0;
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];
        Transform parentCounters = GameAssets.Instance.moveUnitContentUI;

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

    public void LoadProvinceUnitCounters(int index)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];
        Transform ParentCounters = gameAssets.unitCounterContentUI.transform;

        if (provinceStats.units != null)
        {
           
            
            int counterIndex = 0;

            for (int j = 0; j < gameAssets.unitStats.Length; j++)
            {
                if (provinceStats.units.ContainsKey(j) && provinceStats.units[j] > 0)
                {
                    Transform transform;
                    if (ParentCounters.childCount > counterIndex)
                    {
                        transform = ParentCounters.GetChild(counterIndex).transform;
                        transform.gameObject.SetActive(true);
                    }
                    else
                    {
                        transform = Instantiate(gameAssets.unitCounterSlotUI, ParentCounters).transform;
                    }

                    transform.GetChild(0).GetComponent<Image>().sprite = gameAssets.unitStats[j].sprite;
                    transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = provinceStats.units[j].ToString();
                    counterIndex++;
                }

            }

            while(ParentCounters.childCount >= counterIndex + 1)
            {
                ParentCounters.GetChild(counterIndex).gameObject.SetActive(false);
                counterIndex++;
            }
        }else
        {
            for (int i = 0; i < ParentCounters.childCount; i++)
            {
                ParentCounters.GetChild(i).gameObject.SetActive(false);
            }
        }

    }
    public void OpenUIWindow(string name, int provinceIndex)
    {
        Transform transform = GetWindow(name);
        if (name == "ProvinceStats")
        {
            ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
            transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex.ToString();
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = provinceStats.population.ToString();
            LoadProvinceUnitCounters(provinceIndex);
        }
        else if(name == "Buildings" || name == "UnitsRecruitment" || name =="Units")
        {
            CloseUIWindow("UnitsRecruitment");
            CloseUIWindow("Buildings");
            CloseUIWindow("Units");
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
    public void CloseUIWindow(string name)
    {
        Transform transform = GetWindow(name);
        if(name == "ProvinceStats")
        {
            selectingProvinces.ClearSelectedProvince();
            CloseUIWindow("UnitsRecruitment");
            CloseUIWindow("SelectionNumberUnits");
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
    public void OpenNumberSelection()
    {
        selectionNumberUnitsWindow.gameObject.SetActive(true);
    }

}
