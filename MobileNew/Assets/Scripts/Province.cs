using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Province : MonoBehaviour
{
    public Transform[] neighbors;
    public void SetUp(Object[] neighbors)
    {
        this.neighbors = new Transform[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            this.neighbors[i] = neighbors[i].GetComponent<Transform>();
        }
    }
}
