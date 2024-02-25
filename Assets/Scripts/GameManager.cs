
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	const string mapsPath = "Assets/Resources/Maps/";
	public string currentMap = "World";
	public string saveName;

	[SerializeField]float adTime;
    [SerializeField] public float timer = 0;

    [SerializeField] public Color fogColor;
    [SerializeField] public Color neighborColor;


    public bool ready;
	public bool readyToNextTurn;

	public static GameManager Instance;
	public CameraController cameraController;

	public ProvinceStats[] provinces;
	public int numberOfProvinces;
	public List<Player> botsList;
	private Transform players;

	public Player humanPlayer;


	public Transform chests;
	public Transform map;
	public Transform buildings;
	public SelectingProvinces selectingProvinces;
	public PathFinding pathFinding;

	private int yourProvinceIndex;
	private int enemyProvinceIndex;
	public bool youAttack { private set; get; }

	public int turn { private set; get; } = 0;

	public bool isPlaying;
	public bool toLoad;

	public Action load;
	private bool isChest;

	public Action<int> updateReward;
	public Action showAD;

	public int lastPlayer;


    bool iswin;
    public bool warFog {private set; get; }

	private void Awake()
	{
		warFog = false;
		if (Instance == null)
		{
			Instance = this;
			isPlaying = true;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	private void Start()
	{
        Time.timeScale = 1;
        if (!toLoad && isPlaying && SceneManager.GetActiveScene().buildIndex == 2)
		{
			isPlaying = false;
			lastPlayer = -1;
			SetUp();
			iswin = false;
        }
	}

	int indexbot = 0;
	private void Update()
	{
		if (showAD != null)
		{
			timer = timer + Time.deltaTime;
			if (timer >= adTime)
			{
				timer = 0;
				showAD();
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			indexbot = 0;
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			indexbot = 1;
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			indexbot = 2;
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			indexbot = 3;
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			Debug.Log((indexbot + 1) + " - " + botsList[indexbot].stats.coins.GetDetails());
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			Debug.Log((indexbot + 1) + " - " + botsList[indexbot].stats.developmentPoints.GetDetails());
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			Debug.Log((indexbot + 1) + " - " + botsList[indexbot].stats.warriors.GetDetails());
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			Debug.Log((indexbot + 1) + " - " + botsList[indexbot].stats.movementPoints.GetDetails());
		}
	}

	private void SetCamera()
	{
		for (int i = 0; i < provinces.Length; i++)
		{
			if (provinces[i].provinceOwnerIndex == 0)
			{
                Vector3 vector3 = map.GetChild(provinces[i].index).transform.position;
                vector3.z = Camera.main.transform.position.z;
                Camera.main.transform.position = vector3;
            }
	}
}

private void OnLevelWasLoaded(int level)
	{
        if (level == 2 && Instance == this)
		{
            LoadMap(currentMap);
            if (isPlaying)
			{
                isPlaying = false;
                SetUp();
                if(toLoad)
                {
                    load();
                    toLoad = false;
                }
            }
			else
			{
				UpdateMap();

                if (isChest)
                {
                    UIManager.Instance.OpenChest(-1);
					isChest= false;
                }
            }
            SetCamera();
			ready = true;
            StartCoroutine(ContinueTurn());
        }
        else if(level ==0)
		{
			isPlaying = true;
		    if(players !=null)Destroy(players.gameObject);
		}
	}
	private void LoadMap(string name)
	{
        GameObject obj = Resources.Load("Maps/" + name + "/Map") as GameObject;
		Texture2D[] sprites = Resources.LoadAll<Texture2D>("Maps/" + name + "/Sprites");
		Texture2D texture2D = Resources.Load<Texture2D>("Texture/" + name);
        Camera.main.GetComponent<CameraController>().Limit = new Vector3(texture2D.width / 100 + 1f, texture2D.height / 100 + 1f);

	    if(isPlaying) turn = 0;
        obj = Instantiate(obj);
		map = obj.transform;
		foreach (Texture2D sprite in sprites)
		{
			SpriteRenderer spriteRenderer = obj.transform.GetChild(int.Parse(sprite.name)).GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = Sprite.Create(sprite, new Rect(0, 0, sprite.width, sprite.height), new Vector2(0.5f, 0.5f));
		}
	}
	private void SetUp()
	{
        GameAssets.Instance.SetUp();
		players = GameObject.FindGameObjectWithTag("Players").transform;
		DontDestroyOnLoad(players);

        map = GameObject.FindGameObjectWithTag("GameMap").transform;
		buildings = GameObject.FindGameObjectWithTag("Buildings").transform;
		chests = GameObject.FindGameObjectWithTag("Chests").transform;

        selectingProvinces = FindObjectOfType<SelectingProvinces>();
		botsList.Clear();
		CreateHumanPlayer();

		MapStats mapStats = Resources.Load<MapStats>("Maps/" + currentMap + "/MapStats");

		LoadBots(mapStats.players);

        ProvinceStats[] array = mapStats.provinces;
		numberOfProvinces = mapStats.numberOfProvinces;

		provinces = new ProvinceStats[array.Length];


		for (int i = 0; i < array.Length; i++)
		{
			provinces[i] = new ProvinceStats();
			provinces[i].CopyData(array[i],!toLoad);
			ProvinceStats provinceStats = provinces[i];
			if (provinceStats.provinceOwnerIndex != -1)
			{
			  	UpdateUnitCounter(i);
				//selectingProvinces.ChangeProvinceColor(map.GetChild(i).GetComponent<SpriteRenderer>(), GetPlayerColor(provinceStats.provinceOwnerIndex));
			}
		}

		List<int> list = new List<int>();
		for(int i = 0; i < provinces.Length; i++)
		{
			UpdateUnitCounter(i);
			if (!provinces[i].isSea)list.Add(i);
		}

		for (int i = 0; i < mapStats.players; i++)
		{
			int value = UnityEngine.Random.Range(0, list.Count);
			value = list[value];
            provinces[value].provinceOwnerIndex = i;
			provinces[value].Clear();
			if (list.Contains(value)) list.Remove(value);

			for (int j = 0; j < provinces[value].neighbors.Count; j++)
			{
				int index = provinces[value].neighbors[j];
                if (list.Contains(index)) list.Remove(index);

                for (int k = 0; k < provinces[index].neighbors.Count;k++)
                {
                    int index2 = provinces[index].neighbors[k]; 
                    if (list.Contains(index2)) list.Remove(index2);
                }
            }
        //    UpdateUnitCounter(value);
        //    selectingProvinces.ChangeProvinceColor(map.GetChild(value).GetComponent<SpriteRenderer>(), GetPlayerColor(provinces[value].provinceOwnerIndex));
            if (i == 0)
			{
        //        selectingProvinces.ChangeProvinceColor(map.GetChild(value).GetComponent<SpriteRenderer>(), GetPlayerColor(provinces[value].provinceOwnerIndex));
                UpdateNeighbors(value);
			}
		}
		
        UpdateBotProvinces();
		pathFinding = new PathFinding();

		ready = true;
		readyToNextTurn = true;
		cameraController = Camera.main.GetComponent<CameraController>();
		for (int i = 0; i < provinces.Length; i++)
		{
			ProvinceStats provinceStats = provinces[i];
			if (provinceStats.buildingIndex != -1)
			{
				Transform province = map.GetChild(provinceStats.index).transform;
				Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
				transform.position = province.position + new Vector3(0, 0.08f, 0);
				transform.parent = buildings;
				transform.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.buildingsStats[provinceStats.buildingIndex].icon;
				transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
                BonusManager.SetBonus(provinceStats, provinceStats.buildingIndex);
            }

			if(provinceStats.chest)
			{
                Transform province = map.GetChild(provinceStats.index).transform;
                Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
                transform.position = province.position + new Vector3(0, 0.08f, 0);
                transform.parent = chests;
                transform.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.chest;
                transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }

			if(provinceStats.provinceOwnerIndex == 0)
			{
				Vector3 vector3 = map.GetChild(provinceStats.index).transform.position;
				vector3.z = Camera.main.transform.position.z;
				Camera.main.transform.position = vector3;
			}
        }

		for (int i = 0; i < botsList.Count; i++)
		{


            int index = botsList[i].stats.texesIndex;
            GameManager.Instance.GetValuesByTaxesIndex(index, out float coins, out float people);
            botsList[i].stats.ChangeCoinsMultiplier(coins);

            index = botsList[i].stats.researchIndex;
            GameManager.Instance.GetValuesByResearchIndex(index, out float coinsIncome, out float development);
            botsList[i].stats.ChangeDevelopmentCoinsMultiplier(coinsIncome);
            botsList[i].stats.ChangeDevelopmentMultiplier(development);


        }


        for (int i = 0; i < provinces.Length; i++)
        {
            UpdateUnitCounter(i);
        }
        BonusManager.UpdateLimits(0);

		Time.timeScale = 1.0f;

		UIManager.Instance.SetUp();
		humanPlayer.stats.movementPoints.Set(humanPlayer.stats.movementPoints.limit);
		UIManager.Instance.UpdateCounters();


    }

	private void LoadBots(int number)
	{
		Dictionary<string, Color> colors = GetColors();
		
		foreach (var item in colors)
		{
			if(number > 1)
			{
				AddBot(item.Key,true,item.Value,200,botsList.Count + 1);
				number--;
			}
		}
	}

	private Dictionary<string, Color> GetColors()
	{
		Dictionary<string, Color> diction = new Dictionary<string, Color>();
		diction.Add("Yellow", Color.yellow);
		diction.Add("Green", Color.green);
		diction.Add("Red", Color.red);
		diction.Add("Cyan", Color.cyan);
		diction.Add("Magenta", Color.magenta);
		diction.Add("Brown", new Color(0.588f, 0.294f, 0));
		diction.Add("Dark green", new Color (0.107f,0.6043f,0.385f));
        return diction;
    }

	private void AddBot(string name, bool isComputer, Color color, int startCoins, int index)
	{
		Player player = new GameObject(name, typeof(Player)).GetComponent<Player>();
		player.transform.parent = players;
		player.SetUp(name, isComputer, color, startCoins, index);
		botsList.Add(player);
	}

	private void CreateHumanPlayer()
	{
        if (humanPlayer != null) Destroy(humanPlayer.gameObject);
        Player player = new GameObject("Human", typeof(Player)).GetComponent<Player>();
		player.transform.parent = players;
		player.SetUp("Player", false, Color.blue, 200, 0);
		humanPlayer = player;
	}
	private void UpdateBotProvinces()
	{
		foreach (Player item in botsList)
		{
			item.UpdateProvinces();
		}
	}

	public void UpdateUnitCounter(int index)
	{
	 if(map != null)	selectingProvinces.UpdateUnitNumber(map.GetChild(index).transform);
	}
	public void Battle(int yourProvinceIndex, int enemyProvinceIndex, bool youAttack)
	{
		this.youAttack = youAttack;
		this.yourProvinceIndex = yourProvinceIndex;
		this.enemyProvinceIndex = enemyProvinceIndex;
		SceneManager.LoadScene(1);
	}
	public void GetUnits(out Dictionary<int, int> yourUnits, out Dictionary<int, int> enemyUnits)
	{
		yourUnits = provinces[yourProvinceIndex].units;
		enemyUnits = provinces[enemyProvinceIndex].units;
	}
	public void GetProvinceHP(out int yourProvinceHP, out int enemyProvinceHP)
	{
		if (youAttack)
		{
			yourProvinceHP = 10;
			enemyProvinceHP = (int)provinces[enemyProvinceIndex].lifePoints.value;
		}
		else
		{
			yourProvinceHP = (int)provinces[yourProvinceIndex].lifePoints.value;
			enemyProvinceHP = 10;
		}

	}
	public void SetUnitsConters(int your, int enemy)
	{
		GameManager.Instance.humanPlayer.stats.warriors.Subtract(provinces[yourProvinceIndex].unitsCounter - your);
		ProvinceStats province = provinces[enemyProvinceIndex];
		if (province.provinceOwnerIndex > 0)
		{
			botsList[province.provinceOwnerIndex - 1].stats.warriors.Subtract(provinces[enemyProvinceIndex].unitsCounter - enemy);
		}

		provinces[yourProvinceIndex].unitsCounter = your;
		provinces[enemyProvinceIndex].unitsCounter = enemy;
	}
	public void SetBattleResult(bool isWin)
	{
		if (isWin)
		{
			ProvinceStats provinceStats = provinces[enemyProvinceIndex];
			provinceStats.SetNewOwner(0);
			provinceStats.units = new Dictionary<int, int>();
			provinceStats.unitsCounter = 0;
			if (provinceStats.chest)
			{
				Debug.Log("sklkko");
				provinceStats.chest = false;
				isChest = true;
			}
			Debug.Log("simea");
        }
		else
		{

		}
	}
	public void UpdateMap()
	{
		GameAssets.Instance.SetUp();
		cameraController = Camera.main.GetComponent<CameraController>();
		players = GameObject.FindGameObjectWithTag("Players").transform;
		humanPlayer = players.GetChild(0).GetComponent<Player>();
		for (int i = 0; i < botsList.Count; i++)
		{
			botsList[i] = players.GetChild(i + 1).GetComponent<Player>();
		}


		map = GameObject.FindGameObjectWithTag("GameMap").transform;
		buildings = GameObject.FindGameObjectWithTag("Buildings").transform;
        chests = GameObject.FindGameObjectWithTag("Chests").transform;

        selectingProvinces = FindObjectOfType<SelectingProvinces>();
		humanPlayer.stats.movementPoints.limit = 30;

		humanPlayer.stats.movementPoints.UpdateLimit();
		humanPlayer.stats.warriors.UpdateLimit();



		for (int i = chests.childCount - 1; i >= 0; i--)
		{
			Destroy(chests.GetChild(i).gameObject);
        }

        for (int i = buildings.childCount - 1; i >= 0; i--)
        {
            Destroy(buildings.GetChild(i).gameObject);
        }

        for (int i = 0; i < provinces.Length; i++)
		{ 
			ProvinceStats provinceStats = provinces[i];
			if (provinceStats.provinceOwnerIndex != -1)
			{
				selectingProvinces.ChangeProvinceColor(map.GetChild(i).GetComponent<SpriteRenderer>(), GetPlayerColor(provinceStats.provinceOwnerIndex));
			}
			else
			{
				selectingProvinces.ChangeProvinceColor(map.GetChild(i).GetComponent<SpriteRenderer>(), new Color32(48, 48, 48, 255));

			}

			if (provinceStats.buildingIndex != -1)
			{
				Transform province = map.GetChild(provinceStats.index).transform;
				Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
				transform.position = province.position + new Vector3(0, 0.08f, 0);
				transform.parent = buildings;
				transform.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.buildingsStats[provinceStats.buildingIndex].icon;
				transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
			}

            if (provinceStats.chest)
            {
                Transform province = map.GetChild(provinceStats.index).transform;
                Transform transform = new GameObject(province.name, typeof(SpriteRenderer)).transform;
                transform.position = province.position + new Vector3(0, 0.08f, 0);
                transform.parent = chests;
                transform.GetComponent<SpriteRenderer>().sprite = GameAssets.Instance.chest;
                transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }

            UpdateUnitCounter(i);
		}

		GameAssets.Instance.SetUp();
		UIManager.Instance.SetUp();
		UIManager.Instance.UpdateTurnCounter();
	}
	public void NextTurn()
	{
        if (readyToNextTurn)
		{
            turn++;
			updateReward((int)humanPlayer.stats.coins.CountIncome());
			StartCoroutine(BotsNextTurn());


			/*
			string stats = startCoins + " <sprite index=21/>   ";
			if (coinsIncome >= 0) stats += "<color=green>+"+ coinsIncome +"</color>";
			else stats += "<color=red>" + coinsIncome + "</color>";

			stats += "\n";

			stats +=  startPopulation + " <sprite index=1/>   ";
			if (populationIncome >= 0) stats += "<color=green>+" + populationIncome + "</color>";
			else stats += "<color=red>" + populationIncome + "</color>";

			stats += "\n";

			stats += startDevelopmentPoints + " <sprite index=22/>   ";
			if (developmentPointsIncome >= 0) stats += "<color=green>+" + developmentPointsIncome + "</color>";
			else stats += "<color=red>" + developmentPointsIncome + "</color>";

			UIManager.Instance.OpenTurnDetails(stats);
			*/


			UIManager.Instance.UpdateCounters();
		}
	}


	public void ContinueT()
	{
		StartCoroutine(ContinueTurn());
	}
    IEnumerator ContinueTurn()
	{
		Debug.Log("contiune !!!!" + lastPlayer);
		if (lastPlayer > 0)
		{
			readyToNextTurn = false;
            UIManager.Instance.background.gameObject.SetActive(true);
            ready = true;

			for (int i = lastPlayer - 1; i < botsList.Count; i++)
			{
				Player bot = botsList[i];
				if (bot.isComputer)
				{
					yield return new WaitUntil(() => ready);
					if (lastPlayer == bot.index)
					{
						ready = false;
					}
					else
					{
						lastPlayer = bot.index;
						ready = false;
						PlayerStats playerStats = bot.stats;
                        UIManager.Instance.PrintPlayer(bot.playerColor, bot.playerName);
                        playerStats.movementPoints.Set(playerStats.movementPoints.limit);
						playerStats.developmentPoints.NextTurn();
						playerStats.coins.NextTurn();
					}

					bot.RunEnemyManager();
				}
			}
            lastPlayer = -1;
            UpdateStats();
            readyToNextTurn = true;
            UpdateBotDebuger();
            UIManager.Instance.background.gameObject.SetActive(false);
        }
    }

    IEnumerator BotsNextTurn()
	{
		readyToNextTurn = false;
        UIManager.Instance.background.gameObject.SetActive(true);
        foreach (Player bot in botsList)
		{
			if (bot.isComputer)
			{
				yield return new WaitUntil(() => ready);
				lastPlayer = bot.index;
				ready = false;
				PlayerStats playerStats = bot.stats;
                UIManager.Instance.PrintPlayer(bot.playerColor,bot.playerName);
                playerStats.movementPoints.Set(playerStats.movementPoints.limit);
				playerStats.developmentPoints.NextTurn();
				playerStats.coins.NextTurn();
				bot.RunEnemyManager();
			}
		}
		lastPlayer = -1;
		readyToNextTurn = true;

		UpdateStats();

        UpdateBotDebuger();
        UIManager.Instance.background.gameObject.SetActive(false);



        yield return 0;
	}


	private void UpdateStats()
	{

        humanPlayer.stats.movementPoints.Set(humanPlayer.stats.movementPoints.limit);

        float startDevelopmentPoints = (float)Math.Round(humanPlayer.stats.developmentPoints.value, 2);
        float developmentPointsIncome = (float)Math.Round(humanPlayer.stats.developmentPoints.NextTurn(), 2);

        float startCoins = (int)humanPlayer.stats.coins.value;
        float coinsIncome = humanPlayer.stats.coins.NextTurn();

        float startPopulation = humanPlayer.stats.GetPopulation();
        float populationIncome = 0;


        UIManager.Instance.UpdateTurnCounter();
        UIManager.Instance.CloseUIWindow("ProvinceStats");



        for (int i = 0; i < provinces.Length; i++)
        {
            float value = provinces[i].population.NextTurn();
            if (provinces[i].provinceOwnerIndex == 0) populationIncome += value;
        }


        UIManager.Instance.PrintPlayer(Color.white, "Next Turn");
        if (turn % 2 == 0) Save();
    }

	public void UpdateBotDebuger()
	{
		string debugtext = "";
		foreach (Player bot in botsList)
		{
			PlayerStats playerStats = bot.stats;

			debugtext += "<color=#" + GetPlayerColor(bot.index).ToHexString().Remove(6) + ">Player " + bot.index + " : <color=white>" + Icons.GetIcon("Coin") + playerStats.coins.ToString() + "   " + Icons.GetIcon("DevelopmentPoint") + playerStats.developmentPoints.ToString() +
			Icons.GetIcon("Warrior") + playerStats.warriors.ToString() + "\n";
		}
		UIManager.Instance.debugText.text = debugtext;
	}
	/// 0,1,2,3,4
	public void GetValuesByTaxesIndex(int index, out float coinsIncome, out float peopleIncome)
	{
		coinsIncome = 0;
		peopleIncome = 0;
		switch (index)
		{
			case 0:
				coinsIncome =  0.01f;
				peopleIncome = 0.025f;
				break;
			case 1:
				coinsIncome = 0.003f;
				peopleIncome = 0.015f;
				break;
			case 2:
				coinsIncome = 0.05f;
				peopleIncome = 0.01f;
				break;
			case 3:
				coinsIncome = 0.06f;
				peopleIncome = -0.01f;
				break;
			case 4:
				coinsIncome = 0.075f;
				peopleIncome = -0.05f;
				break;
		}
	}
	public void GetValuesByResearchIndex(int index, out float coinsIncome, out float developmentIncome)
	{
		coinsIncome = 0;
		developmentIncome = 0;
		switch (index)
		{
			case 0:
				coinsIncome = -0.005f;
				developmentIncome = 0.05f;
				break;
			case 1:
				coinsIncome = -0.0075f;
				developmentIncome = 0.075f;
				break;
			case 2:
				coinsIncome = -0.01f;
				developmentIncome = 0.1f;
				break;
			case 3:
				coinsIncome = -0.02f;
				developmentIncome = 0.12f;
				break;
			case 4:
				coinsIncome = -0.05f;
				developmentIncome = 0.15f;
				break;
		}
	}
	public Color GetPlayerColor(int playerIndex)
	{
		if (playerIndex == 0)
		{
			return humanPlayer.playerColor;
		}
		else if (playerIndex - 1 <= botsList.Count && playerIndex > 0)
		{
			return botsList[playerIndex - 1].playerColor;
		}
		return Color.gray;
	}
	public int GetEnemyIndex()
	{
		return provinces[enemyProvinceIndex].provinceOwnerIndex;
	}

	public void OpenMenu()
	{
		SceneManager.LoadScene(2);
	}



	public void Save()
	{
		Player[] players = new Player[botsList.Count + 1];
		players[0] = humanPlayer;
		for (int i = 0; i < botsList.Count; i++)
		{
			players[i + 1] = botsList[i];
		}
		GameData gameData = new GameData(provinces, players);
		SavesManager.Save(gameData);
	}

	public void Load(string name,int turn)
	{
		this.turn = turn;
		GameData gameData = SavesManager.Load(name);
		provinces = gameData.LoadProvinces();
		UpdateMap();
		PlayerData[] players = gameData.GetPlayers();

		if (humanPlayer != null) { Destroy(humanPlayer.gameObject); }

		Player player = new GameObject("Human", typeof(Player)).GetComponent<Player>();
		player.transform.parent = this.players;
		player.index = 0;
		player.name = players[0].playerName;
		player.isComputer = false;
		player.playerColor = GetColor(players[0].playerColor);
		
	    players[0].stats.ToPlayerStats(ref player.stats);
		humanPlayer = player;
		player = null;

		for (int i = 0; i < botsList.Count; i++)
		{;
			if (botsList[i] != null) Destroy(botsList[i].gameObject);
		}
		botsList.Clear();

		for (int i = 1; i < players.Length; i++)
		{
			player = new GameObject(name, typeof(Player)).GetComponent<Player>();
			player.transform.parent = this.players;
			player.index = players[i].index;
			player.name = players[i].playerName + "1" ;
			player.playerName = players[i].playerName;
			player.isComputer = true;
			player.playerColor = GetColor(players[i].playerColor);
            players[i].stats.ToPlayerStats(ref player.stats);
            player.EnemyManagerSetUp();
			botsList.Add(player);
		}


        UpdateBotDebuger();
		UIManager.Instance.UpdateCounters();
        UIManager.Instance.UpdateTurnCounter();
		UIManager.Instance.LoadSpells();
        Debug.Log("Loaded");
	}

	private Color GetColor(float[] rgba)
	{
		return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
	}
	public string GetName()
	{
		return currentMap + " Turn " + turn.ToString() + "  " + DateTime.Now.Ticks.ToString();
	}
	private string GetTurn(string fileName)
	{

		for (int i = 0; i < fileName.Length; i++)
		{

		}
		return "";
	}

	public void ClearChest(int index)
	{
		if(index >= 0) provinces[index].chest = false;
		for (int i = 0; i < chests.childCount; i++)
		{
			if(chests.GetChild(i).name == index.ToString())
			{
				Destroy(chests.GetChild(i).gameObject);
				return;
			}

		}
	}

    public PlayerStats GetPlayerStats(int index)
    {
		if (index == 0) return humanPlayer.stats;
		else if (index > 0 && index <= botsList.Count) return botsList[index - 1].stats;
		else return null;
    }

	

	public void StopNewTurn()
	{
		StopAllCoroutines();
	}

	public bool CanBeShow(int index)
	{
		ProvinceStats province = provinces[index];
		if (province.provinceOwnerIndex == 0) return true;
		for (int i = 0; i < province.neighbors.Count; i++)
		{ 
			ProvinceStats neighbor = provinces[province.neighbors[i]];
			if(neighbor.provinceOwnerIndex == 0)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateBuildings(bool value,int index)
	{
        Transform transform = null;
		ProvinceStats provinceStats = provinces[index];



        if (provinceStats.buildingIndex != -1)
		{
			for (int i = 0; i < buildings.childCount; i++)
			{
				if(buildings.GetChild(i).name == index.ToString())
				{
					transform = buildings.GetChild(i).transform;
					break;
				}
			}
		}
		else if (provinceStats.chest)
		{
            for (int i = 0; i < chests.childCount; i++)
            {
                if (chests.GetChild(i).name == index.ToString())
                {
                    transform = chests.GetChild(i).transform;
                    break;
                }
            }
        }

        if (transform != null)
        {
            transform.gameObject.SetActive(value);
        }
    }

    public void UpdateNeighbors(int index)
    {
		ProvinceStats province = provinces[index];
		foreach (int item in province.neighbors)
		{
			UpdateUnitCounter(item);
		}
    }
	public void CheckPLayer()
	{
		int[] array = CountProvinces();

		for (int i = 0; i < array.Length; i++)
		{
			if (i == 0 && array[i] == 0) Win();
			else if (i != 0 && array[i] != 0) return;
		}
		Win();
	}

	private void Win()
	{
		if (!iswin)
		{
			iswin = true;
			StopAllCoroutines();
			Time.timeScale = 0;
			UIManager.Instance.OpenUIWindow("Win", 0);
			UIManager.Instance.background.gameObject.SetActive(true);
			Transform window = UIManager.Instance.GetWindow("Win");

			int[] array = CountProvinces();

			if (array[0] > 0) window.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "you won";
			else window.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "you lost";

			int2[] sorted = new int2[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				sorted[i] = new int2(array[array.Length - 1 - i],array.Length- i - 1 );
			}
			Sort(sorted);

			string text = "";
			for (int i = array.Length - 1; i >= 0; i--)
			{
				int index = sorted[i].y;
				int numberPr = sorted[i].x;
				text += (array.Length - i).ToString() + ". ";
				if (index == 0)
				{
					if (numberPr != 0)
					{
						text += "<color=#" + humanPlayer.playerColor.ToHexString() + ">" + humanPlayer.playerName + " -</color> Provinces: " + numberPr + "\n";
					}
					else
					{
						text += "<color=#5a5a5a>" + humanPlayer.playerName + " - Provinces: " + numberPr + "</color>\n";
					}
				}
				else
				{
					if (numberPr != 0)
					{
						text += "<color=#" + GetPlayerColor(index).ToHexString() + ">" + botsList[index - 1].playerName + "</color> - Provinces: " + numberPr + "\n";
					}
					else
					{
						text += "<color=#5a5a5a>" + botsList[index - 1].playerName + " - Provinces: " + numberPr + "</color>\n";
					}
				}
			}

			int reward = CountVP(array);

			window.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + reward.ToString();
			int vp = PlayerPrefs.GetInt("VictoryPoints", 0);
			vp += reward;
			PlayerPrefs.SetInt("VictoryPoints", vp);

			window.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
			SavesManager.Delete();
		}
    }

	private int CountVP(int[] playersProvinces)
	{
		int rank = playersProvinces.Length;

		for (int i = 1; i < playersProvinces.Length; i++)
		{
			if (playersProvinces[i] == 0)
			{
				rank--;
			}
		}

		int vp = 0;

		if (rank == playersProvinces.Length)
		{
			vp = 2;
		}
		else
		{
			vp = 10 * (playersProvinces.Length - rank);
		}

		return vp;
	}

private int[] CountProvinces()
	{
		int[] array = new int[botsList.Count + 1];
		for (int i = 0; i < provinces.Length; i++)
		{
			if (provinces[i].provinceOwnerIndex != -1)
			{				
				array[provinces[i].provinceOwnerIndex]++;
			}
		}
		return array;
	}

    public void Sort(int2[] array)
    {
        int selected;
        for (int i = 0; i < array.Length - 1; i++)
        {
            selected = 0;
            for (int j = 1; j < array.Length; j++)
            {
                if (array[selected].x > array[j].x)
                {
                    int2 k = array[j];
                    array[j] = array[selected];
                    array[selected] = k;
                    selected = j;
                }
                else
                {
                    selected = j;
                }
            }
        }
    }



}




