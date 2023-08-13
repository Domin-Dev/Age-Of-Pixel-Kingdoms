
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    public void OpenProvinceStats(int index)
    {
        provinceStatsWindow.gameObject.SetActive(true);

        ProvinceStats provinceStats = GameManager.Instance.stats.provinces[index];

        provinceStatsWindow.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Province " + index.ToString();
        provinceStatsWindow.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = provinceStats.population.ToString();

    }
    public void CloseProvinceStats()
    {
        provinceStatsWindow.gameObject.SetActive(false);
    }

    public void BuildBuilding(int index)
    {
        switch (index)
        {
            case 0: 
                break;

        }
    }


}
