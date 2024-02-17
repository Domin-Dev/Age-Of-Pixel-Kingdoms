using UnityEngine;

[CreateAssetMenu(fileName ="New MapStats" , menuName = "Map/MapStats")]
public class MapStats : ScriptableObject
{
    public MapStats(int numberOfProvinces, ProvinceStats[] provinces,int players,int price)
    {
        this.numberOfProvinces = numberOfProvinces;
        this.provinces = provinces;
        this.players = players;
        this.price = price;
    }



    public int price;
    public int players;
    public int numberOfProvinces;
    public ProvinceStats[] provinces;
}
