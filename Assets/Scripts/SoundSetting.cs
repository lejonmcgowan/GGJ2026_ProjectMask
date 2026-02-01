using System.Collections;
using TMPro;
using UnityEngine;
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
    public Image previewImage;
    public TextMeshProUGUI previewText;
    public Image protagMask;

    public AudioClip menuHighlightSfx;
    public AudioClip menuConfirmSfx;

    public Slider MasterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;
    
    AudioSource audioPlayer;

    bool confirming = false;
    
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    public void HighlightOption(MainMenuButton button)
    {
        previewText.text = button.previewText;
        previewImage.sprite = button.previewSprite;
        PlaySfx(menuHighlightSfx);
    }
    
    void PlaySfx(AudioClip clip)
    {
        if(audioPlayer != null)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
        }
    }
}