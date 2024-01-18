
[System.Serializable]
public class BonusData 
{
    public int bonusIndex;
    public float multiplier;
    public float bonusValue;
    public string name;
    public Bonus.bonusType type;


    public BonusData(Bonus bonus)
    {
        this.bonusIndex = bonus.bonusIndex;
        this.multiplier = bonus.multiplier;
        this.bonusValue = bonus.bonusValue;
        this.name = bonus.name;
        this.type = bonus.type;
    }
    public Bonus ToBonus()
    {
        Bonus bonus = new Bonus(name, bonusValue, type);
        bonus.bonusIndex = this.bonusIndex;
        bonus.multiplier = this.multiplier;
        return bonus;
    }
}


