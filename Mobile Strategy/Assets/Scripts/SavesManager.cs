using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavesManager 
{
    public static void Save(GameData gameData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Game.save";
        FileStream file = new FileStream(path, FileMode.OpenOrCreate);

        binaryFormatter.Serialize(file, gameData);
        file.Close();
    }

    public static GameData Load()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Game.save";
        if(File.Exists(path))
        {
            FileStream file = new FileStream(path, FileMode.Open);
            GameData gameData = binaryFormatter.Deserialize(file) as GameData;
            file.Close();
            return gameData;
        }
        else
        {
            Debug.LogError("The save dones not exists");
            return null;
        }
    }
}
