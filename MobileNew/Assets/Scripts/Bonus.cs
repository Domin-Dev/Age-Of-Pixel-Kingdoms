
public class Bonus
{
    public float bonusValue { private set; get; }
    public string name {private set; get; }
    public bonusType type { private set; get; }
    public enum bonusType
    {
        Income,
        Disposable,
    }
    public Bonus(string name, float value, bonusType bonusType)
    {
        this.type = bonusType;
        this.bonusValue = value;
        this.name = name;
    }
}
