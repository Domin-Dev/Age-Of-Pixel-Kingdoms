using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public struct Statistic 
{
    public float value;
    public float turnIncome {private set; get; }

    public List<Bonus> bonuses;

    private Func<float,float> EndTurn;
    private Action updateCounter;

    public override string ToString()
    {
        if (turnIncome == 0) return ((int)value).ToString();
        else
        {
            if(turnIncome >= 0) return ((int)value).ToString() +"  <color=green>+"+ turnIncome +"</color>";
            else return ((int)value).ToString() + "  <color=red>" + turnIncome + "</color>";
        }
    }
    public Statistic(Func<float,float> endturn,float value, float turnIncome,Action counter)
    {
        this.updateCounter = counter;
        this.EndTurn = endturn;      
        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = turnIncome;
    }
    public Statistic(float value)
    {
        this.EndTurn = null;
        this.updateCounter = null;

        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }
    public Statistic(int value)
    {
        this.updateCounter = null;
        this.EndTurn = null;

        bonuses = new List<Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }
    public Statistic(int value, Action counter)
    {
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
        value = Mathf.Clamp((int)value - value2, 0, float.MaxValue);
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
    public void AddBonus(Bonus bonus)
    {
        if(bonus.type == Bonus.bonusType.Disposable)
        {
            value += bonus.bonusValue;
        }
        bonuses.Add(bonus);
    }
    
}
