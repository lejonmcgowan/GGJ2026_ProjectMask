using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string charName;
    public DialogueFace[] faces;
    public NPCDialogue[] interactDialogues;

    Action OnInteractEnd;

    public SpriteRenderer OverworldSprite;

    public Sprite[] overworldStates;

    public GameObject InteractPrompt;

    int currentDialogueState = 0;

    public bool Interact(Action onInteractEnd)
    {
        if(interactDialogues != null && interactDialogues.Length > currentDialogueState)
        {
            if(DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.BeginDialogue(this, interactDialogues[currentDialogueState]);
            }
            OnInteractEnd = onInteractEnd;
            return true;
        }
        else return false;
    }

    public Sprite GetExpression(MaskType expression)
    {
        foreach(DialogueFace face in faces)
        {
            if(face.expression == expression)
            {
                return face.faceSprite;
            }
        }
        return null;
    }

    public void EndInteract()
    {
        OnInteractEnd();
    }

    public void UpdateNPCState(int valueChange)
    {
        currentDialogueState += valueChange;
        if(overworldStates != null && overworldStates.Length > currentDialogueState)
        {
            OverworldSprite.sprite = overworldStates[currentDialogueState];
        }
    }
}
