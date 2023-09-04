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

    public PlayerStats humanPlayer;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Transform map = GameObject.FindGameObjectWithTag("GameMap").transform;
            SelectingProvinces selectingProvinces = FindObjectOfType<SelectingProvinces>();

            ProvinceStats[] array = Resources.Load<MapStats>("Maps/World").provinces;
            provinces = new ProvinceStats[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                provinces[i] = new ProvinceStats();
                provinces[i].CopyData(array[i]);
                if (array[i].provinceOwnerIndex != -1)
                {
                    selectingProvinces.ChangeProvinceColor(map.GetChild(i).GetComponent<SpriteRenderer>(),Color.red);
                }
            }
            numberOfProvinces =  Resources.Load<MapStats>("Maps/World").numberOfProvinces;
            humanPlayer = new PlayerStats(1000);
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
