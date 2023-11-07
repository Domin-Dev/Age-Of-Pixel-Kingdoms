using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;

public static class SavesManager 
{

    
    public static void Save(GameData gameData)
    {
        string path = Application.persistentDataPath + "/Saves";

        if (!Directory.Exists(path))  Directory.CreateDirectory(path);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        path = path + "/Game";
        FileStream file = new FileStream(path, FileMode.OpenOrCreate);
        binaryFormatter.Serialize(file, gameData);
        file.Close();
    }

    public static GameData Load()
    {
        string path = Application.persistentDataPath + "/Saves";
        if (Directory.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            path = path + "/Game";
            if (File.Exists(path))
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
        return null;
    }
}
