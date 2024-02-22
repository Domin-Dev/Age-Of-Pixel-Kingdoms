using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavesManager
{
    public static void Save(GameData gameData)
    {
        string path = Application.persistentDataPath + "/Saves";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string name = GameManager.Instance.saveName;

        if (name != null && File.Exists(path + "/" + name))
        {
            File.Delete(path + "/" + name);
        }

        name = GameManager.Instance.GetName();
        path = path + "/" + name;
        GameManager.Instance.saveName = name;

        FileStream file = new FileStream(path, FileMode.OpenOrCreate);
        binaryFormatter.Serialize(file, gameData);
        file.Close();
    }

    public static GameData Load(string name)
    {
        string path = Application.persistentDataPath + "/Saves";
        if (Directory.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            path = path + "/" + name;
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

    public static void Delete()
    {
        Debug.Log("dzila");
        string path = Application.persistentDataPath + "/Saves/" + GameManager.Instance.saveName;
        if (File.Exists(path))
        {
            Debug.Log("rsss");
            File.Delete(path);
        }
    }
}
