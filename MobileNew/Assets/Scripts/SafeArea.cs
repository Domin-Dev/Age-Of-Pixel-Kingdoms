using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour
{
    [SerializeField] Vector2 vector2;
    private void Update()
    {
       vector2 = this.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
        Debug.Log(vector2); 
        Debug.Log(Screen.currentResolution); 
    }

}
