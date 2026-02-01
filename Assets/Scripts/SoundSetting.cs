
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum SoundAction
{
    MASTER,
    SFX,
    BGM,
    BACK
}

public class SoundMenu : MonoBehaviour
{
    public AudioClip menuHighlightSfx;
    public AudioClip menuConfirmSfx;

    public AudioMixer Mixer; 
    public Slider MasterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;
    
    AudioSource audioPlayer;

    bool confirming = false;
    
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        Mixer.GetFloat("MASTER", out var masterVolume);
        Mixer.GetFloat("BGM", out var bgmVolume);
        Mixer.GetFloat("SFX", out var sfxVolume);
        
        MasterSlider.minValue = -80;
        MasterSlider.maxValue = 20;
        MasterSlider.value = masterVolume;
        
        BGMSlider.minValue = -80;
        BGMSlider.maxValue = 20;
        BGMSlider.value = bgmVolume;
        
        SFXSlider.minValue = -80;
        SFXSlider.maxValue = 20;
        SFXSlider.value = sfxVolume;
    }
    
    void PlaySfx(AudioClip clip)
    {
        if(audioPlayer != null)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
        }
    }
    
    public void OnMasterVolumeChange(Slider slider)
    {
        MasterSlider.value = slider.value;
    }
}