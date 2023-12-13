using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] List<AudioClip> clips;
    [SerializeField] float masterVolume;
    AudioClip activeClip;
    [SerializeField] string themeMusicName;
    [SerializeField] bool playThemeMusicOnAwake;
    public static SoundManager instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = masterVolume;
    }


    void GetClipByName(string name)
    {
        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].name == name)
            {
                activeClip = clips[i];
            }
        }
    }

    

    public void PlaySound(string name, bool loop)
    {
        if (PlayerPrefs.GetInt("sounds") == 0)
        {
            audioSource.Stop();
            GetClipByName(name);
            audioSource.loop = loop;
            audioSource.clip = activeClip;
            audioSource.time = 0;
            audioSource.Play();
            //audioSource.PlayOneShot(activeClip);
        }

    }

    float currentDur;
    public void PlaySoundForAWhile(string name, bool loop)
    {
        currentDur = audioSource.time;
        audioSource.time = 0;
        audioSource.Stop();
        GetClipByName(name);
        //Debug.Log("Current: " + currentDur);
        audioSource.clip = activeClip;
        audioSource.loop = loop;
        audioSource.Play();
    }


    public void ReturnThemeMusic()
    {
        audioSource.Stop();
        GetClipByName(themeMusicName);
        //Debug.Log("Current2: " + currentDur);
        audioSource.loop = true;
        audioSource.clip = activeClip;
        audioSource.Play();
        audioSource.time = Mathf.Min(currentDur, activeClip.length - 0.01f);
    }



    public void PlaySoundOneShot(string name)
    {
        GetClipByName(name);
        audioSource.PlayOneShot(activeClip);
    }
    public void PlaySoundOneShot(string name, float volume)
    {
        if(PlayerPrefs.GetInt("sounds") == 0)
        {
            GetClipByName(name);
            float volumeScale = 1 / masterVolume;
            audioSource.PlayOneShot(activeClip, volume * volumeScale);
        }
  
    }
    public void StopSound()
    {
        audioSource.Stop();
    }
}
