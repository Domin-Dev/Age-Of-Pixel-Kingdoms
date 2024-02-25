using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfRocks : MonoBehaviour,ISpellBase
{
    UnitsManager unitsManager;
    int pathIndex;
    bool isPlayer;
    public void AnimationEnd()
    {
        Destroy(gameObject);
    }
    public void ExecuteSpell()
    {
        List<Unit> list = unitsManager.GetPath(pathIndex);

        for (int i = 0; i < list.Count; i++)
        {
            if (isPlayer != list[i].unitIsFriendly) list[i].Hit(150);
        }
    }
    public bool StartSpell(bool isPlayer, int pathIndex, UnitsManager unitsManager)
    { 
        this.isPlayer = isPlayer;
        this.pathIndex = pathIndex;
        this.unitsManager = unitsManager;
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex - 1).position.y, transform.position.z);
        Sounds.instance.PlaySound(5);
        StartCoroutine(Damage());
        return true;
    }

    IEnumerator Damage()
    {
        yield return new WaitForSeconds(1f);
        ExecuteSpell();
        yield return null;
    }
}
