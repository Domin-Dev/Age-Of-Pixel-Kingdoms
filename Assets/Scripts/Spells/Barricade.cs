using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour, ISpellBase
{
    Unit unit;
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
        if (value)
        {
            Sounds.instance.PlaySound(1);
            List<Unit> units = unitsManager.GetPath(pathIndex);
            foreach (Unit unit in units)
            {
                if (unit.unitIndex == 6 && unit.unitIsFriendly == isPlayer)
                {
                    this.unit = unit;
                }
            }
            StartCoroutine(BarricadeLife());
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator BarricadeLife()
    {
        while(true)
        { 
            yield return new WaitForSeconds(1f);
            if (unit != null && unit.gameObject != null) unit.Hit(20);
        }
        yield return 0;
    }

}