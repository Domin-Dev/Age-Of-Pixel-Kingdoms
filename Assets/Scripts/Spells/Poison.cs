using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour, ISpellBase
{
    bool isPlayer;
    int pathIndex;
    UnitsManager unitsManager;
    public void AnimationEnd()
    {
    }

    public void ExecuteSpell()
    {
    }

    public bool StartSpell(bool isPlayer, int pathIndex, UnitsManager unitsManager)
    {
        this.isPlayer = isPlayer;
        this.pathIndex = pathIndex;
        this.unitsManager = unitsManager;
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex - 1).position.y, transform.position.z);
        Sounds.instance.PlaySound(16);
        StartCoroutine(Damage());
        return true;
    }
    IEnumerator Damage()
    {
        for (int i = 0; i < 20; i++)
        {
            List<Unit> list = unitsManager.GetPath(pathIndex);
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].unitIsFriendly != isPlayer)
                {
                    list[j].Hit(10f);
                }
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
