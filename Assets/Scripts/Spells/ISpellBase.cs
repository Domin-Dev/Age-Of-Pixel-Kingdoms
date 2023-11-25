public interface ISpellBase 
{
    public void StartSpell(int pathIndex, UnitsManager unitsManager);
    public void AnimationEnd();
    public void ExecuteSpell();
}

