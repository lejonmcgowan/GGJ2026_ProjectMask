using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using System;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class StageClearMenu : MonoBehaviour
{
    public GameObject uiHolder;

    public static StageClearMenu Instance;

    public InputAction ConfirmAction;

    const float kConfirmCooldown = .2f;
    float currentCooldown;

    bool confirmed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        ConfirmAction.performed += ctx => { Confirm(ctx); };

        ToggleActive(false);
    }

    void Update()
    {
        if(currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public void ToggleActive(bool active)
    {
        currentCooldown = kConfirmCooldown;
        uiHolder.SetActive(active);
        if(active)
        {
            currentCooldown = kConfirmCooldown;
            ConfirmAction.Enable();

            if(DialogueSystem.Instance != null && PlayerController.Instance != null)
            {
                PlayerController.Instance.ChangeControlScheme(ControlSchemeType.DIALOGUE);
            }
        }
        else
        {
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

    public void Confirm(InputAction.CallbackContext context)
    {
        if(confirmed)
        {
            return;
        }

        confirmed = true;
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }
}