using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public string[] dialogueLines;
    public string dialogueName;
    public Sprite dialogueSprite;

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogueLines, dialogueName, dialogueSprite);
    }
}