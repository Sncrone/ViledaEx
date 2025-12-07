using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Etkileþim Ayarlarý")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Konuþma Sistemi")]
    public DialogueSystem dialogueSystem; // DialogueSystem objesini buraya sürükle

    [Header("Konuþmalar")]
    [TextArea(3, 5)]
    public string[] dialogues = new string[]
    {
        "Merhaba! Bu bir etkileþimli obje.",
        "E tuþuna basarak benimle konuþabilirsin.",
        "Ýþte bu kadar basit!"
    };

    [Header("UI Bildirimi (Opsiyonel)")]
    public GameObject interactionPrompt; // "E tuþuna bas" görseli/text

    [Header("Ses (Opsiyonel)")]
    public AudioClip interactionSound;

    private bool playerNearby = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Baþlangýçta prompt'u gizle
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Player yakýndaysa ve E'ye basýldýysa
        if (playerNearby && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;

            // "E tuþuna bas" göster
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }

            Debug.Log("Player etkileþim alanýna girdi");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;

            // Prompt'u gizle
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            Debug.Log("Player etkileþim alanýndan çýktý");
        }
    }

    void Interact()
    {
        Debug.Log("Etkileþim baþladý!");

        // Ses efekti çal
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);
        }

        // Prompt'u gizle
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Konuþmalarý baþlat
        if (dialogueSystem != null)
        {
            // Konuþmalarý güncelle
            dialogueSystem.dialogues = dialogues;
            dialogueSystem.StartDialogues();
        }
        else
        {
            Debug.LogWarning("DialogueSystem atanmamýþ!");
        }
    }
}