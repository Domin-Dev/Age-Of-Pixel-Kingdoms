using System.Diagnostics;

public static class BonusManager
{
    public static void SetBonus(ProvinceStats provinceStats,int indexBonus)
    {
        switch (indexBonus)
        {
            case 0:provinceStats.lifePoints.AddBonus(new Bonus("Castle",10,Bonus.bonusType.Disposable)); break;
        }
    }
}
