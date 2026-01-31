using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using System;
using NUnit.Framework;

[Serializable]
public struct MaskButton
{
    public MaskType mask;
    public Image maskIcon;
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
        ToggleActive(false);
        ToggleAction.Enable();

        if(masks.Length > 0)
            selector.transform.position = masks[0].maskIcon.transform.position;
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
                return;
            }
        }
    }

    public void ToggleActive(bool active)
    {
        currentCooldown = kToggleCooldown;
        uiHolder.SetActive(active);
        selector.transform.position = masks[selectedIndex].maskIcon.transform.position;
        if(active)
        {
            currentCooldown = kToggleCooldown;
            NavAction.Enable();
            ConfirmAction.Enable();

            if(DialogueSystem.Instance != null && PlayerController.Instance != null)
            {
                PlayerController.Instance.ChangeControlScheme(ControlSchemeType.MASK_SELECT);
            }
        }
        else
        {
            NavAction.Disable();
            ConfirmAction.Disable();

            if(DialogueSystem.Instance != null && PlayerController.Instance != null)
            {
                if(DialogueSystem.Instance.uiHolder.activeInHierarchy)
                    PlayerController.Instance.ChangeControlScheme(ControlSchemeType.DIALOGUE);
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

    bool AttemptMaskHighlight(int index)
    {
        if(masks[index].mask != selectedMask && masks[index].maskIcon.gameObject.activeInHierarchy)
        {
            selectedIndex = index;
            selectedMask = masks[index].mask;
            selector.transform.position = masks[index].maskIcon.transform.position;
            return true;
        }
        return false;
    }
}