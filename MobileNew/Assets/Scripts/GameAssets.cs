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


}
