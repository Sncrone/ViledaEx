using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Referanslarý")]
    public GameObject dialogueBalloon;
    public TextMeshProUGUI dialogueText;
    public Transform playerTransform;
    public Vector3 balloonOffset = new Vector3(0, 2f, 0);

    [Header("Konuþmalar")]
    [TextArea(3, 5)]
    public string[] dialogues = new string[]
    {
        "Hay aksi! Bu da neydi?",
        "Hayýr olamaz! Reaktörden mi geldi bu patlama?",
        "Nasýl oldu bu? Hemen bu iþi düzeltmeliyim."
    };

    [Header("Ayarlar")]
    public float typingSpeed = 0.05f;
    public KeyCode nextKey = KeyCode.Space;

    [Header("Ses (Ýsteðe Baðlý)")]
    public AudioClip typingSound;

    private int currentIndex = 0;
    private bool isTyping = false;
    private bool canContinue = false;
    private AudioSource audioSource;
    private Coroutine typingCoroutine;
    private bool dialoguesFinished = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (dialogueBalloon != null)
        {
            dialogueBalloon.SetActive(false);
        }
    }

    void Update()
    {
        // Baloncuðu karakterin üstünde tut
        if (dialogueBalloon != null && dialogueBalloon.activeSelf && playerTransform != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(playerTransform.position + balloonOffset);
            dialogueBalloon.transform.position = screenPos;
        }

        // Space tuþu kontrolü
        if (dialogueBalloon != null && dialogueBalloon.activeSelf && Input.GetKeyDown(nextKey))
        {
            if (isTyping)
            {
                // Yazýyorsa tamamla
                CompleteTyping();
            }
            else if (canContinue)
            {
                // Sonraki konuþmaya geç
                ShowNextDialogue();
            }
        }
    }

    public void StartDialogues()
    {
        currentIndex = 0;
        dialoguesFinished = false;

        if (dialogueBalloon != null)
        {
            dialogueBalloon.SetActive(true);
        }

        ShowDialogue(0);
    }

    void ShowDialogue(int index)
    {
        if (index >= dialogues.Length)
        {
            // Tüm konuþmalar bitti
            FinishDialogues();
            return;
        }

        canContinue = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(dialogues[index]));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;

            if (audioSource != null && typingSound != null && letter != ' ')
            {
                audioSource.PlayOneShot(typingSound, 0.2f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        canContinue = true;
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = dialogues[currentIndex];
        isTyping = false;
        canContinue = true;
    }

    void ShowNextDialogue()
    {
        currentIndex++;
        ShowDialogue(currentIndex);
    }

    void FinishDialogues()
    {
        if (dialogueBalloon != null)
        {
            dialogueBalloon.SetActive(false);
        }

        dialoguesFinished = true;
        Debug.Log("Tüm konuþmalar bitti!");
    }

    public bool AreDialoguesFinished()
    {
        return dialoguesFinished;
    }
}