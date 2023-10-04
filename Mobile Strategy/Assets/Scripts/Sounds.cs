using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds instance;
    AudioSource audioSource;

    [SerializeField] List<AudioClip> soundsList;

    private void Awake()
    {
        if(instance == null )
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(int index)
    {
        audioSource.PlayOneShot(soundsList[index]);
    }
}
