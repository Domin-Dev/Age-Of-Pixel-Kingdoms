using System;
using System.Collections.Generic;
using UnityEngine;


public struct Statistic 
{
    public float value;
    public float limit;


    public float turnIncome {private set; get; }

    public List<Bonus> bonuses;

    private Func<float,float> EndTurn;
    private Action updateCounter;

    public override string ToString()
    {
        if (limit < float.MaxValue) return ((int)value).ToString() + "/" + ((int)limit).ToString();

        if (turnIncome == 0) return ((int)value).ToString();
        else
        {
            if(turnIncome >= 0) return ((int)value).ToString() +"  <color=green>+"+ turnIncome +"</color>";
            else return ((int)value).ToString() + "  <color=red>" + turnIncome + "</color>";
        }
    }
    public Statistic(Func<float,float> endturn,float value, float turnIncome,Action counter)
    {
        this.limit = float.MaxValue;
        this.updateCounter = counter;
        this.EndTurn = endturn;      
        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = turnIncome;
    }
    public Statistic(float value)
    {
        this.limit = float.MaxValue;
        this.EndTurn = null;
        this.updateCounter = null;

        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }
    public Statistic(int value, Action counter,float limit)
    {
        this.limit = limit;
        this.updateCounter = counter;
        this.EndTurn = null;

        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }

    public float NextTurn()
    {
        if (EndTurn != null)
        {
            float income = EndTurn(turnIncome);
            value += income;
            if (updateCounter != null) updateCounter();
            return income;
        }
        return 0;
    }   
    public void Subtract(int value2)
    {
        value = Mathf.Clamp((int)value - value2, 0, limit);
        if (updateCounter != null)
        {
            updateCounter();
        }
    }
    public void Add(int value2)
    {
        value = Mathf.Clamp((int)value + value2, 0, limit);
        if (updateCounter != null)
        {
            updateCounter();
        }
    }

    public bool CanAfford(int value)
    {
        if (this.value >= value)
            return true;
        else
            return false;
    }
    public bool CheckLimit(int value)
    {
        if (this.value + value <= limit)
            return true;
        else
            return false;
    }
    public int ToLimit()
    {
        return(int)(limit - value);
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
