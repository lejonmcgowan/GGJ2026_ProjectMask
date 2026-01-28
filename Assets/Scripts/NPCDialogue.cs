using System;
using UnityEngine;

[Serializable]
public struct NPCDialogueLine 
{
    public string dialogueLine;
    public MaskReactions[] validResponses;
}

[Serializable]
public struct MaskReactions 
{
    public MaskType requiredMask;
    public NPCDialogue resultingChain;
}

public enum MaskType
{
    NONE,
    HOSTILE,
    FRUSTRATED,
    PLEASED,
    COUNT
}

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "Scriptable Objects/NPCDialogue")]
public class NPCDialogue : ScriptableObject
{
    public NPCDialogueLine[] lines;
    public bool returnToPrevBranchOnEnd;

    public string DisplayLine(int index)
    {
        if(lines != null && lines.Length > index)
        {
            return lines[index].dialogueLine;
        }
        else return string.Empty;
    }
}
