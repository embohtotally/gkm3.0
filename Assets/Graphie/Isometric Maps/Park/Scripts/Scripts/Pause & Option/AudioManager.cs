using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Theme");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"Music '{name}' not found!");
            return;
        }
        if (musicSource == null)
        {
            Debug.LogError("Music source is not assigned!");
            return;
        }

        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"SFX '{name}' not found!");
            return;
        }
        if (sfxSource == null)
        {
            Debug.LogError("SFX source is not assigned!");
            return;
        }

        sfxSource.PlayOneShot(s.clip);
    }

    public void ToggleMusic()
    {
        if (musicSource != null)
            musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        if (sfxSource != null)
            sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }
}
