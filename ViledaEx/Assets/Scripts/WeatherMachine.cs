using System.Collections;
using UnityEngine;
using TMPro;

public class WeatherMachine : MonoBehaviour
{
    [Header("Etkile�im")]
    public KeyCode interactKey = KeyCode.E;
    public TextMeshProUGUI interactionText; // "Kaps�lleri yerle�tir [E]"

    [Header("Kaps�l Sistemi")]
    public int requiredCapsules = 3; // Ka� kaps�l gerekli
    public GameObject[] capsuleSlots; // Kaps�llerin tak�laca�� yerler (g�rsel)

    [Header("Konu�ma Sistemi")]
    public DialogueSystem dialogueSystem;

    [Header("Kaps�l Yerle�tirme Konu�malar�")]
    [TextArea(3, 5)]
    public string[] capsuleDialogues = new string[]
    {
        "�lk kaps�l� yerle�tirdim.",
        "Bir tane daha...",
        "Son kaps�l! Hadi bakal�m..."
    };

    [Header("Tamamlama Konu�malar�")]
    [TextArea(3, 5)]
    public string[] completionDialogues = new string[]
    {
        "Harika! Cihaz �al��maya ba�lad�!",
        "Hava durumu verileri geliyor...",
        "Art�k patlamay� tahmin edebilirim!"
    };

    [Header("Ses Efektleri")]
    public AudioClip capsuleInsertSound;
    public AudioClip machineStartSound;

    [Header("Efektler (Opsiyonel)")]
    public ParticleSystem activationEffect;
    public GameObject brokenMachine; // Bozuk g�rsel
    public GameObject fixedMachine; // �al��an g�rsel

    private int capsulesInserted = 0;
    private bool playerNearby = false;
    private bool machineFixed = false;
    private AudioSource audioSource;
    private GameObject player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (interactionText != null)
        {
            interactionText.text = "";
        }

        // Kaps�l slotlar�n� gizle
        foreach (GameObject slot in capsuleSlots)
        {
            if (slot != null) slot.SetActive(false);
        }

        if (fixedMachine != null)
        {
            fixedMachine.SetActive(false);
        }
    }

    void Update()
    {
        if (playerNearby && !machineFixed && Input.GetKeyDown(interactKey))
        {
            InsertCapsule();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
            player = collision.gameObject;

            if (!machineFixed)
            {
                ShowInteractionPrompt();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
            HideInteractionPrompt();
        }
    }

    void ShowInteractionPrompt()
    {
        if (interactionText != null)
        {
            int remaining = requiredCapsules - capsulesInserted;
            interactionText.text = $"Kaps�lleri yerle�tir ({capsulesInserted}/{requiredCapsules}) [E]";
        }
    }

    void HideInteractionPrompt()
    {
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    void InsertCapsule()
    {
        if (capsulesInserted >= requiredCapsules) return;

        // Kaps�l ses efekti
        if (audioSource != null && capsuleInsertSound != null)
        {
            audioSource.PlayOneShot(capsuleInsertSound);
        }

        // Kaps�l slotunu g�ster
        if (capsulesInserted < capsuleSlots.Length && capsuleSlots[capsulesInserted] != null)
        {
            capsuleSlots[capsulesInserted].SetActive(true);
        }

        capsulesInserted++;

        Debug.Log($"Kaps�l yerle�tirildi: {capsulesInserted}/{requiredCapsules}");

        // Player'� durdur ve konu�ma g�ster
        StopPlayer();

        if (capsulesInserted < requiredCapsules)
        {
            // Ara konu�ma
            if (dialogueSystem != null && capsulesInserted <= capsuleDialogues.Length)
            {
                string[] singleDialogue = new string[] { capsuleDialogues[capsulesInserted - 1] };
                StartCoroutine(ShowDialogueAndContinue(singleDialogue));
            }
            else
            {
                EnablePlayer();
            }

            ShowInteractionPrompt();
        }
        else
        {
            // T�m kaps�ller yerle�tirildi
            HideInteractionPrompt();
            StartCoroutine(CompleteMachine());
        }
    }

    IEnumerator ShowDialogueAndContinue(string[] dialogues)
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.dialogues = dialogues;
            dialogueSystem.StartDialogues();

            yield return new WaitUntil(() => dialogueSystem.AreDialoguesFinished());
        }

        EnablePlayer();
    }

    IEnumerator CompleteMachine()
    {
        machineFixed = true;

        Debug.Log("Cihaz tamamland�!");

        // Makine ba�latma sesi
        if (audioSource != null && machineStartSound != null)
        {
            audioSource.PlayOneShot(machineStartSound);
        }

        // G�rsel de�i�imi
        if (brokenMachine != null) brokenMachine.SetActive(false);
        if (fixedMachine != null) fixedMachine.SetActive(true);

        // Particle efekt
        if (activationEffect != null)
        {
            activationEffect.Play();
        }

        yield return new WaitForSeconds(1f);

        // Tamamlama konu�malar�
        if (dialogueSystem != null)
        {
            dialogueSystem.dialogues = completionDialogues;
            dialogueSystem.StartDialogues();

            yield return new WaitUntil(() => dialogueSystem.AreDialoguesFinished());
        }

        EnablePlayer();

        Debug.Log("Sahne g�revi tamamland�!");
    }

    void StopPlayer()
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Animator anim = player.GetComponent<Animator>();
        if (anim != null) anim.SetFloat("Speed", 0);
    }

    void EnablePlayer()
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;
    }
}