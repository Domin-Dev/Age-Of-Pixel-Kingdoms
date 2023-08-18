
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Progress;
using System;
using UnityEditor.Experimental.GraphView;

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


    private SelectingProvinces selectingProvinces;
    private GameAssets gameAssets;

    private void Start()
    {
        gameAssets = GameAssets.Instance;
        selectingProvinces = Camera.main.GetComponent<SelectingProvinces>();  
        LoadUnits();
    }


    private void LoadUnits()
    {
        int index = 0;
        foreach (UnitStats item in gameAssets.unitStats)
        {
            Transform transform = Instantiate(gameAssets.unitSlotUI, gameAssets.contentUI).transform;
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = item.name; 
            transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = item.sprite;
           
            Transform transformStats = transform.GetChild(0).GetChild(2);
            transformStats.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.lifePoints.ToString();
            transformStats.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.damage.ToString();
            transformStats.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.speed.ToString();
            transformStats.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.range.ToString();
            transformStats.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.rateOfFire.ToString();
            transformStats.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.turnCost.ToString();

            transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "recruit\n" + item.price;
            int id = index;
            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { selectingProvinces.SelectUnitToRecruit(id); });
            index++;
        }
    }
    public void LoadProvinceUnitCounters(int index)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[index];
        Transform ParentCounters = gameAssets.contentUnitsCounterUI.transform;

        if (provinceStats.units != null)
        {
           
            
            int counterIndex = 0;

            for (int j = 0; j < gameAssets.unitStats.Count; j++)
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
            OpenUIWindow("UnitsRecruitment",0);
            provinceStatsWindow.gameObject.SetActive(true);
            ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
            transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + provinceIndex.ToString();
            transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = provinceStats.population.ToString();
            LoadProvinceUnitCounters(provinceIndex);
        }
        else if (transform != null)
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
