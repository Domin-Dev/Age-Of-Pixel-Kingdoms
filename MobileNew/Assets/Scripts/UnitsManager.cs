using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    [SerializeField]public GameObject unit;

    [SerializeField]  List<Unit> path1 = new List<Unit>();
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

                    List<Unit> units = GetPath(int.Parse(raycastHit.collider.name));
                    Unit unit = Instantiate(this.unit, raycastHit.collider.transform.GetChild(0).transform.position + new Vector3(0f,0.4f,0f),Quaternion.identity).GetComponent<Unit>();
                    unit.SetUp(true,raycastHit.collider.transform.GetChild(1).position + new Vector3(1.5f,0,0),() => { units.Remove(unit); });
                    units.Add(unit);
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

                    List<Unit> units = GetPath(int.Parse(raycastHit.collider.name));
                    Unit unit = Instantiate(this.unit, raycastHit.collider.transform.GetChild(1).transform.position + new Vector3(0f, 0.4f, 0f), Quaternion.identity).GetComponent<Unit>();
                    unit.SetUp(false, raycastHit.collider.transform.GetChild(0).position + new Vector3(1.5f, 0, 0), () => { units.Remove(unit); });
                    units.Add(unit);
                }
            }
        }
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
