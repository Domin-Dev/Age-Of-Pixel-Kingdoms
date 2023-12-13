using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour, ISpellBase
{

    UnitsManager unitsManager;
    int pathIndex;
    bool isPlayer;
    public void AnimationEnd()
    {
    }

    public void ExecuteSpell()
    {
    }

    public bool StartSpell(bool isPlayer, int pathIndex, UnitsManager unitsManager)
    {
        this.pathIndex = pathIndex;
        this.isPlayer = isPlayer;
        this.unitsManager = unitsManager;
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex - 1).position.y, transform.position.z);
        Sounds.instance.PlaySound(16);
        StartCoroutine(FireDamage());
        return true;
    }

    IEnumerator FireDamage()
    {
        for (int i = 0; i < 10; i++)
        {
            List<Unit> list = unitsManager.GetPath(pathIndex);
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].unitIsFriendly != isPlayer)
                {
                    list[j].Hit(20f);
                }
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
 
}
