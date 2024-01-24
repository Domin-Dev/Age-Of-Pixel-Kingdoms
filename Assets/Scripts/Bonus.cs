using System;


[System.Serializable]
public class Bonus
{
    public override string ToString()
    {
        float value = bonusValue;
        string answer = name +":";
        if(type == bonusType.DependentIncome || type == bonusType.IncreaseLimit)
        {
            if (countBonus != null)
            {
                value = countBonus(multiplier);
                answer += " " + toString(multiplier);
            }
        }
        value = (float)Math.Round(value,2);

        if (value > 0) return answer + "<color=green>  +" + value.ToString() + "</color>";
        else return answer + "<color=red>  " + value.ToString() + "</color>";
    }

    public int bonusIndex;

    public float multiplier;
    public float bonusValue { private set; get; }
    public string name {private set; get; }
    public bonusType type { private set; get; }

    public Func<float, float> countBonus;
    public Func<float, String> toString;
    public enum bonusType
    {
        Income,
        Disposable,
        DependentIncome,
        IncreaseLimit,
    }
    public Bonus(string name, float value, bonusType bonusType)
    {
        this.type = bonusType;
        this.bonusValue = value;
        this.name = name;
    }

    public Bonus(string name,Func<float,float> countBonus, Func<float,string> toString, float multiplier)
    {
        this.bonusValue = -1;
        this.name = name;
        this.type = bonusType.DependentIncome;
        this.countBonus = countBonus;
        this.toString = toString;
        this.multiplier = multiplier;
    }


    public Bonus(string name, Func<float, float> countBonus, Func<float, string> toString)
    {
        this.multiplier = 1;
        this.bonusValue = 0;
        this.name = name;
        this.type = bonusType.IncreaseLimit;
        this.countBonus = countBonus;
        this.toString = toString;
    }
}
