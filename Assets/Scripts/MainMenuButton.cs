using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    public MainMenu parentMenu;
    public Sprite previewSprite;
    public string previewText;
    public ButtonAction myAction;

    public void OnHighlight()
    {
        parentMenu.HighlightOption(this);
    }

    public void OnClick()
    {
        parentMenu.RunConfirm(this);
    }
}
