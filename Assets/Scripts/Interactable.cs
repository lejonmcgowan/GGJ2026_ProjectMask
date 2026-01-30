using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string charName;
    public Sprite dialogueSpriteBase;
    public DialogueFace[] faces;
    public NPCDialogue[] interactDialogues;
    public int currentDialogueState = 0;

    Action OnInteractEnd;

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
}
