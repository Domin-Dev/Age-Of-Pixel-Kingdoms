
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class FPSCounter : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        text.text = math.round(GameManager.Instance.timer).ToString();
    }
}
