using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : MonoBehaviour, ISpellBase
{
    UnitsManager unitsManager;
    int pathIndex;
    bool isPlayer;
    public bool StartSpell(bool isPlayer,int pathIndex,UnitsManager unitsManager)
    {
        this.isPlayer = isPlayer;
        this.pathIndex = pathIndex;
        this.unitsManager = unitsManager;
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex -1).position.y, transform.position.z);
        Sounds.instance.PlaySound(15);
        StartCoroutine(Damage());
        return true;
    }
    public void AnimationEnd()
    {
    }
    public void ExecuteSpell()
    {
       List<Unit> list = unitsManager.GetPath(pathIndex);

        for (int i = 0; i < list.Count; i++) 
        {
           if(isPlayer != list[i].unitIsFriendly) list[i].Hit(100);
        }
    }

    IEnumerator Damage()
    {
        yield return new WaitForSeconds(0.9f);
        Debug.Log("git");
        ExecuteSpell();
        yield return null;
    }
}
