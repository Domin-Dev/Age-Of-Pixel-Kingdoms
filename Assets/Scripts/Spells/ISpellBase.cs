public interface ISpellBase 
{
    public void StartSpell(bool isPlayer,int pathIndex, UnitsManager unitsManager);
    public void AnimationEnd();
    public void ExecuteSpell();
}

