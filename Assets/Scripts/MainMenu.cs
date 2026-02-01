using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ButtonAction
{
    STAGE_1,
    STAGE_2,
    STAGE_3,
    OPTIONS,
    CREDITS,
    QUIT
}

public class MainMenu : MonoBehaviour
{
    public Image previewImage;
    public TextMeshProUGUI previewText;
    public Image protagMask;

    public AudioClip menuHighlightSfx;
    public AudioClip menuConfirmSfx;

    AudioSource audioPlayer;

    bool confirming = false;

    public Sprite caseOpenMask;
    public Sprite caseCloseMask;
    public Sprite optionsMask;
    public Sprite creditsMask;
    public Sprite quitMask;

    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    public void HighlightOption(MainMenuButton button)
    {
        previewText.text = button.previewText;
        previewImage.sprite = button.previewSprite;
        switch(button.myAction)
        {
            case ButtonAction.STAGE_1:
            case ButtonAction.STAGE_2:
            case ButtonAction.STAGE_3:
                //if stage complete, then caseClosedMask. else,
                protagMask.sprite = caseOpenMask;
                break;
            case ButtonAction.OPTIONS:
                protagMask.sprite = optionsMask;
                break;
            case ButtonAction.CREDITS:
                protagMask.sprite = creditsMask;
                break;
            case ButtonAction.QUIT:
                protagMask.sprite = quitMask;
                break;
        }
        PlaySfx(menuHighlightSfx);
    }

    public void RunConfirm(MainMenuButton button)
    {
        if(!confirming)
        {
            confirming = true;
            PlaySfx(menuConfirmSfx);
            switch(button.myAction)
            {
                case ButtonAction.STAGE_1:
                    SceneManager.LoadScene("Scenes/Stage1");
                    break;
                case ButtonAction.STAGE_2:
                    confirming = false;
                    break;
                case ButtonAction.STAGE_3:
                    confirming = false;
                    break;
                case ButtonAction.OPTIONS:
                    SceneManager.LoadScene("Scenes/SoundSettingsMenu");
                    break;
                case ButtonAction.CREDITS:
                    confirming = false;
                    break;
                case ButtonAction.QUIT:
                    Application.Quit();
                    confirming = false;
                    break;
            }
        }
    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneIndex);
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