using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ProvinceStats[] provinces;
    public int numberOfProvinces;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            provinces = Resources.Load<MapStats>("Maps/World").provinces;
            numberOfProvinces =  Resources.Load<MapStats>("Maps/World").numberOfProvinces;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }






}
