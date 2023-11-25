using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : MonoBehaviour, ISpellBase
{
    UnitsManager unitsManager;
    public void StartSpell(int pathIndex,UnitsManager unitsManager)
    {
        Debug.Log(pathIndex);
        this.unitsManager = unitsManager;
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex -1).position.y, transform.position.z);
    }

    public void AnimationEnd()
    {
        Destroy(gameObject,5f);
    }
    public void ExecuteSpell()
    {
        
    }
}
