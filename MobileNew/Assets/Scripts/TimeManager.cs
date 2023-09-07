using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    Button button;
    TextMeshProUGUI text;
    int[] speeds = { 1, 2, 4, 8};
    int currentSpeed;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { SetSpeed(); });
        currentSpeed = 0;
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void SetSpeed()
    {
        if (currentSpeed + 1 < speeds.Length)
        {
            currentSpeed++;
        }
        else
        {
            currentSpeed = 0;
        }
        Time.timeScale = speeds[currentSpeed];
        text.text = speeds[currentSpeed].ToString() + "X";
    }
    private void OnDestroy()
    {
        Time.timeScale = 1;

    }
}