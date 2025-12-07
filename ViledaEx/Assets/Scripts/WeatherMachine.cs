using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Sahne geçişi için eklendi

public class WeatherMachine : MonoBehaviour
{
    [Header("Etkileşim")]
    public KeyCode interactKey = KeyCode.E;
    public TextMeshProUGUI interactionText; // "Kapsülleri yerleştir [E]"

    [Header("Kapsül Sistemi")]
    public int requiredCapsules = 3; // Kaç kapsül gerekli
    public GameObject[] capsuleSlots; // Kapsüllerin takılacağı yerler (görsel)

    [Header("Konuşma Sistemi")]
    public DialogueSystem dialogueSystem;

    [Header("Kapsül Yerleştirme Konuşmaları")]
    [TextArea(3, 5)]
    public string[] capsuleDialogues = new string[]
    {
        "İlk kapsülü yerleştirdim.",
        "Bir tane daha...",
        "Son kapsül! Hadi bakalım..."
    };

    [Header("Tamamlama Konuşmaları")]
    [TextArea(3, 5)]
    public string[] completionDialogues = new string[]
    {
        "Harika! Cihaz çalışmaya başladı!",
        "Hava durumu verileri geliyor...",
        "Artık patlamayı tahmin edebilirim!"
    };

    [Header("Sahne Geçişi")]
    public string finishSceneName = "Finish"; // Geçilecek sahne adı
    public float sceneTransitionDelay = 2f; // Konuşmalar bittikten sonra bekleme süresi

    [Header("Ses Efektleri")]
    public AudioClip capsuleInsertSound;
    public AudioClip machineStartSound;

    [Header("Efektler (Opsiyonel)")]
    public ParticleSystem activationEffect;
    public GameObject brokenMachine; // Bozuk görsel
    public GameObject fixedMachine; // Çalışan görsel

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

        // Kapsül slotlarını gizle
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
            interactionText.text = $"Kapsülleri yerleştir ({capsulesInserted}/{requiredCapsules}) [E]";
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

        // Kapsül ses efekti
        if (audioSource != null && capsuleInsertSound != null)
        {
            audioSource.PlayOneShot(capsuleInsertSound);
        }

        // Kapsül slotunu göster
        if (capsulesInserted < capsuleSlots.Length && capsuleSlots[capsulesInserted] != null)
        {
            capsuleSlots[capsulesInserted].SetActive(true);
        }

        capsulesInserted++;

        Debug.Log($"Kapsül yerleştirildi: {capsulesInserted}/{requiredCapsules}");

        // Player'ı durdur ve konuşma göster
        StopPlayer();

        if (capsulesInserted < requiredCapsules)
        {
            // Ara konuşma
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
            // Tüm kapsüller yerleştirildi
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

        Debug.Log("Cihaz tamamlandı!");

        // Makine başlatma sesi
        if (audioSource != null && machineStartSound != null)
        {
            audioSource.PlayOneShot(machineStartSound);
        }

        // Görsel değişimi
        if (brokenMachine != null) brokenMachine.SetActive(false);
        if (fixedMachine != null) fixedMachine.SetActive(true);

        // Particle efekt
        if (activationEffect != null)
        {
            activationEffect.Play();
        }

        yield return new WaitForSeconds(1f);

        // Tamamlama konuşmaları
        if (dialogueSystem != null)
        {
            dialogueSystem.dialogues = completionDialogues;
            dialogueSystem.StartDialogues();

            yield return new WaitUntil(() => dialogueSystem.AreDialoguesFinished());
        }

        EnablePlayer();

        Debug.Log("Sahne görevi tamamlandı!");

        // Finish sahnesine geçiş
        yield return new WaitForSeconds(sceneTransitionDelay);

        Debug.Log($"Finish sahnesine geçiliyor: {finishSceneName}");
        SceneManager.LoadScene(finishSceneName);
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