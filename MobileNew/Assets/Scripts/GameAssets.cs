using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Space(20f, order = 0)]

    public Transform moveUnitContentUI1;
    public Transform moveUnitContentUI2;

    [Space(20f, order = 0)]
    
    public Transform AttackUnitContentUI1;
    public Transform AttackUnitContentUI2;

    [Space(20f, order = 0)]

    public GameObject unitSlotUI;
    public GameObject buildingSlotUI;
    public GameObject unitCounterSlotUI;


    [Space(40f, order = 0)]

    public GameObject BattleConter;
    public Transform BattleUnits
        ;
    public Transform battleYourBar;
    public Transform battleEnemyBar;

    public Transform battleInfo;


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
        
        if(SceneManager.GetActiveScene().buildIndex == 0)
        map = GameObject.FindGameObjectWithTag("GameMap").transform;


        buildingsStats = Resources.LoadAll<BuildingStats>("Buildings");
        unitStats = Resources.LoadAll<UnitStats>("Units");    
    }

}
