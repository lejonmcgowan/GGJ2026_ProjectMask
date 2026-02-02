using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using System;
using NUnit.Framework;
using System.Collections;
using TMPro;

[Serializable]
public struct MaskButton
{
    public MaskType mask;
    public Image maskIcon;
    public Sprite maskFace;
}

public class MaskSelectMenu : MonoBehaviour
{
    public GameObject uiHolder;

    public static MaskSelectMenu Instance;

    public InputAction ToggleAction;
    public InputAction ConfirmAction;
    public InputAction NavAction;

    public MaskButton[] masks;

    const float kToggleCooldown = .2f;
    float currentCooldown;

    public GameObject selector;

    MaskType selectedMask = MaskType.NONE;

    List<MaskType> unlockedMasks = new List<MaskType>();
    int selectedIndex = 0;

    #region MaskStatusPrompt
    [SerializeField] Animator MaskStatusUpdate;
    [SerializeField] GameObject GotMaskParent;
    [SerializeField] Image GotMaskImage;
    [SerializeField] TextMeshProUGUI MaskStatusText;
    [SerializeField] GameObject GaveMaskParent;
    [SerializeField] Image GaveMaskImage;

    const string kPromptTrigger = "Show";

    void ShowMaskUpdate(MaskType mask, Sprite maskSprite, bool obtained)
    {
        if(obtained)
        {
            GaveMaskParent.SetActive(false);
            GotMaskParent.SetActive(true);
            GotMaskImage.sprite = maskSprite;
            MaskStatusText.text = string.Format("Got a new mask!\n\"{0}\"!",mask.ToString());
        }
        else
        {
            GaveMaskParent.SetActive(true);
            GotMaskParent.SetActive(false);
            GaveMaskImage.sprite = maskSprite;
            MaskStatusText.text = string.Format("Delivered the \"{0}\" mask!", mask.ToString());
        }
        MaskStatusUpdate.SetTrigger(kPromptTrigger);
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        ToggleAction.performed += ctx => { ToggleMenu(ctx); };
        ConfirmAction.performed += ctx => { ConfirmMask(ctx); };
        NavAction.performed += ctx => { NavigateMenu(ctx); };

        unlockedMasks.Add(MaskType.NONE);
        foreach(MaskButton b in masks)
        {
            if(b.mask == MaskType.NONE)
            {
                b.maskIcon.gameObject.SetActive(true);
            }
            else
            {
                b.maskIcon.gameObject.SetActive(false);
            }
        }
        ToggleActive(false, isInitial: true);
        ToggleAction.Enable();

        if(masks.Length > 0)
            StartCoroutine(MoveSelector(0));
    }

    void Update()
    {
        if(currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public void UnlockMask(MaskType mask)
    {
        if(unlockedMasks.Contains(mask))
            return;

        foreach(MaskButton b in masks)
        {
            if(b.mask == mask)
            {
                b.maskIcon.gameObject.SetActive(true);
                unlockedMasks.Add(mask);
                ShowMaskUpdate(mask, b.maskFace, true);
                return;
            }
        }
    }

    public void RemoveMask(MaskType mask)
    {
        if(!unlockedMasks.Contains(mask))
        {
            return;
        }

        foreach(MaskButton b in masks)
        {
            if(b.mask == mask)
            {
                b.maskIcon.gameObject.SetActive(false);
                unlockedMasks.Remove(mask);
                if(PlayerController.Instance.currentMask == mask)
                {
                    if(DialogueSystem.Instance.uiHolder.activeInHierarchy)
                        DialogueSystem.Instance.SetPlayerFace(MaskType.NONE);
                    else
                        PlayerController.Instance.SetExpression(MaskType.NONE);
                }
                ShowMaskUpdate(mask, b.maskFace, false);
                return;
            }
        }
    }

    IEnumerator MoveSelector(int index)
    {
        selector.gameObject.SetActive(false);
        yield return null;
        selector.transform.position = masks[index].maskIcon.transform.position;
        selector.gameObject.SetActive(true);
    }

    public void ToggleActive(bool active, bool isInitial = false)
    {
        if(!isInitial)
        {
            SFXManager.Instance.PlayConfirmSFX();
        }
        currentCooldown = kToggleCooldown;
        uiHolder.SetActive(active);
        for(int i = selectedIndex; i >= 0; i--)
        {
            if(AttemptMaskHighlight(i, isInitial: true))
            {
                break;
            }
        }
        if(active)
        {
            currentCooldown = kToggleCooldown;
            NavAction.Enable();
            ConfirmAction.Enable();

            if(DialogueSystem.Instance != null && PlayerController.Instance != null)
            {
                PlayerController.Instance.ChangeControlScheme(ControlSchemeType.MASK_SELECT);
                DialogueSystem.Instance.ToggleControlsText(false);
            }
        }
        else
        {
            NavAction.Disable();
            ConfirmAction.Disable();

            if(DialogueSystem.Instance != null && PlayerController.Instance != null)
            {
                if(DialogueSystem.Instance.uiHolder.activeInHierarchy)
                {
                    PlayerController.Instance.ChangeControlScheme(ControlSchemeType.DIALOGUE);
                    DialogueSystem.Instance.ToggleControlsText(true);
                }
                else
                    PlayerController.Instance.ChangeControlScheme(ControlSchemeType.FIELD);
            }
        }
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        ToggleActive(!uiHolder.activeInHierarchy);
    }

    public void ConfirmMask(InputAction.CallbackContext context)
    {
        PlayerController.Instance.SetExpression(selectedMask);
        DialogueSystem.Instance.SetPlayerFace(selectedMask);
        if(DialogueSystem.Instance.uiHolder.activeInHierarchy)
            DialogueSystem.Instance.CheckForMaskResponse(selectedMask);
        ToggleActive(false);
    }

    public void NavigateMenu(InputAction.CallbackContext context)
    {
        if(context.action.ReadValue<float>() > 0)
        {
            for(int i = selectedIndex; i < masks.Length; i++)
            {
                if(AttemptMaskHighlight(i))
                    return;
            }
        }
        else if (context.action.ReadValue<float>() < 0)
        {
            for(int i = selectedIndex; i >= 0; i--)
            {
                if(AttemptMaskHighlight(i))
                    return;
            }
        }
    }

    bool AttemptMaskHighlight(int index, bool isInitial = false)
    {
        if((masks[index].mask != selectedMask || isInitial) && masks[index].maskIcon.gameObject.activeInHierarchy)
        {
            selectedIndex = index;
            selectedMask = masks[index].mask;
            StartCoroutine(MoveSelector(index));
            SFXManager.Instance.PlaySelectSFX();
            return true;
        }
        return false;
    }
}