
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

        }
        icon += ">";
        return icon;
    }
}
