

using System;

[System.Serializable]
public class BonusData 
{
    public float multiplier;
    public float bonusValue;
    public string name;
    public Bonus.bonusType type;


    public BonusData(Bonus bonus)
    {
        this.multiplier = bonus.multiplier;
        this.bonusValue = bonus.bonusValue;
        this.name = bonus.name;
        this.type = bonus.type;
    }
    public Bonus ToBonus()
    {
        Bonus bonus = new Bonus(name, bonusValue, type);
        bonus.multiplier = this.multiplier;
        return bonus;
    }
}


