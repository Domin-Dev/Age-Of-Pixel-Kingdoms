using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : MonoBehaviour, ISpellBase
{
    UnitsManager unitsManager;
    int pathIndex;
    bool isPlayer;
    public void StartSpell(bool isPlayer,int pathIndex,UnitsManager unitsManager)
    {
        Debug.Log(pathIndex);
        this.isPlayer = isPlayer;
        this.pathIndex = pathIndex;
        this.unitsManager = unitsManager;
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex -1).position.y, transform.position.z);
        Sounds.instance.PlaySound(15);
    }
    public void AnimationEnd()
    {
        Destroy(gameObject);
    }
    public void ExecuteSpell()
    {
       List<Unit> list = unitsManager.GetPath(pathIndex);

        for (int i = 0; i < list.Count; i++) 
        {
           if(isPlayer != list[i].unitIsFriendly) list[i].Hit(100);
        }
    }
}
