using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string charName;
    public NPCDialogue[] tempInteractText;
    public int currentDialogueState = 0;

    public void Interact()
    {
        if(tempInteractText != null && tempInteractText.Length > 0)
        {
            string line = tempInteractText[0].DisplayLine(0);
            if(line != string.Empty)
            {
                Debug.LogError($"{charName}: {line}");
            }
            else
            {
                Debug.LogError($"ERROR - {charName} has no valid lines to display");
            }
        }
    }
}
