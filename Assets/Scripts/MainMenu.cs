using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    public InputAction AltConfirmAction;
    public GameObject CreditsScreen;

    MainMenuButton currentSelectedButton;

    const float kSelectCooldown = .2f;
    float currentCooldown;

    void Awake()
    {
        AltConfirmAction.performed += ctx => { OnAltConfirm(ctx); };
    }

    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        AltConfirmAction.Enable();
        currentCooldown = kSelectCooldown;
    }

    void Update()
    {
        if(currentCooldown > 0)
            currentCooldown -= Time.deltaTime;
    }

    public void HighlightOption(MainMenuButton button)
    {
        previewText.text = button.previewText;
        previewImage.sprite = button.previewSprite;
        currentSelectedButton = button;
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
                    StartCoroutine(LoadLevel("Scenes/Stage1"));
                    break;
                case ButtonAction.STAGE_2:
                    confirming = false;
                    break;
                case ButtonAction.STAGE_3:
                    confirming = false;
                    break;
                case ButtonAction.OPTIONS:
                    StartCoroutine(LoadLevel("Scenes/SoundSettingsMenu"));
                    break;
                case ButtonAction.CREDITS:
                    CreditsScreen.gameObject.SetActive(true);
                    confirming = false;
                    break;
                case ButtonAction.QUIT:
                    Application.Quit();
                    confirming = false;
                    break;
            }
        }
    }

    IEnumerator LoadLevel(string scenePath)
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(scenePath);
    }

    void PlaySfx(AudioClip clip)
    {
        if(audioPlayer != null)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
        }
    }

    void OnAltConfirm(InputAction.CallbackContext context)
    {
        if(currentCooldown > 0) 
            return;

        currentCooldown = kSelectCooldown;

        if(CreditsScreen.activeInHierarchy)
        {
            PlaySfx(menuConfirmSfx);
            CreditsScreen.SetActive(false);
        }
        else
        {
            RunConfirm(currentSelectedButton);
        }
    }
}