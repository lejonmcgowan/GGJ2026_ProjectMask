using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable]
public struct DialogueFace
{
    public MaskType expression;
    public Sprite faceSprite;
}

public class DialogueBranchCheckpoint
{
    public NPCDialogue dialogue;
    public int currentLineIndex;

    public DialogueBranchCheckpoint( NPCDialogue dialogue, int lastIndex)
    {
        this.dialogue = dialogue;
        this.currentLineIndex = lastIndex;
    }
}

public class DialogueSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject uiHolder;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public static DialogueSystem Instance;
    List<DialogueBranchCheckpoint> dialogueBranchChain = new List<DialogueBranchCheckpoint>();

    public UnityEngine.UI.Image speakerBaseSprite;
    public UnityEngine.UI.Image speakerFaceSprite;

    Interactable currentSpeaker;
    NPCDialogue currentDialogue;
    NPCDialogueLine currentLine;

    public InputAction ProgressAction;

    const float kProgressCooldown = .5f;
    float currentCooldown;

    void Awake()
    {
        ProgressAction.performed += ctx => { ProgressDialogue(ctx); };
    }

    void Start()
    {
        Instance = this;
        ToggleActive(false);
    }

    void Update()
    {
        if(uiHolder.activeInHierarchy == false)
            return;

        if(currentCooldown > 0)
            currentCooldown -= Time.deltaTime;
    }

    public void ToggleActive(bool active)
    {
        uiHolder.SetActive(active);
        if(active)
        {
            currentCooldown = kProgressCooldown;
            ProgressAction.Enable();
        }
        else 
            ProgressAction.Disable();
    }

    public void BeginDialogue(Interactable speaker, NPCDialogue dialogue)
    {
        if(dialogue == null || dialogue.lines.Length == 0)
            return;

        currentSpeaker = speaker;
        speakerText.text = currentSpeaker.charName;
        dialogueBranchChain.Add(new DialogueBranchCheckpoint(dialogue, 0));
        speakerBaseSprite.sprite = speaker.dialogueSpriteBase;
        currentDialogue = dialogue;
        SetSpeakerDialogue(dialogue.lines[0]);

        if(!uiHolder.activeInHierarchy)
            ToggleActive(true);
    }

    void SetSpeakerDialogue(NPCDialogueLine line)
    {
        currentLine = line;
        dialogueText.text = line.dialogueLine;
        speakerFaceSprite.sprite = currentSpeaker.GetExpression(line.expression);
    }

    public bool CheckForMaskResponse(MaskType mask)
    {
        foreach(MaskReactions react in currentLine.validResponses)
        {
            if(react.requiredMask == mask)
            {
                BeginDialogue(currentSpeaker, react.resultingChain);
                return true;
            }
        }
        return false;
    }

    public void ProgressDialogue(InputAction.CallbackContext context)
    {
        if(currentCooldown > 0) return;

        dialogueBranchChain[dialogueBranchChain.Count-1].currentLineIndex++;
        if(currentDialogue.lines.Length <= dialogueBranchChain[dialogueBranchChain.Count - 1].currentLineIndex)
        {
            switch(currentDialogue.clearAction)
            {
                case DialogueClearAction.RETURN_TO_PREV_CHAIN:
                    //pop latest chain.
                    dialogueBranchChain.RemoveAt(dialogueBranchChain.Count - 1);
                    //set current dialogue back to previous.
                    currentDialogue = dialogueBranchChain[dialogueBranchChain.Count - 1].dialogue;
                    //display line we came from
                    SetSpeakerDialogue(currentDialogue.lines[dialogueBranchChain[dialogueBranchChain.Count - 1].currentLineIndex]);
                    break;
                case DialogueClearAction.CLEAR_STAGE:
                    EndDialogue();
                    //Call for stage clear here.
                    break;
                case DialogueClearAction.END_DIALOGUE:
                default:
                    EndDialogue();
                    break;
            }
        }
        else
        {
            SetSpeakerDialogue(currentDialogue.lines[dialogueBranchChain[dialogueBranchChain.Count - 1].currentLineIndex]);
        }
    }

    public void EndDialogue()
    {
        ToggleActive(false);
        dialogueBranchChain.Clear();
        currentSpeaker.EndInteract();
    }
}
