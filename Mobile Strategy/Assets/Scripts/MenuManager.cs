using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform buttons;

    [SerializeField] private Transform settings;
    [SerializeField] private Transform saves;

    [SerializeField] private GameObject saveUI;

    private void Start()
    {
        buttons.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { settings.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });
        buttons.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { saves.gameObject.SetActive(true); Sounds.instance.PlaySound(5); });

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
            save.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { LoadSave(fileInfo.Name); });
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
        GameManager.Instance.isPlaying = true;
        GameManager.Instance.load = () =>
        {
            GameManager.Instance.Load(name);
            GameManager.Instance.load = null;         
        };
        SceneManager.LoadScene(2);

    }
}
