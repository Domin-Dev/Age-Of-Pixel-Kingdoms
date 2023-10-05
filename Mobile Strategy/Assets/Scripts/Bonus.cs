
using System;

public class Bonus
{
    public override string ToString()
    {
       if(bonusValue > 0) return  name + "<color=green> +" + bonusValue + "</color>";
       else return name +"<color=red>" + bonusValue + "</color>";
    }
    public float bonusValue { private set; get; }
    public string name {private set; get; }
    public bonusType type { private set; get; }

    public Func<float> countBonus { private set; get; } 
    public enum bonusType
    {
        Income,
        Disposable,
        DependentIncome,
    }
    public Bonus(string name, float value, bonusType bonusType)
    {
        this.type = bonusType;
        this.bonusValue = value;
        this.name = name;
    }

    public Bonus(string name,Func<float> countBonus)
    {
        this.bonusValue = 0;
        this.name = name;
        this.type = bonusType.DependentIncome;
        this.countBonus = countBonus;
    }
}
