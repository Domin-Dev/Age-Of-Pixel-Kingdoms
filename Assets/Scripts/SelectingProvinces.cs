using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.Mathematics;
using static UnityEditor.Progress;
using UnityEditor;

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

    private int barState = -1;

    public Button moveAll1;
    public Button moveHalf1;

    public Button moveAll2;
    public Button moveHalf2;

    private Transform battleIcon;
    private void Start()
    {
        barState = -1;
        buildingsParent = GameObject.FindGameObjectWithTag("Buildings").transform;
        Transform selectionNumberUnits = UIManager.Instance.GetSelectionNumberUnitsWindowWindow();
        nameWindow = selectionNumberUnits.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        buttons = selectionNumberUnits.GetChild(0);
        numberText = selectionNumberUnits.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        slider = selectionNumberUnits.GetChild(1).GetComponent<Slider>();
        price = selectionNumberUnits.GetChild(3).GetComponent<TextMeshProUGUI>();
        buttonRecruit = selectionNumberUnits.GetChild(4).GetComponent<Button>();
        buttonText = buttonRecruit.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        buttonRecruit.onClick.RemoveAllListeners();
        buttonRecruit.onClick.AddListener(() => { Sounds.instance.PlaySound(0); Recruit(); });
        slider.maxValue = maxUnitsNumber;
        slider.onValueChanged.AddListener((float value) => { SetUnitsNumber((int)(value)); Sounds.instance.PlaySound(5); });


        buttons.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-20); Sounds.instance.PlaySound(5); });
        buttons.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-5); Sounds.instance.PlaySound(5); });
        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(-1); Sounds.instance.PlaySound(5); });

        buttons.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(1); Sounds.instance.PlaySound(5); });
        buttons.GetChild(4).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(5); Sounds.instance.PlaySound(5); });
        buttons.GetChild(5).GetComponent<Button>().onClick.AddListener(() => { AddToUnitsNumber(20); Sounds.instance.PlaySound(5); });

        moveAll1 = UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>();
        moveHalf1 = UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>();

        moveAll2 = UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<Button>();
        moveHalf2 = UIManager.Instance.GetUnitsWindow().GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<Button>();

        moveAll1.onClick.AddListener(() => { MoveAll(1); });
        moveHalf1.onClick.AddListener(() => { MoveHalf(1); });

        moveAll2.onClick.AddListener(() => { MoveAll(2); });
        moveHalf2.onClick.AddListener(() => { MoveHalf(2); });


        map = GameObject.FindGameObjectWithTag("GameMap").transform;
        SetUpBattleIcon();
    }

    private void SetUpBattleIcon()
    {
        battleIcon = new GameObject("Battle", typeof(SpriteRenderer)).transform;
        battleIcon.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        battleIcon.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.battleIcon;
        battleIcon.GetComponent<SpriteRenderer>().sortingOrder = 60;
        battleIcon.gameObject.SetActive(false);
    }

    float currentTime = 0;
    float time = 0;
    private void Update()
    {
        if (time > 0)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= time)
            {
                battleIcon.gameObject.SetActive(false);
                time = 0;
                currentTime = 0;
            }
        }
    }
    private void SetSelectionNumberUnits(bool isMove)
    {
        if (isMove)
        {
            buttonRecruit.onClick.RemoveAllListeners();
            buttonRecruit.onClick.AddListener(() => { Sounds.instance.PlaySound(7); Move(); });
            nameWindow.text = "movement of units";
            buttonText.text = "Move";
        }
        else
        {
            buttonRecruit.onClick.RemoveAllListeners();
            buttonRecruit.onClick.AddListener(() => { Sounds.instance.PlaySound(0); Recruit(); });
            nameWindow.text = "recruitment";
            buttonText.text = "Recruit";
        }
    }
    public void SelectingProvince()
    {
        Sounds.instance.PlaySound(6);
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
                            if (moveMode)
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
                            if (moveMode && IsNeighbor(int.Parse(item.collider.gameObject.name)))
                            {
                                selectedNeighbor = item.collider.gameObject.transform;
                                ProvinceStats province = GetProvinceStats(selectedNeighbor.transform);
                                if (province.unitsCounter == 0 || province.provinceOwnerIndex == 0)
                                {
                                    UIManager.Instance.LoadUnitsMove(int.Parse(selectedProvince.name), int.Parse(item.collider.gameObject.name), false);
                                }
                                else
                                {
                                    UIManager.Instance.LoadUnitsAttack(int.Parse(selectedProvince.name), int.Parse(item.collider.gameObject.name), null,true);
                                }

                            }
                            else if (!GetProvinceStats(item.collider.transform).isSea)
                            {
                                UIManager.Instance.CloseUIWindow("ProvinceStats");
                                ResetNeighbors();
                                selectedProvince = item.collider.gameObject.transform;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else if (!GetProvinceStats(item.collider.transform).isSea)
                    {
                        selectedProvince = item.collider.gameObject.transform;
                    }
                    else
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
        int price = buildingStats.price;
        int priceMP = buildingStats.movementPointsPrice;

        if (GameManager.Instance.humanPlayer.stats.cheaperBuilding) price = price - 50;
        if (GameManager.Instance.humanPlayer.stats.movementBuilding) priceMP = priceMP - 5;

        if (selectedProvince != null && GameManager.Instance.humanPlayer.stats.coins.CanAfford(price))
        {
            if (GameManager.Instance.humanPlayer.stats.movementPoints.CanAfford(priceMP))
            {
                ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
                if (provinceStats.buildingIndex == -1)
                {
                    provinceStats.buildingIndex = index;
                    Sounds.instance.PlaySound(1);
                    BonusManager.SetBonus(provinceStats, buildingStats.bonusIndex);

                    GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(priceMP);
                    GameManager.Instance.humanPlayer.stats.coins.Subtract(price);

                    Transform transform = new GameObject(selectedProvince.name, typeof(SpriteRenderer)).transform;
                    transform.position = selectedProvince.position + new Vector3(0, 0.08f, 0);
                    transform.parent = buildingsParent;
                    transform.GetComponent<SpriteRenderer>().sprite = buildingStats.icon;
                    transform.GetComponent<SpriteRenderer>().sortingOrder = 0;

                }
                UIManager.Instance.LoadBuildings(int.Parse(selectedProvince.name));
                UIManager.Instance.OpenUIWindow("ProvinceStats", int.Parse(selectedProvince.name));
            }
            else
            {
                Alert.Instance.OpenAlert("no movement points!");
            }
        }
        else
        {
            Alert.Instance.OpenAlert("not enough coins!");
        }

    }
    public void Destroy()
    {
        if (selectedProvince != null)
        {
            ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
            BuildingStats buildingStats = GameAssets.Instance.buildingsStats[provinceStats.buildingIndex];

            for (int i = 0; i < buildingsParent.childCount; i++)
            {
                if (buildingsParent.GetChild(i).name == provinceStats.index.ToString())
                {
                    Destroy(buildingsParent.GetChild(i).gameObject);
                    break;
                }
            }
            Sounds.instance.PlaySound(2);
            int index = provinceStats.buildingIndex;
            provinceStats.buildingIndex = -1;
            BonusManager.RemoveBonus(provinceStats, index);


            UIManager.Instance.UpdateCounters();
            UIManager.Instance.LoadBuildings(int.Parse(selectedProvince.name));
            UIManager.Instance.OpenUIWindow("ProvinceStats", int.Parse(selectedProvince.name));
        }
    }
    public void HighlightNeighbors()
    {
        if (selectedProvince != null)
        {
            List<int> list = GetProvinceStats(selectedProvince).neighbors;

            for (int i = 0; i < list.Count; i++)
            {
                SpriteRenderer spriteRenderer = GameAssets.Instance.map.GetChild(list[i]).GetComponent<SpriteRenderer>();
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
    public void ChangeProvinceColor(SpriteRenderer spriteRenderer, Color provinceColor)
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
        if (barState != -1)
        {
            UnitStats unitStats = GameAssets.Instance.unitStats[selectedUnitIndex];
            int priceCoins = unitStats.price;
            int priceMP = unitStats.movementPointsPrice;
            if (GameManager.Instance.humanPlayer.stats.cheaperRecruitment) priceCoins = priceCoins - 5;
            if (GameManager.Instance.humanPlayer.stats.movementRecruitment) priceMP = priceMP - 1;

            numberText.text = unitsNumber.ToString() + "/" + maxUnitsNumber.ToString();
            slider.value = unitsNumber;
            if (barState == 1) price.text = "Price:  <color=#FF0000>" + unitsNumber * priceCoins + " <sprite index=21>  " + unitsNumber + " <sprite index=1>  " + unitsNumber * priceMP + " <sprite index=23>";
            else if (barState == 0) price.text = "Price:  <color=#FF0000>" + unitsNumber + " <sprite index=23>";
        }
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
            if (unitIndex.ToString() == child.name)
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
        if (selectedProvince != null && GameManager.Instance.humanPlayer.stats.movementPoints.value > 0)
        {
            if (selectedUnitIndex >= 0 && selectedProvinceNumber >= 0)
            {
                Transform selected = GetCounter(selectedUnitIndex, selectedProvinceNumber);
                if (selected != null) selected.GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;
            }

            barState = 0;
            maxUnitsNumber = (int)GameManager.Instance.humanPlayer.stats.movementPoints.value;
            selectedUnitIndex = index;
            selectedProvinceNumber = provinceNumber;

            Transform transform = GetCounter(selectedUnitIndex, selectedProvinceNumber);
            if (transform != null) transform.GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;


            if (GetProvinceStats(provinceNumber).units.ContainsKey(index) && GetProvinceStats(provinceNumber).units[index] < maxUnitsNumber)
                maxUnitsNumber = GetProvinceStats(provinceNumber).units[index];

            slider.maxValue = maxUnitsNumber;
            unitsNumber = 0;
            UpdateRecruitUI();
            ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
            selectedUnitIndex = index;
            SetSelectionNumberUnits(true);
            UIManager.Instance.OpenUIWindow("SelectionNumberUnits", 0);
        }
        else
        {
            Alert.Instance.OpenAlert("no movement points!");
        }
    }
    public void SelectUnitToRecruit(int index)
    {
        UnitStats unitStats = GameAssets.Instance.unitStats[index];
        int price = unitStats.price;
        int priceMP = unitStats.movementPointsPrice;
        if (GameManager.Instance.humanPlayer.stats.cheaperRecruitment) price = price - 5;
        if (GameManager.Instance.humanPlayer.stats.movementRecruitment) priceMP = priceMP - 1;


        if (selectedProvince != null && GameManager.Instance.humanPlayer.stats.coins.CanAfford(price))
        {
            int population = (int)GetProvinceStats(selectedProvince).population.value;
            if (population > 0)
            {
                if (GameManager.Instance.humanPlayer.stats.warriors.CheckLimit(1))
                {
                    if (GameManager.Instance.humanPlayer.stats.movementPoints.value >= priceMP)
                    {
                        barState = 1;
                        if (selectedUnitIndex >= 0) GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
                        selectedUnitIndex = index;
                        GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.blueTexture;




                        int maxToRecruit = (int)GameManager.Instance.humanPlayer.stats.coins.value / price;
                        int value = math.min(maxToRecruit, population);
                        value = math.min(value, (int)(GameManager.Instance.humanPlayer.stats.movementPoints.value / priceMP));
                        value = math.min(value, GameManager.Instance.humanPlayer.stats.warriors.ToLimit());

                        maxUnitsNumber = value;
                        slider.maxValue = maxUnitsNumber;

                        unitsNumber = 0;
                        UpdateRecruitUI();
                        ProvinceStats provinceStats = GetProvinceStats(selectedProvince);
                        selectedUnitIndex = index;

                        SetSelectionNumberUnits(false);
                        UIManager.Instance.OpenUIWindow("SelectionNumberUnits", 0);
                    }
                    else
                    {
                        Alert.Instance.OpenAlert("no movement points!");
                    }
                }
                else
                {
                    Alert.Instance.OpenAlert("number of warriors has reached the limit!");
                }
            }
            else
            {
                Alert.Instance.OpenAlert("No population in the province!");
            }
        }
        else
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
            provinceStats.population.Subtract(unitsNumber);
            
            UnitStats unitStats = GameAssets.Instance.unitStats[selectedUnitIndex];
            int price = unitStats.price;
            int priceMP = unitStats.movementPointsPrice;
            if (GameManager.Instance.humanPlayer.stats.cheaperRecruitment) price = price - 5;
            if (GameManager.Instance.humanPlayer.stats.movementRecruitment) priceMP = priceMP - 1;

            GameManager.Instance.humanPlayer.stats.coins.Subtract(unitsNumber * price);
            GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(unitsNumber * priceMP);
            GameManager.Instance.humanPlayer.stats.warriors.Add(unitsNumber);


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

            UIManager.Instance.OpenUIWindow("ProvinceStats", int.Parse(selectedProvince.name));
            GameAssets.Instance.recruitUnitContentUI.GetChild(selectedUnitIndex).GetComponent<Image>().sprite = GameAssets.Instance.brownTexture;
            UIManager.Instance.CloseUIWindow("SelectionNumberUnits");
            UIManager.Instance.UpdateCounters();
        }
    }
    public void Move()
    {
        if (selectedNeighbor != null && selectedProvince != null && unitsNumber > 0)
        {
            ProvinceStats provinceStats1 = GetProvinceStats(selectedProvince);
            ProvinceStats provinceStats2 = GetProvinceStats(selectedNeighbor);

            GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(unitsNumber);

            if (selectedProvinceNumber == 1)
            {
                MoveTo(provinceStats1, provinceStats2);
            }
            else if (selectedProvinceNumber == 2)
            {
                MoveTo(provinceStats2, provinceStats1);
            }

            UpdateUnitNumber(selectedProvince);
            UpdateUnitNumber(selectedNeighbor);
            UIManager.Instance.CloseUIWindow("SelectionNumberUnits");
            UIManager.Instance.LoadUnitsMove(int.Parse(selectedProvince.name), int.Parse(selectedNeighbor.name), true);
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

        if (unitsNumber > 0 && from.provinceOwnerIndex != to.provinceOwnerIndex) //&& to.provinceOwnerIndex == -1)
        {
            to.SetNewOwner(from.provinceOwnerIndex);
            if(to.chest) UIManager.Instance.OpenChest(to.index);
            GameManager.Instance.UpdateUnitCounter(to.index);
          //  ChangeProvinceColor(map.GetChild(to.index).GetComponent<SpriteRenderer>(), GameManager.Instance.GetPlayerColor(from.provinceOwnerIndex));
        }
        UIManager.Instance.OpenUIWindow("ProvinceStats", to.index);
    }
    public void UpdateUnitNumber(Transform province)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[int.Parse(province.name)];
        int number = provinceStats.unitsCounter;
       

        if (province.childCount == 0 && number > 0)
        {
            Instantiate(GameAssets.Instance.unitCounter, province.transform.position - new Vector3(0, 0.05f, 0), Quaternion.identity, province);
        }
        else if (number == 0 && province.childCount > 0)
        {
            Destroy(province.GetChild(0).gameObject);
            return;
        }
        if (province.childCount > 0) province.GetChild(0).GetComponentInChildren<TextMeshPro>().text = number.ToString();

        if(GameManager.Instance.warFog)
        {
            bool value = GameManager.Instance.CanBeShow(int.Parse(province.name));
            if (province.childCount > 0)
            {
                province.GetChild(0).gameObject.SetActive(value);
            }

            if(provinceStats.provinceOwnerIndex == -1)
            {
                if (value) ChangeProvinceColor(province.GetComponent<SpriteRenderer>(), GameManager.Instance.neighborColor);
                else ChangeProvinceColor(province.GetComponent<SpriteRenderer>(), GameManager.Instance.fogColor);
            }
            else
            {
                if(value) ChangeProvinceColor(province.GetComponent<SpriteRenderer>(), GameManager.Instance.GetPlayerColor(provinceStats.provinceOwnerIndex));
                else ChangeProvinceColor(province.GetComponent<SpriteRenderer>(), GameManager.Instance.fogColor);
            }
        }

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
            if (GameManager.Instance.humanPlayer.stats.movementPoints.CanAfford(provinceStatsFrom.unitsCounter))
            {
                if (provinceStatsFrom.unitsCounter > 0)
                {
                    Sounds.instance.PlaySound(7);
                    GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(provinceStatsFrom.unitsCounter);
                    for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
                    {
                        selectedUnitIndex = i;
                        if (provinceStatsFrom.units.ContainsKey(selectedUnitIndex))
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
                else
                {
                    Alert.Instance.OpenAlert("no units!");
                }
            }
            else
            {
                Alert.Instance.OpenAlert("no movement points!");
            }
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
            if (GameManager.Instance.humanPlayer.stats.movementPoints.CanAfford(provinceStatsFrom.unitsCounter / 2))
            {
                if (provinceStatsFrom.unitsCounter / 2 > 0)
                {
                    Sounds.instance.PlaySound(7);
                    GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(provinceStatsFrom.unitsCounter / 2);
                    for (int i = 0; i < GameAssets.Instance.unitStats.Length; i++)
                    {
                        selectedUnitIndex = i;
                        if (provinceStatsFrom.units.ContainsKey(selectedUnitIndex))
                        {
                            int units = provinceStatsFrom.units[selectedUnitIndex];

                            if (units % 2 == 1)
                            {
                                b++;
                            }

                            if (b > 0 && (numberFrom + numberTo) / 2 == 0 && units > units / 2 + b)
                            {
                                unitsNumber = units / 2 + (b / 2);
                                b = 0;
                            }
                            else if (b > 1)
                            {
                                unitsNumber = units / 2 + (b / 2);
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
                else
                {
                    Alert.Instance.OpenAlert("No units!");
                }
            }
            else
            {
                Alert.Instance.OpenAlert("no movement points!");
            }
        }
    }
    private ProvinceStats GetProvinceStats(Transform province)
    {
        return GameManager.Instance.provinces[int.Parse(province.name)];
    }
    public void AIRecruit(int provinceIndex, int unitIndex, int unitsNumber, PlayerStats playerStats)
    {
        ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
        if (playerStats.warriors.CheckLimit(unitsNumber) && playerStats.movementPoints.CanAfford(unitsNumber * GameAssets.Instance.unitStats[unitIndex].movementPointsPrice) &&
            playerStats.coins.CanAfford(unitsNumber * GameAssets.Instance.unitStats[unitIndex].price) && provinceStats.population.CanAfford(unitsNumber))
        {
            provinceStats.unitsCounter += unitsNumber;
            provinceStats.population.Subtract(unitsNumber);
            playerStats.coins.Subtract(unitsNumber * GameAssets.Instance.unitStats[unitIndex].price);
            playerStats.movementPoints.Subtract(unitsNumber * GameAssets.Instance.unitStats[unitIndex].movementPointsPrice);
            playerStats.warriors.Add(unitsNumber);
            if (provinceStats.units == null) provinceStats.units = new Dictionary<int, int>();
            if (provinceStats.units.ContainsKey(unitIndex))
            {
                provinceStats.units[unitIndex] += unitsNumber;
            }
            else
            {
                provinceStats.units.Add(unitIndex, unitsNumber);
            }
            Transform provinceTransform = map.GetChild(provinceIndex);
            if (provinceTransform.childCount == 0)
            {
                Instantiate(GameAssets.Instance.unitCounter, provinceTransform.position - new Vector3(0, 0.05f, 0), Quaternion.identity, provinceTransform);
            }
            provinceTransform.GetChild(0).GetComponentInChildren<TextMeshPro>().text = provinceStats.unitsCounter.ToString();
        }
    }
    public bool AIRecruitArray(int provinceIndex, int[] array, PlayerStats playerStats)
    {
        if (provinceIndex < GameManager.Instance.provinces.Length && provinceIndex >= 0)
        {
            ProvinceStats provinceStats = GameManager.Instance.provinces[provinceIndex];
            int unitsNumber, price, movementPoints;
            array = ArrayCount(array, out unitsNumber, out movementPoints, out price, playerStats, provinceStats);

            provinceStats.unitsCounter += unitsNumber;
            provinceStats.population.Subtract(unitsNumber);
            playerStats.coins.Subtract(price);
            playerStats.movementPoints.Subtract(movementPoints);
            playerStats.warriors.Add(unitsNumber);
            if (provinceStats.units == null) provinceStats.units = new Dictionary<int, int>();

            for (int i = 0; i < array.Length; i++)
            {
                if (provinceStats.units.ContainsKey(i))
                {
                    provinceStats.units[i] += array[i];
                }
                else
                {
                    provinceStats.units.Add(i, array[i]);
                }
            }


            if(unitsNumber > 0)
            {
                GameManager.Instance.UpdateUnitCounter(provinceIndex);
            }
            
          /*  Transform provinceTransform = map.GetChild(provinceIndex);
            if (provinceTransform.childCount == 0 && unitsNumber > 0)
            {
                Instantiate(GameAssets.Instance.unitCounter, provinceTransform.position - new Vector3(0, 0.05f, 0), Quaternion.identity, provinceTransform);
            }

            if (unitsNumber > 0) provinceTransform.GetChild(0).GetComponentInChildren<TextMeshPro>().text = provinceStats.unitsCounter.ToString();
          */

            if (unitsNumber == 0) return false;
            else return true;
        }
        return false;
    }
    private int[] ArrayCount(int[] array, out int unitsNumber, out int mPoints, out int price, PlayerStats playerStats, ProvinceStats provinceStats)
    {
        int movementPoits = (int)playerStats.movementPoints.value;
        int warriors = playerStats.warriors.ToLimit();
        int coins = (int)playerStats.coins.value;
        int population = (int)provinceStats.population.value;

        mPoints = 0;
        unitsNumber = 0;
        price = 0;

        for (int i = 0; i < array.Length; i++)
        {
            int max = array[i];
            UnitStats stats = GameAssets.Instance.unitStats[i];

            int priceUnit = stats.price;
            int priceMP = stats.movementPointsPrice;
            if (playerStats.cheaperRecruitment) price = price - 5;
            if (playerStats.movementRecruitment) priceMP = priceMP - 1;

            int value = movementPoits / priceMP;
            if (value < array[i])
            {
                if (value < max) max = value;
            }
            movementPoits -= priceMP * max;

            value = warriors;
            if (value < array[i])
            {
                if (value < max) max = value;
            }
            warriors -= max;

            value = coins / priceUnit;
            if (value < array[i])
            {
                if (value < max) max = value;
            }
            coins -= priceUnit * max;

            value = population;
            if (value < array[i])
            {
                if (value < max) max = value;
            }
            population -= max;

            array[i] = max;
            mPoints += array[i] * priceMP;
            unitsNumber += array[i];
            price += array[i] * priceUnit;
        }

        return array;
    }
    public void AIMove(int fromIndex, int toIndex, int unitsNumber, int unitIndex)
    {
        ProvinceStats from = GameManager.Instance.provinces[fromIndex];
        ProvinceStats to = GameManager.Instance.provinces[toIndex];
        if (from.unitsCounter >= unitsNumber)
        {
            from.unitsCounter -= unitsNumber;
            from.units[unitIndex] -= unitsNumber;
            to.unitsCounter += unitsNumber;

            if (to.units != null)
            {
                if (to.units.ContainsKey(unitIndex))
                    to.units[unitIndex] += unitsNumber;
                else
                    to.units.Add(unitIndex, unitsNumber);
            }
            else
            {
                to.units = new Dictionary<int, int>();
                to.units.Add(unitIndex, unitsNumber);
            }

            if (unitsNumber > 0 && from.provinceOwnerIndex != to.provinceOwnerIndex) //&& to.provinceOwnerIndex == -1)
            {
                to.SetNewOwner(from.provinceOwnerIndex);
                // ChangeProvinceColor(map.GetChild(to.index).GetComponent<SpriteRenderer>(), GameManager.Instance.GetPlayerColor(from.provinceOwnerIndex));
                GameManager.Instance.UpdateUnitCounter(to.index);
            }
            GameManager.Instance.humanPlayer.stats.movementPoints.Subtract(unitsNumber);
            UpdateUnitNumber(map.GetChild(fromIndex));
            UpdateUnitNumber(map.GetChild(toIndex));
        }
    }

    public void AIMoveArray(int[] array, int fromIndex, int toIndex)
    {
        ProvinceStats from = GameManager.Instance.provinces[fromIndex];
        ProvinceStats to = GameManager.Instance.provinces[toIndex];

        int value = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > 0)
            {
                int unitsNumber = array[i];
                value += unitsNumber;
                int unitIndex = i;
                


                from.unitsCounter -= unitsNumber;
                from.units[unitIndex] -= unitsNumber;
                to.unitsCounter += unitsNumber;


                if (to.units != null)
                {
                    if (to.units.ContainsKey(unitIndex))
                        to.units[unitIndex] += unitsNumber;
                    else
                        to.units.Add(unitIndex, unitsNumber);
                }
                else
                {
                    to.units = new Dictionary<int, int>();
                    to.units.Add(unitIndex, unitsNumber);
                }
            }
        }

        if (value > 0 && from.provinceOwnerIndex != to.provinceOwnerIndex) //&& to.provinceOwnerIndex == -1)
        {
            to.SetNewOwner(from.provinceOwnerIndex);
            //  ChangeProvinceColor(map.GetChild(to.index).GetComponent<SpriteRenderer>(), GameManager.Instance.GetPlayerColor(from.provinceOwnerIndex));
            GameManager.Instance.UpdateUnitCounter(to.index);
        }
        GameManager.Instance.GetPlayerStats(from.provinceOwnerIndex).movementPoints.Subtract(value);
        UpdateUnitNumber(map.GetChild(fromIndex));
        UpdateUnitNumber(map.GetChild(toIndex));
    }
    public void AutoBattle(bool isPlayer, int aggressorProvinceIndex, int defenderProvinceIndex)
    {
        bool isOpenChest = false;
        ProvinceStats aggressor = GameManager.Instance.provinces[aggressorProvinceIndex];
        ProvinceStats defender = GameManager.Instance.provinces[defenderProvinceIndex];
        if (aggressor.unitsCounter > 0)
        {
            float aggressorPower = 0;
            float defenderPower = 0;
            int unitsNumber = GameAssets.Instance.unitStats.Length;
            if (defender.units != null)
            {
                for (int i = 0; i < unitsNumber; i++)
                {
                    float unitValue = GameAssets.Instance.unitStats[i].battleValue;

                    if (aggressor.units.ContainsKey(i))
                    {
                        aggressorPower += unitValue * aggressor.units[i];
                    }

                    if (defender.units.ContainsKey(i))
                    {
                        defenderPower += unitValue * defender.units[i];
                    }
                }
            }
            else
            {
                defenderPower = 0;
                for (int i = 0; i < unitsNumber; i++)
                {
                    float unitValue = GameAssets.Instance.unitStats[i].battleValue;
                    if (aggressor.units.ContainsKey(i))
                    {
                        aggressorPower += unitValue * aggressor.units[i];
                    }
                }
            }
            ProvinceStats winner;
            ProvinceStats loser;
            float value;
            if (aggressorPower > defenderPower)
            {
                winner = aggressor;
                value = defenderPower;
                loser = defender;
            }
            else
            {
                winner = defender;
                value = aggressorPower;
                loser = aggressor;
            }
            if (loser.provinceOwnerIndex == 0)
            {
                GameManager.Instance.humanPlayer.stats.warriors.value -= loser.unitsCounter;
            }
            else if (loser.provinceOwnerIndex > 0)
            {
                GameManager.Instance.botsList[loser.provinceOwnerIndex - 1].stats.warriors.value -= loser.unitsCounter;
            }

            loser.unitsCounter = 0;
            if (loser.units != null)
            {
                for (int i = 0; i < unitsNumber; i++)
                {
                    if (loser.units.ContainsKey(i))
                    {
                        loser.units[i] = 0;
                    }
                }
            }
            List<float3> list = new List<float3>();
            for (int i = 0; i < unitsNumber; i++)
            {
                if (winner.units.ContainsKey(i) && winner.units[i] > 0)
                {
                    float3 f = new float3(i, winner.units[i], GameAssets.Instance.unitStats[i].battleValue);
                    list.Add(f);
                }
            }

            while (value > 0)
            {
                if (value > 0.5f)
                {
                    int index = UnityEngine.Random.Range(0, list.Count);
                    value -= list[index].z;
                    float3 v = list[index];
                    v.y--;
                    list[index] = v;
                    if (v.y <= 0)
                    {
                        list.RemoveAt(index);
                    }
                }
                else value = 0f;
            }
            int number = 0;

            for (int i = 0; i < unitsNumber; i++)
            {
                if (winner.units.ContainsKey(i))
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (i == (int)list[j].x)
                        {
                            winner.units[i] = (int)list[j].y;
                            number += (int)list[j].y;
                            break;
                        }
                        if (j + 1 == list.Count)
                        {
                            winner.units[i] = 0;
                        }
                    }
                    if (list.Count == 0) winner.units[i] = 0;
                }
            }
            if (winner.provinceOwnerIndex == 0)
            {
                GameManager.Instance.humanPlayer.stats.warriors.value -= winner.unitsCounter - number;
            }
            else if (winner.provinceOwnerIndex > 0)
            {
                GameManager.Instance.botsList[winner.provinceOwnerIndex - 1].stats.warriors.value -= winner.unitsCounter - number;
            }
            winner.unitsCounter = number;
            if (winner.index == aggressorProvinceIndex)
            {
                Debug.Log(winner.provinceOwnerIndex);
                loser.SetNewOwner(winner.provinceOwnerIndex);
                if (loser.chest)
                {
                    if (winner.provinceOwnerIndex != 0)
                    {
                        Chest.OpenChest(GameManager.Instance.GetPlayerStats(winner.provinceOwnerIndex));
                        GameManager.Instance.ClearChest(loser.index);
                    }
                    else
                    {
                        isOpenChest = true;
                        UIManager.Instance.OpenChest(loser.index);
                    }
                }
              //  ChangeProvinceColor(map.GetChild(loser.index).GetComponent<SpriteRenderer>(), GameManager.Instance.GetPlayerColor(winner.provinceOwnerIndex));
                GameManager.Instance.UpdateUnitCounter(loser.index);
            }


        }
        UpdateUnitNumber(map.GetChild(aggressorProvinceIndex));
        UpdateUnitNumber(map.GetChild(defenderProvinceIndex));
        GameManager.Instance.UpdateBotDebuger();
        if (isPlayer)
        {
            UIManager.Instance.UpdateCounters();
            if(!isOpenChest)
            { 
              UIManager.Instance.CloseUIWindow("ProvinceStats");
              UIManager.Instance.CloseUIWindow("Battle");
            }
        }
        Sounds.instance.PlaySound(8);
        SetBattleIcon(aggressorProvinceIndex, defenderProvinceIndex);
    }


    private void SetBattleIcon(int province1, int province2)
    {
        Vector3 vector1 = map.GetChild(province1).transform.position;
        Vector3 vector2 = map.GetChild(province2).transform.position;
        battleIcon.gameObject.SetActive(true);
        battleIcon.transform.position = new Vector3((vector1.x + vector2.x) / 2, (vector1.y + vector2.y) / 2, 0);
        SetTimer(1f);
    }

    private void SetTimer(float timer)
    {
        time = timer;
        currentTime = 0;
    }




}
