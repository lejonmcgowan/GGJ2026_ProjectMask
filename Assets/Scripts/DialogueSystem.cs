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

    public UnityEngine.UI.Image speakerFaceSprite;
    public UnityEngine.UI.Image playerFaceSprite;

    Interactable currentSpeaker;
    NPCDialogue currentDialogue;
    NPCDialogueLine currentLine;

    public InputAction ProgressAction;

    const float kProgressCooldown = .2f;
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
        SetPlayerFace(PlayerController.Instance.currentMask);

        currentLine = dialogue.lines[0];
        currentSpeaker = speaker;
        speakerText.text = currentSpeaker.charName;
        if(CheckForMaskResponse(PlayerController.Instance.currentMask))
            return;
        dialogueBranchChain.Add(new DialogueBranchCheckpoint(dialogue, 0));
        currentDialogue = dialogue;
        SetSpeakerDialogue(dialogue.lines[0]);

        if(!uiHolder.activeInHierarchy)
            ToggleActive(true);
    }

    void SetSpeakerDialogue(NPCDialogueLine line)
    {
        MaskSelectMenu.Instance.UnlockMask(line.expression);
        if(line.maskToRemove != MaskType.NONE)
        {
            MaskSelectMenu.Instance.RemoveMask(line.maskToRemove);
        }
        currentLine = line;
        dialogueText.text = line.dialogueLine;
        speakerFaceSprite.sprite = currentSpeaker.GetExpression(line.expression);
    }

    public void SetPlayerFace(MaskType mask)
    {
        playerFaceSprite.sprite = PlayerController.Instance.GetExpression(mask);
        PlayerController.Instance.SetExpression(mask);
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
        if(currentCooldown > 0 || MaskSelectMenu.Instance.uiHolder.activeInHierarchy) return;

        dialogueBranchChain[dialogueBranchChain.Count-1].currentLineIndex++;
        if(currentDialogue.lines.Length <= dialogueBranchChain[dialogueBranchChain.Count - 1].currentLineIndex)
        {
            switch(currentDialogue.clearAction)
            {
                case DialogueClearAction.RETURN_TO_PREV_CHAIN:
                    //pop latest chain.
                    dialogueBranchChain.RemoveAt(dialogueBranchChain.Count - 1);
                    if(dialogueBranchChain.Count == 0)
                        EndDialogue();
                    else
                    {
                        //set current dialogue back to previous.
                        currentDialogue = dialogueBranchChain[dialogueBranchChain.Count - 1].dialogue;
                        SetPlayerFace(MaskType.NONE);
                        //display line we came from
                        SetSpeakerDialogue(currentDialogue.lines[dialogueBranchChain[dialogueBranchChain.Count - 1].currentLineIndex]);
                    }
                    break;
                case DialogueClearAction.CLEAR_STAGE:
                    EndDialogue();
                    StageClearMenu.Instance.ToggleActive(true);
                    //Call for stage clear here.
                    break;
                case DialogueClearAction.INCREMENT_NPC_STATE:
                    currentSpeaker.currentDialogueState++;
                    EndDialogue();
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
