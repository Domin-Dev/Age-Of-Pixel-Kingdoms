using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        text.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}
