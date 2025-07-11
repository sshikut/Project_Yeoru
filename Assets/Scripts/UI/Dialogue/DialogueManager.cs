using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public Image dialogueCharacter;
    public float typingSpeed = 0.05f;

    private string[] dialogueLines;
    private int currentLineIndex;

    private bool isTyping = false;
    private bool dialogueActive = false;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialogue(string[] lines, string name, Sprite sprite)
    {
        dialogueLines = lines;
        dialogueName.text = name;
        dialogueCharacter.sprite = sprite;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        dialogueActive = true;
        CharacterMove.instance.canMove = false;

        StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }

    void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // 즉시 한 줄 완성
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < dialogueLines.Length)
                {
                    StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        dialogueActive = false;
        CharacterMove.instance.canMove = true;
    }
}
