using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MapStats stats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            stats = Resources.Load<MapStats>("Maps/World");





         //   Debug.Log(stats.numberOfProvinces);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }






}
