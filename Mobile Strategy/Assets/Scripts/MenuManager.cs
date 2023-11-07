using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform settings;

    [SerializeField] private Transform buttons;




    private void Start()
    {
        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { settings.gameObject.SetActive(true);});

        settings.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => {  settings.gameObject.SetActive(false);});
    }
}
