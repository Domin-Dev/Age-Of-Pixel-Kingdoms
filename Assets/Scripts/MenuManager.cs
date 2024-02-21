using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Mathematics;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform buttons;

    [SerializeField] private Transform victoryPoints;
    [SerializeField] private Transform vpButton;
    [SerializeField] private Transform settings;
    [SerializeField] private Transform saves;
    [SerializeField] private Transform confirmation;
    [SerializeField] private Transform maps;

    [SerializeField] private GameObject saveUI;
    [SerializeField] private GameObject mapUI;

    private string nameSaveToDelete;
    Transform objToDelete;

    int vp;
    private void Start()
    {
        vpButton.GetComponent<Button>().onClick.AddListener(() => {; victoryPoints.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });
        victoryPoints.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { victoryPoints.gameObject.SetActive(false); Sounds.instance.PlaySound(5); });

        vp = PlayerPrefs.GetInt("VictoryPoints", 0);
        vpButton.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = vp.ToString();

        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { settings.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });
        buttons.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { saves.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });
        buttons.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { maps.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });

        confirmation.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { confirmation.gameObject.SetActive(false);Sounds.instance.PlaySound(5); });
        confirmation.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { Delete(true); Sounds.instance.PlaySound(5); });
        confirmation.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Delete(false); Sounds.instance.PlaySound(5); });
        
        settings.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => {  settings.gameObject.SetActive(false); Sounds.instance.PlaySound(5); });
        saves.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { saves.gameObject.SetActive(false); Sounds.instance.PlaySound(5); });
        maps.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { maps.gameObject.SetActive(false); Sounds.instance.PlaySound(5); });
        LoadMaps();
        LoadSaves(); 
    }

    private void LoadSaves()
    {
        Transform parent = saves.GetChild(1).GetChild(0).GetChild(0).transform;
        string path = Application.persistentDataPath + "/Saves";

        if(!Directory.Exists(path)) 
        {
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path);
        int k = files.Length;
        for (int j = 0; j < files.Length; j++)
        {
            for (int i = 0; i < k; i++)
            {
                FileInfo fileInfo = new FileInfo(files[i]);
                if (i + 1 < files.Length)
                {
                    FileInfo fileInfo2 = new FileInfo(files[i + 1]);
                    if (fileInfo.LastWriteTime.CompareTo(fileInfo2.LastWriteTime) < 0)
                    {
                        files[i] = fileInfo2.FullName;
                        files[i + 1] = fileInfo.FullName;
                    }
                }
            }
            k--;
        }
        foreach (string file in files)
        {
            Transform save = Instantiate(saveUI, parent).transform;
            FileInfo fileInfo = new FileInfo(file);

            Texture2D texture = Resources.Load<Texture2D>("Texture/" + GetMapName(fileInfo.Name,out string turn));
            save.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            save.GetChild(1).GetComponent<TextMeshProUGUI>().text = GetName(fileInfo.Name) + "<size=50><color=#686868>\n" + fileInfo.LastWriteTime;
            save.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); LoadSave(fileInfo.Name); });
            save.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); OpenConfirmation(fileInfo.FullName,save.transform); });
        }
    }

    private void LoadMaps()
    {
        Transform parent = maps.GetChild(1).GetChild(0).GetChild(0).transform;
        Texture2D[] mapArray = Resources.LoadAll<Texture2D>("Texture");
        MapStats[] mapStats = new MapStats[mapArray.Length];

        for (int i = 0; i < mapArray.Length; i++)
        {
            mapStats[i] = Resources.Load<MapStats>("Maps/"+mapArray[i].name+"/MapStats");
        }
          
        Transform map;

        int2[] array = new int2[mapStats.Length];

        for (int i = 0; i < mapStats.Length; i++)
        {
            array[i] = new int2(mapStats[i].price,i);
        }
        GameManager.Instance.Sort(array);



        for (int i = 0; i < array.Length; i++)
        {
            int index = array[i].y;
            Texture2D item = mapArray[index];


            map =  Instantiate(mapUI, parent).transform;
            if (mapStats[index].price > PlayerPrefs.GetInt("VictoryPoints", 0))
            {
                map.GetChild(2).GetChild(1).gameObject.SetActive(true);
                map.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = mapStats[index].price.ToString();
                map.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(4);});
            }
            else
            {
                map.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); OpenMap(item.name); });
                map.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Play";
            }

            map.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.name + "<size=55><color=#686868>\nPlayers: " + mapStats[index].players + "\nProvinces: " + mapStats[index].numberOfProvinces + "\n" + item.width + " x " + item.height;
            map.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Sprite.Create(item, new Rect(0, 0, item.width, item.height), new Vector2(0.5f, 0.5f));
        }
    }

    private string GetName(string name)
    {
        string toReturn = "";
        int k = 0;
        for (int i = 0; i < name.Length; i++)
        {
            if(name[i] == ' ')
            {
                k++;
            }
            else
            {
                k = 0;
            }
            if (k > 1) break;
            toReturn += name[i];
        }
        return toReturn;
    }

    private string GetMapName(string fileName,out string turn)
    {
        string toReturn = "";
        turn = "";
        for (int i = 0; i < fileName.Length; i++)
        {
            if (fileName[i] == ' ')
            {
                string m = fileName[i+1].ToString() + fileName[i+ 2].ToString() + fileName[i+ 3].ToString() + fileName[i+ 4].ToString();
                if (m == "Turn")
                {
                    int j = i + 6;
                    while (fileName[j] != ' ')
                    {
                        turn += fileName[j];
                        j++;
                    }
                    break;
                }
            }
            toReturn += fileName[i];
        }
        return toReturn;
    }
    private void LoadSave(string name)
    {
        string turn;
        GameManager.Instance.currentMap = GetMapName(name,out turn);
        GameManager.Instance.saveName = name;
        GameManager.Instance.toLoad = true;
        GameManager.Instance.load = () =>
        {
            GameManager.Instance.Load(name,int.Parse(turn));
        };
        SceneManager.LoadScene(2);
    }
    private void OpenMap(string name)
    {
        GameManager.Instance.currentMap = name;
        GameManager.Instance.saveName = "";

        SceneManager.LoadScene(2);
    }


    private void OpenConfirmation(string name,Transform todelete)
    {
        confirmation.gameObject.SetActive(true);
        nameSaveToDelete = name;
        objToDelete = todelete.transform;
    }

    private void Delete(bool x)
    {
        if(x)
        {
            File.Delete(nameSaveToDelete);
            objToDelete.gameObject.SetActive(false);
        }
        confirmation.gameObject.SetActive(false);
    }
}
