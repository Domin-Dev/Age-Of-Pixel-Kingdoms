using System;
using System.Diagnostics;

[System.Serializable]
public class BonusData 
{
    public float multiplier;
    public float bonusValue;
    public string name;
    public Bonus.bonusType type;

    public Func<float, float> countBonus;
    public Func<float, String> toString;

    public BonusData(Bonus bonus)
    {
        this.multiplier = bonus.multiplier;
        this.bonusValue = bonus.bonusValue;
        this.name = bonus.name;
        this.type = bonus.type;
        if(type == Bonus.bonusType.DependentIncome && bonusValue >= 0)
        {
            UnityEngine.Debug.Log("dziala  " + name);
            this.countBonus = null;
            this.toString = null;


        }
        else
        {
            this.countBonus = bonus.countBonus;
            this.toString = bonus.toString;
        }

        UnityEngine.Debug.Log(this.countBonus);
        UnityEngine.Debug.Log(this.toString);

    }
    public Bonus ToBonus()
    {
        Bonus bonus = new Bonus(name, bonusValue, type);
        bonus.multiplier = this.multiplier;
        bonus.countBonus = this.countBonus;
        bonus.toString = this.toString;
        return bonus;
    }
}


