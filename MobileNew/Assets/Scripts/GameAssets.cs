using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance { get; private set; }


    
    public Material outline;
    public Material highlight;

    public Transform buildWorkshop;
    public Transform buildFort;
    public Transform buildUniversity;

    [Space(20f,order =0)]

    public Sprite spriteWorkshop;
    public Sprite spriteFort;
    public Sprite spriteUniversity;

    [Space(20f, order = 0)]

    public GameObject unitCounter;

    [Space(20f, order = 0)]

    public List<UnitStats> unitStats;
    public GameObject unitSlotUI;
    public Transform contentUI;

    public Sprite brownTexture;
    public Sprite blueTexture;

    [Space(20f, order = 0)]

    public GameObject unitCounterSlotUI;
    public Transform contentUnitsCounterUI;



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
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


}
