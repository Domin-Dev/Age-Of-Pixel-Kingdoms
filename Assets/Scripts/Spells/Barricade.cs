using System.Collections;
using UnityEngine;

public class Barricade : MonoBehaviour,ISpellBase
{
    public void AnimationEnd()
    {
    }

    public void ExecuteSpell()
    {
    }

    public bool StartSpell(bool isPlayer, int pathIndex, UnitsManager unitsManager)
    {
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex - 1).position.y, transform.position.z);
       bool value = unitsManager.CreateUnit(6, unitsManager.paths.GetChild(pathIndex - 1));
        if(value) 
        {        
            Sounds.instance.PlaySound(1);
            return true;
        }else
        {
            return false;
        }
    }

}