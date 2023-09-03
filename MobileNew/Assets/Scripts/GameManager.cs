using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ProvinceStats[] provinces;
    public int numberOfProvinces;
    public List<Player> playerList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ProvinceStats[] array = Resources.Load<MapStats>("Maps/World").provinces;
            provinces = new ProvinceStats[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                provinces[i] = array[i];    
            }
            numberOfProvinces =  Resources.Load<MapStats>("Maps/World").numberOfProvinces;  

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void LoadPlayers()
    {
        playerList.Add(new Player("xd",true,Color.cyan) );
        playerList.Add(new Player("xd",true,Color.cyan) );
        playerList.Add(new Player("xd",true,Color.cyan) );
    }
}
