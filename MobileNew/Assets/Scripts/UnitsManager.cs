using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    [SerializeField]public GameObject unit;

    [SerializeField] List<Unit> path1 = new List<Unit>();
    [SerializeField] List<Unit> path2 = new List<Unit>();
    [SerializeField] List<Unit> path3 = new List<Unit>();
    [SerializeField] List<Unit> path4 = new List<Unit>();




    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (RaycastHit2D raycastHit in Physics2D.RaycastAll(worldMousePosition, Vector3.zero))
            {
                if(raycastHit.collider.CompareTag("Path"))
                {

                    CreateUnit(unit, raycastHit.collider.transform);
                }
            }


        }


        if (Input.GetMouseButtonDown(1))
        {

            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (RaycastHit2D raycastHit in Physics2D.RaycastAll(worldMousePosition, Vector3.zero))
            {
                if (raycastHit.collider.CompareTag("Path"))
                {
                    EnemyCreateUnit(unit, raycastHit.collider.transform);
                }
            }
        }
    }

    private void CreateUnit(GameObject gameObject,Transform pathTransform)
    {
        int path = int.Parse(pathTransform.name);
        List<Unit> units = GetPath(path);

        Unit unit = Instantiate(gameObject, pathTransform.GetChild(0).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
     //   unit.SetUp(path,true, pathTransform.GetChild(1).position.x, () => { units.Remove(unit); }, (unit) => { return CheckPath(unit); });
        units.Add(unit);
    }

    private void EnemyCreateUnit(GameObject gameObject, Transform pathTransform)
    {
        int path = int.Parse(pathTransform.name);
        List<Unit> units = GetPath(path);

        Unit unit = Instantiate(gameObject, pathTransform.GetChild(1).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
       // unit.SetUp(path, false, pathTransform.GetChild(0).position.x, () => { units.Remove(unit); }, (unit) => { return CheckPath(unit); });
        units.Add(unit);
    }





    private Unit CheckPath(Unit selectedUnit)
    {
        List<Unit> units = GetPath(selectedUnit.pathIndex);

        foreach (Unit unit in units) 
        {
            if (unit != selectedUnit)
            {
                float selectedUnitPosX = selectedUnit.transform.position.x;
                float unitPosX = unit.transform.position.x;


                float distance = Math.Abs(selectedUnitPosX - unitPosX);

                if(unit.unitIsFriendly != selectedUnit.unitIsFriendly)
                {
                    if(distance <= selectedUnit.range) return unit;
                }            
                else
                { 
                   if(distance < 1.1f && selectedUnitPosX * selectedUnit.multiplier < unitPosX * unit.multiplier ) return unit;
                }
                
            }
        }
        return null;
    }

    private List<Unit> GetPath(int index)
    {
        switch (index)
        {
            case 1: return path1;
            case 2: return path2;
            case 3: return path3;
            case 4: return path4;
        }
        return path1;
    }

}
