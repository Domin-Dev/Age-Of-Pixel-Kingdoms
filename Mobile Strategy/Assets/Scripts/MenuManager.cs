using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

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
        foreach (string file in files)
        {

            Transform save = Instantiate(saveUI, parent).transform;
            FileInfo fileInfo = new FileInfo(file);
            save.GetChild(1).GetComponent<TextMeshProUGUI>().text = fileInfo.Name + "<size=50><color=#686868>\n" + fileInfo.LastWriteTime;
        }
    }
}
