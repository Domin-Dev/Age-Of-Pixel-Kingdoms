

using UnityEngine;

[CreateAssetMenu(fileName ="New MapStats" , menuName = "Map/MapStats")]
public class MapStats : ScriptableObject
{
    public MapStats(int numberOfProvinces, ProvinceStats[] provinces)
    {
        this.numberOfProvinces = numberOfProvinces;
        this.provinces = provinces;
    }

    [SerializeField] public int numberOfProvinces;
    [SerializeField] public ProvinceStats[] provinces;

}
