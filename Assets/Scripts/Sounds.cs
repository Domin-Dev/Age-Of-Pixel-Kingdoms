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

    [SerializeField] int currentMusic;
    private void Awake()
    {
        if(instance == null )
        {
            instance = this;
            PlayerPrefs.SetFloat("Sounds", 0.5f);
            PlayerPrefs.SetFloat("Music", 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SetSliders();
        currentMusic = Random.Range(0, musicList.Count);
        PlayMusic(currentMusic);
    }

    private void OnLevelWasLoaded(int level)
    {
        SetSliders();
    }

    private void SetSliders()
    {
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Slider sl in sliders)
        {
            if (sl.tag == "Sounds")
            {
                soundsSlider = sl;
            }
            else if (sl.tag == "Music")
            {
                musicSlider = sl;
            }
        }
        soundsSlider.value = PlayerPrefs.GetFloat("Sounds");
        musicSlider.value = PlayerPrefs.GetFloat("Music");
        soundsSlider.onValueChanged.AddListener((float value) => { SetSoundsVolume(value); PlaySound(5); });
        musicSlider.onValueChanged.AddListener((float value) => { SetMusicVolume(value); PlaySound(5); });
    }
    private void Update()
    {
        if(!audioSourceMusic.isPlaying)
        {
            if(currentMusic + 1 < musicList.Count)
            {
                currentMusic++;
                PlayMusic(currentMusic);
            }
            else
            {
                currentMusic = 0;
            }
        }
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
