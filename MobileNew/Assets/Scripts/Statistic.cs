using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Statistic 
{
    public float value {private set; get; }
    public float turnIncome {private set; get; }

    public List<Bonus> bonuses;




    public override string ToString()
    {
        return value.ToString();
    }
    public Statistic(float value, float turnIncome) 
    { 
        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = turnIncome;
    }
    public Statistic(float value)
    {
        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }
    public void Subtract(float value)
    {
        this.value = Mathf.Clamp(this.value - value, 0, float.MaxValue);
    }

    public void AddBonus(Bonus bonus)
    {
        if(bonus.type == Bonus.bonusType.Disposable)
        {
            value += bonus.bonusValue;
        }
        bonuses.Add(bonus);
    }
    
}
