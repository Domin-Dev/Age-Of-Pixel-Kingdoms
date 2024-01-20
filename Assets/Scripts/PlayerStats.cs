

using System;

[Serializable]
public class PlayerStats
{
    public Statistic coins;
    public Statistic warriors;
    public Statistic developmentPoints;

    public Statistic movementPoints;

    public bool[] buildingsPermit;
    public bool[,] research;
    public bool[] units;
    public bool[] spells;

    public bool cheaperBuilding;
    public bool cheaperRecruitment;

    public bool movementBuilding;
    public bool movementRecruitment;



    public int[] selectedSpells;

    public int texesIndex;
    public int researchIndex;
    public int index;

    public bool taxManagement;
    public bool researchManagement;

    public PlayerStats()
    {

    }
    public PlayerStats(int coins, int index)
    {
        this.index = index;
        this.texesIndex = 2;
        this.researchIndex = 2;

        this.coins = new Statistic((float)coins, 0f, () => { UIManager.Instance.UpdateCounters(); }, "Coin");
        this.coins.AddBonus(-1, this);
        this.coins.AddBonus(-2, this);
        this.coins.AddBonus(-3, this);
        this.coins.AddBonus(-4, this);
        this.coins.SetDescription("<color=#fad000>Coins</color> are used to recruit\n and build units.");

        this.warriors = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); }, 0, "Warrior");
        this.warriors.AddBonus(-5, this);
        this.warriors.AddBonus(-6, this);
        this.warriors.SetDescription("<color=#636363>The warrior limit </color>determines the maximum\n number of units. Conquer new provinces to\n increase this limit.");

        this.developmentPoints = new Statistic(0f, 0f, () => { UIManager.Instance.UpdateCounters(); }, "DevelopmentPoint");
        this.developmentPoints.AddBonus(-7, this);
        this.developmentPoints.AddBonus(-8, this);
        this.developmentPoints.SetDescription("<color=#004ffa>Development points</color> are used to discover \n new technologies and spells.");

        this.movementPoints = new Statistic(0, () => { UIManager.Instance.UpdateCounters(); }, 0, "MovementPoint");
        this.movementPoints.AddBonus(-9,this);
        this.movementPoints.AddBonus(-10, this);
        this.movementPoints.SetDescription("<color=#05a65b>Movement points</color> are used to recruit\nunits, construct buildings and move units.\n The points regenerate at the beginning of the turn.");
        
        this.buildingsPermit = new bool[GameAssets.Instance.buildingsStats.Length];
        this.research = new bool[4,GameAssets.Instance.research.GetLength(1)];
        this.units = new bool[GameAssets.Instance.unitStats.Length];
        this.units[0] = true;
        this.spells = new bool[GameAssets.Instance.spells.Length];
        this.selectedSpells = new int[3];
        for (int i = 0; i < 3; i++)
        {
            selectedSpells[i] = -2;
        }
        selectedSpells[0] = -1;

        movementPoints.UpdateLimit();
        warriors.UpdateLimit();
        movementPoints.value = movementPoints.limit;
    }   

    public float GetNumberOfProvinces()
    {
        int number = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == 0)
            {
                number++;
            }
        }
        return number;
    }
    public float GetPopulation()
    {
        int population = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
            {
                population += (int)GameManager.Instance.provinces[i].population.value;
            }
        }
        return population;
    }
    public float GetWarriors()
    {
        int warriors = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
            {
                warriors += (int)GameManager.Instance.provinces[i].warriors.value;
            }
        }
        return warriors;
    }

   

    public float GetTurnWarriosCost()
    {
        int cost = 0;
        int unitsNumber = GameAssets.Instance.unitStats.Length;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            ProvinceStats province = GameManager.Instance.provinces[i];
            if (province.provinceOwnerIndex == index && province.unitsCounter > 0 && province.units !=null)
            {
                for (int j = 0; j < unitsNumber; j++)
                {
                    if(province.units.ContainsKey(j))
                    {
                        cost += province.units[j] * GameAssets.Instance.unitStats[j].turnCost;
                    }
                }
            }
        }
        return cost;
    }
    public int CountBuildings(int buildingIndex)
    {
        int output = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            ProvinceStats province = GameManager.Instance.provinces[i];
            if (province.provinceOwnerIndex == index && province.buildingIndex == buildingIndex) 
            {
                output++;
            }
        }
        return output;
    }
    public float GetMovementPoints()
    {
        int movementPoints = 0;
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
            {
                movementPoints += (int)GameManager.Instance.provinces[i].movementPoints.value;
            }
        }
        return movementPoints;
    }
    public void ChangeCoinsMultiplier(float coinsMultiplier)
    {
        coins.bonuses[-2].multiplier = coinsMultiplier;
    }
    public void ChangeDevelopmentMultiplier(float developmentMultiplier)
    {
        developmentPoints.bonuses[-8].multiplier = developmentMultiplier;
    }
    public void ChangePopulationMultiplier(float populationMultiplier)
    {
        for (int i = 0; i < GameManager.Instance.provinces.Length; i++)
        {
            if (GameManager.Instance.provinces[i].provinceOwnerIndex == index)
            {
                GameManager.Instance.provinces[i].population.bonuses[-11].multiplier = populationMultiplier;
            }
        }
    }
    public void ChangeDevelopmentCoinsMultiplier(float developmentCoinsMultiplier)
    {
        coins.bonuses[-3].multiplier = developmentCoinsMultiplier;
    }
    public bool CanBuild(int index)
    {
        return buildingsPermit[index];
    }

    public Func<float,float> GetFunc(int buildingIndex)
    {
        return (float multiplier) => { return CountBuildings(buildingIndex) * multiplier; };
    }

    public Func<float, string> GetStringFunc(int buildingIndex)
    {
        return (float multiplier) => { return CountBuildings(buildingIndex).ToString() + " x " + multiplier; };
    }
}
