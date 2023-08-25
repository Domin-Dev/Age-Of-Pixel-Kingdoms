using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }

    public Transform map;

    public Material outline;

    [Space(20f, order = 0)]

    public GameObject unitCounter;

    [Space(20f, order = 0)]

    public Sprite brownTexture;
    public Sprite blueTexture;

    [Space(20f, order = 0)]

    public Transform unitCounterContentUI;
    public Transform recruitUnitContentUI;
    public Transform buildingsContentUI;
    public Transform moveUnitContentUI;


    [Space(20f, order = 0)]

    public GameObject unitSlotUI;
    public GameObject buildingSlotUI;
    public GameObject unitCounterSlotUI;

    public BuildingStats[] buildingsStats { private set; get; }
    public UnitStats[] unitStats { private set; get; }  

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        buildingsStats = Resources.LoadAll<BuildingStats>("Buildings");
        unitStats = Resources.LoadAll<UnitStats>("Units");
        map = GameObject.FindGameObjectWithTag("GameMap").transform;

    }

    private void Start()
    { 
        DontDestroyOnLoad(gameObject);        
    }


}
