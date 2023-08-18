using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEngine.UI.CanvasScaler;

public class SelectingProvinces : MonoBehaviour
{
    private Transform selectedObject;
    private Color selectedColor;

    private int SelectedIndex = -1;


    private int unitsNumber = 0;
    private int maxUnitsNumber = 100;

    
    private Slider slider;
    private TextMeshProUGUI textMeshPro;
    private Transform buttons;
    private TextMeshProUGUI price;


    private void Start()
    {
        Transform selectionNumberUnits = UIManager.Instance.GetSelectionNumberUnitsWindowWindow();
        buttons = selectionNumberUnits.GetChild(0);
        textMeshPro = selectionNumberUnits.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        slider = selectionNumberUnits.GetChild(1).GetComponent<Slider>();
        price = selectionNumberUnits.GetChild(3).GetComponent<TextMeshProUGUI>();

        slider.maxValue = maxUnitsNumber;
        slider.onValueChanged.AddListener((float value) => { SetUnitsNumber((int)(value)); });

        buttons.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-20); });
        buttons.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-5); });
        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-1); });

        buttons.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(1); });
        buttons.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(5); });
        buttons.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(20); });

        

        //     GameAssets.Instance.buildWorkshop.GetComponent<Button>().onClick.AddListener(() =>Build(0));;
        //     GameAssets.Instance.buildFort.GetComponent<Button>().onClick.AddListener(() =>Build(1));;
        //     GameAssets.Instance.buildUniversity.GetComponent<Button>().onClick.AddListener(() =>Build(2));;
    }
    public void SelectingProvince()
    {
        Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (RaycastHit2D item in Physics2D.RaycastAll(worldClickPosition, Vector2.zero))
        {
            if (item.collider.tag == "Province")
            {
                SpriteRenderer spriteRenderer = item.collider.GetComponent<SpriteRenderer>();
                Texture2D spriteTexture = spriteRenderer.sprite.texture;
                Rect rect = spriteRenderer.sprite.rect;

                float x = (worldClickPosition.x - item.collider.transform.position.x) * spriteRenderer.sprite.pixelsPerUnit;
                float y = (worldClickPosition.y - item.collider.transform.position.y) * spriteRenderer.sprite.pixelsPerUnit;

                x += rect.width / 2;
                y += rect.height / 2;

                x += rect.x;
                y += rect.y;

                Color pixel = spriteTexture.GetPixel(Mathf.FloorToInt(x), Mathf.FloorToInt(y));

                if (pixel.a == 0) continue;
                else
                {
                    if (selectedObject != null)
                    {
                        SpriteRenderer spriteRenderer1 = selectedObject.GetComponent<SpriteRenderer>();
                        spriteRenderer1.color = selectedColor;
                        spriteRenderer1.material = GameAssets.Instance.outline;
                        spriteRenderer1.sortingOrder = -10;
                        selectedObject = item.collider.gameObject.transform;
                    }
                    else
                    {
                        selectedObject = item.collider.gameObject.transform;
                    }
                    selectedColor = spriteRenderer.color;
                    spriteRenderer.color = Color.white;
                    spriteRenderer.material = GameAssets.Instance.highlight;
                    spriteRenderer.sortingOrder = -1;
                    spriteRenderer.material.SetColor("_Color_2", selectedColor);

                    UIManager.Instance.OpenUIWindow("ProvinceStats", int.Parse(item.collider.name));
                    break;
                }
            }
        }

      
    }
    public void Build(int index)
    {
        if (selectedObject != null)
        {
            Sprite sprite = GameAssets.Instance.spriteWorkshop;
            switch (index)
            {
                case 0:
                    sprite = GameAssets.Instance.spriteWorkshop;
                    break;
                case 1:
                    sprite = GameAssets.Instance.spriteFort;
                    break;
                case 2:
                    sprite = GameAssets.Instance.spriteUniversity;
                    break;
            }

            Transform transform = new GameObject(selectedObject.name, typeof(SpriteRenderer)).transform;
            transform.position = selectedObject.position + new Vector3(0, 0.1f, 0);
            transform.GetComponent<SpriteRenderer>().sprite = sprite;
            transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
    }


    public void SetUnitsNumber(int unit)
    {
        unitsNumber = Math.Clamp(unit, 0, maxUnitsNumber); 
        UpdateRecruitUI();
    }
    public void AddToUnitsNumber(int number)
    {
        unitsNumber = Math.Clamp(unitsNumber + number, 0, maxUnitsNumber);
        UpdateRecruitUI();
    }
    private void UpdateRecruitUI()
    {
        textMeshPro.text = unitsNumber.ToString() + "/" + maxUnitsNumber.ToString();
        slider.value = unitsNumber;
        price.text = "Price:  <color=#FF0000>"+ unitsNumber * 10 +" <sprite index=4> </color>";
    }



    public void SelectUnitToRecruit(int index)
    {
        if (selectedObject != null)
        {
            if(SelectedIndex >= 0) GameAssets.Instance.contentUI.GetChild(SelectedIndex).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
            SelectedIndex = index;
            GameAssets.Instance.contentUI.GetChild(SelectedIndex).GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;

            unitsNumber = 0;
            ProvinceStats provinceStats = GameManager.Instance.provinces[int.Parse(selectedObject.name)];
            SelectedIndex = index;
            UIManager.Instance.OpenUIWindow("SelectionNumberUnits", 0);
        }
    }
    public void Recruit(float index)

    {
        if (selectedObject != null)
        {



            ProvinceStats provinceStats = GameManager.Instance.provinces[int.Parse(selectedObject.name)];
            provinceStats.unitsCounter++;

            if (provinceStats.units == null) provinceStats.units = new Dictionary<int, int>();


      //      if (provinceStats.units.ContainsKey(index))
       //     {
      //          provinceStats.units[index]++;
       //     }
       //     else
       ///     {
       //         provinceStats.units.Add(index, 1);
        //    }

            Debug.Log(provinceStats.unitsCounter);

            if(provinceStats.unitsCounter > 0 && selectedObject.childCount == 0)
            {
                Instantiate(GameAssets.Instance.unitCounter, selectedObject.transform.position - new Vector3(0, 0.05f, 0), Quaternion.identity, selectedObject);
            }

            selectedObject.GetChild(0).GetComponentInChildren<TextMeshPro>().text = provinceStats.unitsCounter.ToString();

            UIManager.Instance.LoadProvinceUnitCounters(int.Parse(selectedObject.name));
        }
    }


}
