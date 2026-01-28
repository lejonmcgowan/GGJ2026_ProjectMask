using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string charName;
    public string tempInteractText;

    public void Interact()
    {
        Debug.LogError(charName + ": " + tempInteractText);
    }
}
