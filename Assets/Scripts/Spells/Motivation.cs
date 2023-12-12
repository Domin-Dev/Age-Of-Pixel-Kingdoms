using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motivation : MonoBehaviour, ISpellBase
{
    public void AnimationEnd()
    {
    }

    public void ExecuteSpell()
    {
    }

    public void StartSpell(bool isPlayer, int pathIndex, UnitsManager unitsManager)
    {
        this.transform.position = new Vector3(transform.position.x, unitsManager.paths.GetChild(pathIndex - 1).position.y, transform.position.z);
        List<Unit> list = unitsManager.GetPath(pathIndex);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].unitIsFriendly == isPlayer)
            {
                list[i].SpeedBoost(1.5f);
            }
        }
        Sounds.instance.PlaySound(8);
    }
}
