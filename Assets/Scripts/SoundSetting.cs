
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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

    public Button DefaultsButton;
    public Button BackButton;
    
    AudioSource audioPlayer;

    bool confirming = false;

    private float defaultMasterVolume;
    private float defaultBGMVolume;
    private float defaultSFXVolume;
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        Mixer.GetFloat("MASTER", out defaultMasterVolume);
        Mixer.GetFloat("BGM", out defaultBGMVolume);
        Mixer.GetFloat("SFX", out defaultSFXVolume);
        
        MasterSlider.value = defaultMasterVolume;
        BGMSlider.value = defaultBGMVolume;
        SFXSlider.value = defaultSFXVolume;
        
        MasterSlider.onValueChanged.AddListener(delegate {OnMasterVolumeChanged();});
        BGMSlider.onValueChanged.AddListener(delegate {OnBGMVolumeChanged();});
        SFXSlider.onValueChanged.AddListener(delegate {OnSFXVolumeChanged();});
        DefaultsButton.onClick.AddListener(ResetDefaultVolumes);
        BackButton.onClick.AddListener(GoBack);
    }

    private void GoBack()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    void PlaySfx(AudioClip clip)
    {
        if(audioPlayer != null)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
        }
    }
    
    public void OnMasterVolumeChanged()
    {
        Mixer.SetFloat("MASTER", MasterSlider.value);
    }
    
    public void OnSFXVolumeChanged()
    {
        Mixer.SetFloat("SFX", SFXSlider.value);
    }
    
    public void OnBGMVolumeChanged()
    {
        Mixer.SetFloat("BGM", BGMSlider.value);
    }

    public void ResetDefaultVolumes()
    {
        MasterSlider.value = defaultMasterVolume;
        BGMSlider.value = defaultBGMVolume;
        SFXSlider.value = defaultSFXVolume;
    }
}
