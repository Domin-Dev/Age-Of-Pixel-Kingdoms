using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SafeArea : MonoBehaviour
{
    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Rect rect = Screen.safeArea;
        Vector2 anchorMin = rect.position;
        Vector2 anchorMax = rect.position + rect.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
    
        anchorMax.y /= Screen.height;
        anchorMax.x /= Screen.width;
            
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax; 
    }

}
