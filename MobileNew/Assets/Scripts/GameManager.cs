using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MapStats Stats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Stats = AssetDatabase.LoadAssetAtPath<MapStats>("Assets/Maps/Map1/World.asset");

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

 

   

}
