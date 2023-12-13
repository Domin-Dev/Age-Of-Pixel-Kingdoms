public interface ISpellBase 
{
    public bool StartSpell(bool isPlayer,int pathIndex, UnitsManager unitsManager);
    public void AnimationEnd();
    public void ExecuteSpell();

}

