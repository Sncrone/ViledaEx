using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Basit Dialogue Sistemi - Mevcut baloncuðu kullanýr
public class DialogueSystem2 : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject dialogueBalloon; // Senin baloncuðun
    [SerializeField] private TextMeshProUGUI dialogueText; // Baloncuktaki text
    [SerializeField] private GameObject player; // Player objesi

    [Header("Daktilo Ayarlarý")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Konuþmalar")]
    [SerializeField]
    private string[] dialogueLines = new string[]
    {
        "Tamam reaktörün patlama anýna geldik.",
        "Bakalým ne olmuþ?"
    };

    [Header("Reaktör")]
    [SerializeField] private ReactorDestruction reactor;
    [SerializeField] private float delayBeforeExplosion = 1f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private MonoBehaviour playerController;

    void Start()
    {
        // PlayerMovement scriptini bul
        playerController = player.GetComponent("PlayerMovement") as MonoBehaviour;

        // Baloncuðu gizle
        dialogueBalloon.SetActive(false);

        // Dialogue'u baþlat
        StartDialogue();
    }

    void Update()
    {
        if (!dialogueActive) return;

        // Space tuþu kontrolü
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Yazmayý atla
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLine];
                isTyping = false;
            }
            else
            {
                // Sonraki satýra geç
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        dialogueActive = true;
        currentLine = 0;

        // Player'ý durdur
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Baloncuðu göster
        dialogueBalloon.SetActive(true);

        // Ýlk satýrý göster
        ShowLine();
    }

    void ShowLine()
    {
        StartCoroutine(TypeLine(dialogueLines[currentLine]));
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

        if (currentLine < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueActive = false;

        // Baloncuðu gizle
        dialogueBalloon.SetActive(false);

        // Player'ý aktif et
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Reaktörü patlat
        StartCoroutine(TriggerReactorExplosion());
    }

    IEnumerator TriggerReactorExplosion()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);

        if (reactor != null)
        {
            reactor.StartDestruction();
        }
    }
}

// Reaktör yýkýlma sistemi
public class ReactorDestruction : MonoBehaviour
{
    [Header("Yýkýlma Ayarlarý")]
    [SerializeField] private float destructionDelay = 2f;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int debrisCount = 20;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float debrisLifetime = 3f;

    [Header("Çatlak Sistemi")]
    [SerializeField] private List<SpriteRenderer> crackSprites;
    [SerializeField] private float crackInterval = 0.5f;

    [Header("Kamera Sarsma")]
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 2f;

    [Header("Partiküller")]
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private ParticleSystem explosionParticles;

    private SpriteRenderer mainRenderer;
    private Camera mainCamera;

    void Start()
    {
        mainRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        foreach (var crack in crackSprites)
        {
            if (crack != null)
                crack.enabled = false;
        }
    }

    public void StartDestruction()
    {
        StartCoroutine(DestructionSequence());
    }

    private IEnumerator DestructionSequence()
    {
        if (smokeParticles != null)
            smokeParticles.Play();

        yield return StartCoroutine(ShowCracks());

        StartCoroutine(CameraShake());

        yield return new WaitForSeconds(destructionDelay);

        if (explosionParticles != null)
            explosionParticles.Play();

        SpawnDebris();

        if (mainRenderer != null)
            mainRenderer.enabled = false;

        yield return new WaitForSeconds(debrisLifetime);
        Destroy(gameObject);
    }

    private IEnumerator ShowCracks()
    {
        foreach (var crack in crackSprites)
        {
            if (crack != null)
            {
                crack.enabled = true;
                yield return new WaitForSeconds(crackInterval);
            }
        }
    }

    private void SpawnDebris()
    {
        if (debrisPrefab == null) return;

        for (int i = 0; i < debrisCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2f;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

            GameObject debris = Instantiate(debrisPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (spawnPos - transform.position).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                rb.angularVelocity = Random.Range(-360f, 360f);
            }

            Destroy(debris, debrisLifetime);
        }
    }

    private IEnumerator CameraShake()
    {
        if (mainCamera == null) yield break;

        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            mainCamera.transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }
}