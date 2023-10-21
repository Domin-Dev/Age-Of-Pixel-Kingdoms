
using TMPro;
using UnityEngine;

public class Alert : MonoBehaviour
{
    public static Alert Instance;
    private Animator animator;
    private bool isPlaying;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isPlaying = false;
        DontDestroyOnLoad(gameObject);
        animator = GetComponent<Animator>();
    }
    public void OpenAlert(string message)
    {
        Sounds.instance.PlaySound(4);
        if (!isPlaying)
        {
            isPlaying = true;
            GetComponentInChildren<TextMeshProUGUI>().text = message;
            animator.SetTrigger("Alert");
        }
    }

    public void End()
    {
        isPlaying = false;
    }
}
