using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform buttons;


    [SerializeField] private Transform settings;
    [SerializeField] private Transform saves;
    [SerializeField] private Transform confirmation;
    [SerializeField] private Transform maps;

    [SerializeField] private GameObject saveUI;

    private string nameSaveToDelete;
    Transform objToDelete;
    private void Start()
    {
        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { settings.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });
        buttons.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { saves.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });


        confirmation.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { confirmation.gameObject.SetActive(false);Sounds.instance.PlaySound(5); });
        confirmation.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { Delete(true); Sounds.instance.PlaySound(5); });
        confirmation.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Delete(false); Sounds.instance.PlaySound(5); });
        
        settings.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => {  settings.gameObject.SetActive(false); Sounds.instance.PlaySound(5); });
        saves.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { saves.gameObject.SetActive(false); Sounds.instance.PlaySound(5); });
        LoadSaves();
    }

    private void LoadSaves()
    {
        Transform parent = saves.GetChild(1).GetChild(0).GetChild(0).transform;
        string path = Application.persistentDataPath + "/Saves";
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
            save.GetChild(1).GetComponent<TextMeshProUGUI>().text = GetName(fileInfo.Name) + "<size=50><color=#686868>\n" + fileInfo.LastWriteTime;
            save.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); LoadSave(fileInfo.Name); });
            save.GetChild(3).GetComponent<Button>().onClick.AddListener(() => { Sounds.instance.PlaySound(5); OpenConfirmation(fileInfo.FullName,save.transform); });
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

    private void LoadSave(string name)
    {
        GameManager.Instance.toLoad = true;
        GameManager.Instance.load = () =>
        {
            GameManager.Instance.Load(name);
        };
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
