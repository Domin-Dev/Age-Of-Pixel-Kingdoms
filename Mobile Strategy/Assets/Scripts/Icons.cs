
public abstract class Icons
{
    public static string GetIcon(string iconName)
    {
        string icon = " <sprite index=";
        switch (iconName)
        {
            case "MovementPoint": icon += "23"; break;
            case "Coin": icon += "21"; break;
            case "Turn": icon += "2"; break;
            case "Population": icon += "1"; break;
            case "DevelopmentPoint": icon += "22"; break;
            case "Warrior": icon += "0"; break;
        }
        icon += ">";
        return icon;
    }
}
