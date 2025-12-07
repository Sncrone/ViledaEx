using UnityEngine;
using System.Collections;
using TMPro;

public class SceneDialogue : MonoBehaviour
{
    public GameObject dialogueBalloon;
    public TextMeshProUGUI dialogueText;
    public GameObject player;
    public float typingSpeed = 0.05f;

    private string[] dialogues = new string[]
    {
        "Sonunda, sanýrým doðru yere geldim.",
        "Bir dakika burayý nasýl su basmýþ? Bunun olmamasý gerekiyordu.",
        "Neyse buralarý düzeltmek gerekecek."
    };

    private int currentLine = 0;
    private bool isTyping = false;
    private MonoBehaviour playerController;

    void Start()
    {
        playerController = player.GetComponent("PlayerMovement") as MonoBehaviour;
        dialogueBalloon.SetActive(false);
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogues[currentLine];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        currentLine = 0;
        if (playerController != null) playerController.enabled = false;
        dialogueBalloon.SetActive(true);
        StartCoroutine(TypeLine(dialogues[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < dialogues.Length)
        {
            StartCoroutine(TypeLine(dialogues[currentLine]));
        }
        else
        {
            dialogueBalloon.SetActive(false);
            if (playerController != null) playerController.enabled = true;
        }
    }
}