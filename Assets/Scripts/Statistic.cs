using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Statistic 
{
    public float value;
    public float limit;
    public float turnIncome;

    public Dictionary<int,Bonus> bonuses;


    public string description;
    public string icon;
    private Action updateCounter;
    public override string ToString()
    {
        float turnAllIncome = CountIncome();

        if (limit < float.MaxValue) return ((int)value).ToString() + "/" + ((int)limit).ToString();

        if (turnAllIncome == 0) return ((int)value).ToString();
        else
        {
            turnAllIncome = (float)Math.Round(turnAllIncome, 2);
            if(turnAllIncome >= 0) return ((int)value).ToString() +"  <color=green>+"+ turnAllIncome + "</color>" + Icons.GetIcon("Turn"); 
            else return ((int)value).ToString() + "  <color=red>" + turnAllIncome + "</color>" + Icons.GetIcon("Turn"); ;
        }
    }
    public Statistic(float value, float turnIncome,Action counter,string icon)
    {
        this.description = "";
        this.icon = icon;
        this.limit = float.MaxValue;
        this.updateCounter = counter;
        bonuses = new Dictionary<int,Bonus>();
        this.value = value;
        this.turnIncome = turnIncome;
    }
    public Statistic(float value, string icon)
    {
        this.description = "";
        this.icon = icon;
        this.limit = float.MaxValue;
        this.updateCounter = null;
        bonuses = new Dictionary<int,Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }
    public Statistic(int value, Action counter,float limit, string icon)
    {
        this.description = "";
        this.icon = icon;
        this.limit = limit;
        this.updateCounter = counter;
        bonuses = new Dictionary<int,Bonus>();
        this.value = value;
        this.turnIncome = 0;
    }


    public void SetDescription(string description)
    {
        this.description = description;
    }
    public float CountIncome()
    {
        float income = turnIncome;
        foreach (Bonus item in bonuses.Values)
        { 
            if(item.type == Bonus.bonusType.DependentIncome)
            {
                income += item.countBonus(item.multiplier);
              //  Debug.Log(item.countBonus(item.multiplier) + " "+ item.multiplier );
            }

        }
        income = (float)Math.Round(income,2);
        return income;
    }
    public float NextTurn()
    {       
        float income = CountIncome();

        value += income;
        MathF.Round(income,2);
        if (updateCounter != null) updateCounter();
        return income;
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

    public void Set(float value)
    {
        this.value = Mathf.Clamp(value, 0, limit);
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

    public void AddBonus(int index,Bonus bonus)
    {
        if(bonus.type == Bonus.bonusType.Disposable)
        {
            value += bonus.bonusValue;
        }
        else if(bonus.type == Bonus.bonusType.IncreaseLimit)
        {
            limit += bonus.bonusValue;
        }  
        else
        {
            turnIncome += bonus.bonusValue;
        }

        bonuses.Add(index,bonus);
    }

    public void RemoveBonus(int index)
    {
        if (bonuses.ContainsKey(index))
        {
            Bonus bonus = bonuses[index];

            if (bonus.type == Bonus.bonusType.Disposable)
            {
                value -= bonus.bonusValue;
            }
            else
            {
                turnIncome -= bonus.bonusValue;
            }

            bonuses.Remove(index);
        }
       
    }
    public string GetDetails()
    {
        string details;

        details = Icons.GetIcon(icon) + ToString() +"\n";
        if(description.Length > 1) details += description + "\n";

        foreach (var bonus in bonuses.Values)
        {
            if (limit < float.MaxValue)
            {
                if(bonus.type == Bonus.bonusType.IncreaseLimit)
                {
                    details += "\n" + bonus.ToString() + Icons.GetIcon(icon);
                }
            }
            else
            {
                if (bonus.type == Bonus.bonusType.Income)
                {
                    details += "\n" + bonus.ToString() + Icons.GetIcon(icon);
                }
                else if (bonus.type == Bonus.bonusType.DependentIncome)
                {
                    details += "\n" + bonus.ToString() + Icons.GetIcon(icon);
                }
            }
        }
        return details;
    }

    public void UpdateLimit()
    {
        if(limit < float.MaxValue && bonuses != null)
        {
            limit = 0;
            foreach (Bonus item in bonuses.Values)
            {
                if(item.type == Bonus.bonusType.IncreaseLimit)
                {
                    if (item.bonusValue > 0) limit += item.bonusValue;
                    else limit += item.countBonus(0);
                }
            }
        }
    }
}
