using System;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource AudioSource; 
    public static SFXManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public AudioClip selectSFX;
    public AudioClip confirmSFX;

    private void Awake()
    {
        if (FindObjectsByType<SFXManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
    }
    public void PlayConfirmSFX()
    {
        PlaySFX(confirmSFX);
    }

    public void PlaySelectSFX()
    {
        PlaySFX(selectSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if(AudioSource != null)
        {
            AudioSource.clip = clip;
            AudioSource.Play();
        }
    }
}
