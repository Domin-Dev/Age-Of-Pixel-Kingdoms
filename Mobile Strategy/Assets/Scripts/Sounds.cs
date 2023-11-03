using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sounds : MonoBehaviour
{
    public static Sounds instance;
    [SerializeField] AudioSource audioSourceSounds;
    [SerializeField] AudioSource audioSourceMusic;



    [SerializeField] List<AudioClip> musicList;
    [SerializeField] List<AudioClip> soundsList;
    [SerializeField] AudioMixer audioMixer;

    Slider soundsSlider;
    Slider musicSlider;

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
        DontDestroyOnLoad(gameObject);

        soundsSlider = GameObject.FindGameObjectWithTag("Sounds").GetComponent<Slider>();
        musicSlider = GameObject.FindGameObjectWithTag("Music").GetComponent<Slider>();
        soundsSlider.onValueChanged.AddListener((float value) => { SetSoundsVolume(value); });
        musicSlider.onValueChanged.AddListener((float value) => { SetMusicVolume(value); });

        PlayMusic(0);
    }

    public void SetSoundsVolume(float value)
    {
        audioMixer.SetFloat("Sounds", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("Sounds", value);
    } 
    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("Music", value);
    }

    public void PlaySound(int index)
    {
        audioSourceSounds.PlayOneShot(soundsList[index]);
    }
    
    public void PlayMusic(int index)
    {
        audioSourceMusic.PlayOneShot(musicList[index]);
    }



    public AudioClip GetClip(int index)
    {
        return soundsList[index];
    }
}
