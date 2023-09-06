using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SelectingProvinces : MonoBehaviour
{
    public Transform selectedProvince;
    public Transform selectedNeighbor;
    private bool moveMode = false;


    private Color selectedColor;

    private int selectedUnitIndex = -1;
    private int selectedProvinceNumber = 0;


    private int unitsNumber = 0;
    private int maxUnitsNumber = 100;


    private TextMeshProUGUI nameWindow;
    private Slider slider;
    private TextMeshProUGUI numberText;
    private Transform buttons;
    private TextMeshProUGUI price;
    private TextMeshProUGUI buttonText;
    private Button buttonRecruit;

    private Transform buildingsParent;
    private Transform map;


    private void Start()
    {
        buildingsParent = GameObject.FindGameObjectWithTag("Buildings").transform;
        Transform selectionNumberUnits = UIManager.Instance.GetSelectionNumberUnitsWindowWindow();
        nameWindow = selectionNumberUnits.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        buttons = selectionNumberUnits.GetChild(0);
        numberText = selectionNumberUnits.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        slider = selectionNumberUnits.GetChild(1).GetComponent<Slider>();
        price = selectionNumberUnits.GetChild(3).GetComponent<TextMeshProUGUI>();
        buttonRecruit = selectionNumberUnits.GetChild(4).GetComponent<Button>();
        buttonText = buttonRecruit.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        buttonRecruit.onClick.AddListener(() => { Recruit(); });
        slider.maxValue = maxUnitsNumber;
        slider.onValueChanged.AddListener((float value) => { SetUnitsNumber((int)(value)); });
        buttons.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-20); });
        buttons.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-5); });
        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-1); });

        buttons.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(1); });
        buttons.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(5); });
        buttons.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(20); });

        UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { MoveAll(1); });
        UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => { MoveHalf(1); });

        UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { MoveAll(2); }); 
        UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => { MoveHalf(2); });


         map = GameObject.FindGameObjectWithTag("GameMap").transform;
 //       ChangeProvinceColor(map.GetChild(4).GetComponent<SpriteRenderer>(), Color.blue);
    }
    private void SetSelectionNumberUnits(bool isMove)
    {
        if(isMove)
        {
            buttonRecruit.onClick.RemoveAllListeners();
            buttonRecruit.onClick.AddListener(() => { Move(); });
            nameWindow.text = "movement of units";
            buttonText.text = "Move";
        }
        else
        {
            buttonRecruit.onClick.RemoveAllListeners();
            buttonRecruit.onClick.AddListener(() => { Recruit(); });
            nameWindow.text = "recruitment";
            buttonText.text = "Recruit";
        }
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
                    

                    if (selectedProvince != null)
                    {
                        if (selectedProvince.name == item.collider.gameObject.name && GetProvinceStats(selectedProvince).provinceOwnerIndex == 0)
                        {
                            if(moveMode)
                            {
                                ResetNeighbors();
                            }
                            else
                            {
                                HighlightNeighbors();
                            }
                            
                        }
                        else
                        {
                            if(moveMode && IsNeighbor(int.Parse(item.collider.gameObject.name)))
                            {
                                selectedNeighbor = item.collider.gameObject.transform;
                                ProvinceStats province = GetProvinceStats(selectedNeighbor.transform);
                                if (province.unitsCounter == 0 || province.provinceOwnerIndex == 0)
                                {
                                    UIManager.Instance.LoadUnitsMove(int.Parse(selectedProvince.name), int.Parse(item.collider.gameObject.name), false);
                                }
                                else
                                {
                                    UIManager.Instance.LoadUnitsAttack(int.Parse(selectedProvince.name), int.Parse(item.collider.gameObject.name));
                                }

                            }
                            else if(!GetProvinceStats(item.collider.transform).isSea)
                            {
                                UIManager.Instance.CloseUIWindow("ProvinceStats");
                                ResetNeighbors();
                                selectedProvince = item.collider.gameObject.transform;
                            }else
                            {
                                return;
                            }
                        }
                    }else if (!GetProvinceStats(item.collider.transform).isSea)
                    {
                        selectedProvince = item.collider.gameObject.transform;
                    }else
                    {
                        return;
                    }


                    ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
                    if (!provinceStats.isSea && provinceStats.provinceOwnerIndex == 0)
                    {
                        UIManager.Instance.ManagerUI(true);
                    }

                    spriteRenderer.sortingOrder = -1;
                    ChangeProvinceBorderColor(spriteRenderer, Color.white);
                    UIManager.Instance.OpenUIWindow("ProvinceStats", int.Parse(item.collider.name));
                    break;
                }
            }
        }

      
    }
    public void ClearSelectedProvince()
    {
        if (selectedProvince != null)
        {
            SpriteRenderer spriteRenderer = selectedProvince.GetComponent<SpriteRenderer>();
            ChangeProvinceBorderColor(spriteRenderer, Color.black);
            if (GetProvinceStats(selectedProvince).isSea) spriteRenderer.sortingOrder = -11;
            else spriteRenderer.sortingOrder = -10;
            selectedProvince = null;
            UIManager.Instance.ManagerUI(false);
        }
    }
    public void Build(int index)
    {
        BuildingStats buildingStats = GameAssets.Instance.buildingsStats[index];
        if (selectedProvince != null && GameManager.Instance.humanPlayer.CanAfford(buildingStats.price))
        {
            ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
            if (provinceStats.buildingIndex == -1)
            {
                GameManager.Instance.humanPlayer.Subtract(buildingStats.price);
                Transform transform = new GameObject(selectedProvince.name, typeof(SpriteRenderer)).transform;
                transform.position = selectedProvince.position + new Vector3(0, 0.08f, 0);
                transform.parent = buildingsParent;
                transform.GetComponent<SpriteRenderer>().sprite = buildingStats.icon;
                transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
                provinceStats.buildingIndex = index;
            }
            UIManager.Instance.LoadBuildings(int.Parse(selectedProvince.name));
        }
        else
        {
            Alert.Instance.OpenAlert("not enough coins!");
        }

    }

    public void HighlightNeighbors()
    {
        if (selectedProvince != null)
        { 
            List<int> list = GetProvinceStats(selectedProvince).neighbors;

            for (int i = 0; i < list.Count; i++)
            {
               SpriteRenderer spriteRenderer =  GameAssets.Instance.map.GetChild(list[i]).GetComponent<SpriteRenderer>();
               spriteRenderer.sortingOrder = -2;
               ChangeProvinceBorderColor(spriteRenderer, Color.yellow);
            }
            moveMode = true;
        }
    }
    public void ResetNeighbors()
    {
        if (selectedProvince != null)
        {
            List<int> list = GetProvinceStats(selectedProvince).neighbors;

            for (int i = 0; i < list.Count; i++)
            {
                SpriteRenderer spriteRenderer = GameAssets.Instance.map.GetChild(list[i]).GetComponent<SpriteRenderer>();
                ChangeProvinceBorderColor(spriteRenderer, Color.black);
                spriteRenderer.sortingOrder = -10;      
            }
            moveMode = false;
        }
    }
    public bool IsNeighbor(int index)
    {
        if (selectedProvince != null)
        {
            List<int> list = GetProvinceStats(selectedProvince).neighbors;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == index)
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }
    public void ChangeProvinceColor(SpriteRenderer spriteRenderer,Color provinceColor)
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        if (spriteRenderer.HasPropertyBlock())
        {
            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("Color2", provinceColor);
        }
        else
        {         
            materialPropertyBlock.SetColor("Color1", Color.black);
            materialPropertyBlock.SetFloat("Float", -0.05f);
            materialPropertyBlock.SetColor("Color2", provinceColor);
            materialPropertyBlock.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }
    public void ChangeProvinceBorderColor(SpriteRenderer spriteRenderer, Color borderColor)
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        if (spriteRenderer.HasPropertyBlock())
        {
            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("Color1", borderColor);
        }
        else
        {
            materialPropertyBlock.SetColor("Color1", borderColor);
            materialPropertyBlock.SetFloat("Float", -0.05f);
            materialPropertyBlock.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
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
        numberText.text = unitsNumber.ToString() + "/" + maxUnitsNumber.ToString();
        slider.value = unitsNumber;
        price.text = "Price:  <color=#FF0000>"+ unitsNumber * GameAssets.Instance.unitStats[selectedUnitIndex].price +" <sprite index=20> </color>";
    }
    public void ResetUnits()
    {
        if (selectedUnitIndex >= 0)
        {
            if (selectedProvinceNumber == 0)
            {
                GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
                selectedUnitIndex = -1;
            }
            else
            {
                GetCounter(selectedUnitIndex, selectedProvinceNumber).GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;
                selectedUnitIndex = -1;
                selectedProvinceNumber = 0;
            }
        }
    }
    private Transform GetUIContent(int provinceNumber)
    {
        switch (provinceNumber)
        {
            case 1: return GameAssets.Instance.moveUnitContentUI1;
            case 2: return GameAssets.Instance.moveUnitContentUI2;
        }
        return null;
    }
    private Transform GetCounter(int unitIndex, int provinceNumber)
    {
        Transform transform = GetUIContent(provinceNumber);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if(unitIndex.ToString() == child.name)
            {
                return child;
            }          
        }
        return null;
    }
    private ProvinceStats GetProvinceStats(int provinceNumber) 
    {
        if (provinceNumber > 0)
        {
            Transform transform;
            if (provinceNumber == 1) transform = selectedProvince;
            else transform = selectedNeighbor;

            return GameManager.Instance.provinces[int.Parse(transform.name)];
        }
        else
            return null;
    }
    public void SelectUnitToMove(int index, int provinceNumber)
    {
        if (selectedProvince != null)
        {
            if (selectedUnitIndex >= 0 && selectedProvinceNumber >= 0)
            {
                Transform selected = GetCounter(selectedUnitIndex, selectedProvinceNumber);
                if (selected != null) selected.GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;
            }

            selectedUnitIndex = index;
            selectedProvinceNumber = provinceNumber;

            Transform transform = GetCounter(selectedUnitIndex, selectedProvinceNumber);
            if(transform != null)  transform.GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;


            if (GetProvinceStats(provinceNumber).units.ContainsKey(index))
                maxUnitsNumber = GetProvinceStats(provinceNumber).units[index];
            slider.maxValue = maxUnitsNumber;
            unitsNumber = 0;
            UpdateRecruitUI();
            ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
            selectedUnitIndex = index;

            SetSelectionNumberUnits(true);
            UIManager.Instance.OpenUIWindow("SelectionNumberUnits", 0);

        }
    }
    public void SelectUnitToRecruit(int index)
    {
        if (selectedProvince != null && GameManager.Instance.humanPlayer.CanAfford(GameAssets.Instance.unitStats[index].price))
        {
            if(selectedUnitIndex >= 0) GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
            selectedUnitIndex = index;
            GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;

            maxUnitsNumber = GameManager.Instance.humanPlayer.coins / GameAssets.Instance.unitStats[index].price;
            slider.maxValue = maxUnitsNumber;

            unitsNumber = 0;
            UpdateRecruitUI();
            ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
            selectedUnitIndex = index;

            SetSelectionNumberUnits(false);
            UIManager.Instance.OpenUIWindow("SelectionNumberUnits", 0);
        }else
        {
            Alert.Instance.OpenAlert("not enough coins!");
        }
    }
    public void Recruit()
    {
        if (selectedProvince != null && unitsNumber > 0)
        {
            ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
            provinceStats.unitsCounter += unitsNumber;
            GameManager.Instance.humanPlayer.Subtract(unitsNumber * GameAssets.Instance.unitStats[selectedUnitIndex].price);

            if (provinceStats.units == null) provinceStats.units = new Dictionary<int, int>();

            if (provinceStats.units.ContainsKey(selectedUnitIndex))
            {
                provinceStats.units[selectedUnitIndex] += unitsNumber;
            }
            else
            {
                provinceStats.units.Add(selectedUnitIndex, unitsNumber);
            }


            if (selectedProvince.childCount == 0)
            {
                Instantiate(GameAssets.Instance.unitCounter, selectedProvince.transform.position - new Vector3(0, 0.05f, 0), Quaternion.identity, selectedProvince);
            }
            selectedProvince.GetChild(0).GetComponentInChildren<TextMeshPro>().text = provinceStats.unitsCounter.ToString();
           
            UIManager.Instance.LoadProvinceUnitCounters(int.Parse(selectedProvince.name), GameAssets.Instance.unitCounterContentUI.transform,false);
            GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
            UIManager.Instance.CloseUIWindow("SelectionNumberUnits");
        }
    }
    public void Move()
    {
        if(selectedNeighbor != null && selectedProvince != null && unitsNumber > 0)
        {
            ProvinceStats provinceStats1 = GetProvinceStats(selectedProvince);
            ProvinceStats provinceStats2 = GetProvinceStats(selectedNeighbor);

            if (selectedProvinceNumber == 1)
            {
                MoveTo(provinceStats1, provinceStats2);
            }
            else if(selectedProvinceNumber == 2)
            {
                MoveTo(provinceStats2, provinceStats1);
            }

            UpdateUnitNumber(selectedProvince);
            UpdateUnitNumber(selectedNeighbor);
            UIManager.Instance.CloseUIWindow("SelectionNumberUnits");
            UIManager.Instance.LoadUnitsMove(int.Parse(selectedProvince.name), int.Parse(selectedNeighbor.name),true);
        }
    }
    private void MoveTo(ProvinceStats from, ProvinceStats to) 
    {
        from.unitsCounter -= unitsNumber;
        from.units[selectedUnitIndex] -= unitsNumber;
        to.unitsCounter += unitsNumber;

        if (to.units != null)
        {
            if (to.units.ContainsKey(selectedUnitIndex))
                to.units[selectedUnitIndex] += unitsNumber;
            else
                to.units.Add(selectedUnitIndex, unitsNumber);
        }
        else
        {
            to.units = new Dictionary<int, int>();
            to.units.Add(selectedUnitIndex, unitsNumber);
        }

        if(unitsNumber > 0 && to.provinceOwnerIndex == -1)
        {
            to.SetNewOwner(0);
            ChangeProvinceColor(map.GetChild(to.index).GetComponent<SpriteRenderer>(), Color.red);
        }
        UIManager.Instance.OpenUIWindow("ProvinceStats", to.index);
    } 
    public void UpdateUnitNumber(Transform province)
    {
        int number = GameManager.Instance.provinces[int.Parse(province.name)].unitsCounter;
        if (province.childCount == 0 && number > 0)
        {
            Instantiate(GameAssets.Instance.unitCounter, province.transform.position - new Vector3(0, 0.05f, 0), Quaternion.identity, province);
        }else if(number == 0 && province.childCount > 0) 
        {
            Destroy(province.GetChild(0).gameObject);
            return;
        }
        if(province.childCount > 0) province.GetChild(0).GetComponentInChildren<TextMeshPro>().text = number.ToString();
    }
    private void MoveAll(int provinceNumber)
    {
        UIManager.Instance.CloseUIWindow("SelectionNumberUnits");
        ProvinceStats provinceStatsFrom, provinceStatsTo;
        if (provinceNumber == 1)
        {
            provinceStatsFrom = GetProvinceStats(selectedProvince);
            provinceStatsTo = GetProvinceStats(selectedNeighbor);
        }
        else
        {
            provinceStatsFrom = GetProvinceStats(selectedNeighbor);
            provinceStatsTo = GetProvinceStats(selectedProvince);
        }
        
        if (provinceStatsFrom.units != null)
        {
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
                selectedUnitIndex = i;
                if(provinceStatsFrom.units.ContainsKey(selectedUnitIndex))
                {
                    unitsNumber = provinceStatsFrom.units[selectedUnitIndex];
                    MoveTo(provinceStatsFrom, provinceStatsTo);
                }
            }

            UpdateUnitNumber(selectedProvince);
            UpdateUnitNumber(selectedNeighbor);
            UIManager.Instance.LoadUnitsMove(int.Parse(selectedProvince.name), int.Parse(selectedNeighbor.name), true);
            selectedUnitIndex = -1;
        }
    }
    private void MoveHalf(int provinceNumber)
    {
        UIManager.Instance.CloseUIWindow("SelectionNumberUnits");
        ProvinceStats provinceStatsFrom, provinceStatsTo;
        if (provinceNumber == 1)
        {
            provinceStatsFrom = GetProvinceStats(selectedProvince);
            provinceStatsTo = GetProvinceStats(selectedNeighbor);
        }
        else
        {
            provinceStatsFrom = GetProvinceStats(selectedNeighbor);
            provinceStatsTo = GetProvinceStats(selectedProvince);
        }

        int numberFrom = provinceStatsFrom.unitsCounter / 2;
        int numberTo = 0;
        int b = 0;

        if (provinceStatsFrom.units != null)
        {
            for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
            {
                selectedUnitIndex = i;
                if (provinceStatsFrom.units.ContainsKey(selectedUnitIndex))
                {
                    int units  = provinceStatsFrom.units[selectedUnitIndex];

                    if(units % 2 == 1)
                    {
                        b++;
                    }

                    if( b > 0 && (numberFrom + numberTo) / 2 == 0 && units > units / 2 + b)
                    {
                        Debug.Log(b);
                        unitsNumber = units / 2 + (b/2);
                        b = 0;
                    }
                    else if( b > 1)
                    {
                        Debug.Log(b);
                        unitsNumber = units / 2 + (b/2);
                        b = 0;
                    } 
                    else
                    {
                        unitsNumber = units / 2;
                    }
                    
                    numberFrom -= unitsNumber;
                    numberTo += unitsNumber;
                    MoveTo(provinceStatsFrom, provinceStatsTo);
                }
            }


            UpdateUnitNumber(selectedProvince);
            UpdateUnitNumber(selectedNeighbor);
            UIManager.Instance.LoadUnitsMove(int.Parse(selectedProvince.name), int.Parse(selectedNeighbor.name), true);
            selectedUnitIndex = -1;
        }
    }


    private ProvinceStats GetProvinceStats(Transform province)
    { 
        return GameManager.Instance.provinces[int.Parse(province.name)];
    }
}