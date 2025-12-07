/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Basit Dialogue Sistemi - Mevcut baloncu�u kullan�r
public class DialogueSystem2 : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject dialogueBalloon; // Senin baloncu�un
    [SerializeField] private TextMeshProUGUI dialogueText; // Baloncuktaki text
    [SerializeField] private GameObject player; // Player objesi

    [Header("Daktilo Ayarlar�")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Konu�malar")]
    [SerializeField]
    private string[] dialogueLines = new string[]
    {
        "Tamam reakt�r�n patlama an�na geldik.",
        "Bakal�m ne olmu�?"
    };

    [Header("�kinci Konu�ma (Patlama Sonras�)")]
    [SerializeField]
    private string[] postExplosionDialogueLines = new string[]
    {
        "Hay aksi! Biraz daha geriye gitmeliyim."
    };

    [Header("Reakt�r")]
    [SerializeField] private ReactorDestruction reactor;
    [SerializeField] private BackgroundExplosion backgroundExplosion;
    [SerializeField] private float delayBeforeExplosion = 1f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private bool isPostExplosionDialogue = false; // Hangi dialogue oldu�unu takip et
    private MonoBehaviour playerController;

    void Start()
    {
        // PlayerMovement scriptini bul
        playerController = player.GetComponent("PlayerMovement") as MonoBehaviour;

        // Baloncu�u gizle
        dialogueBalloon.SetActive(false);

        // Dialogue'u ba�lat
        StartDialogue();
    }

    void Update()
    {
        if (!dialogueActive) return;

        // Space tu�u kontrol�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Yazmay� atla
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLine];
                isTyping = false;
            }
            else
            {
                // Sonraki sat�ra ge�
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        dialogueActive = true;
        currentLine = 0;
        isPostExplosionDialogue = false; // �lk dialogue

        // Player'� durdur
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Baloncu�u g�ster
        dialogueBalloon.SetActive(true);

        // �lk sat�r� g�ster
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

        // Baloncu�u gizle
        dialogueBalloon.SetActive(false);

        // Player'� aktif et
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Sadece ilk dialogue bittiyse patlamay� tetikle
        if (!isPostExplosionDialogue)
        {
            StartCoroutine(TriggerReactorExplosion());
        }
        // �kinci dialogue bittiyse hi�bir �ey yapma (d�ng�ye girmesin)
    }

    IEnumerator TriggerReactorExplosion()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);

        // �nce background patlar
        if (backgroundExplosion != null)
        {
            backgroundExplosion.TriggerExplosion();

            // Patlama bitene kadar bekle (toplam s�re hesapla)
            float explosionDuration = backgroundExplosion.GetTotalExplosionDuration();
            yield return new WaitForSeconds(explosionDuration);

            // Patlama sonras� dialogue ba�lat
            StartPostExplosionDialogue();
        }

        // Sonra reakt�r (e�er varsa)
        if (reactor != null)
        {
            reactor.StartDestruction();
        }
    }

    public void StartPostExplosionDialogue()
    {
        // Player'� durdur
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // �kinci konu�may� ba�lat
        dialogueLines = postExplosionDialogueLines;
        currentLine = 0;
        dialogueActive = true;
        isPostExplosionDialogue = true; // �kinci dialogue oldu�unu i�aretle

        dialogueBalloon.SetActive(true);
        ShowLine();
    }
}

// Reakt�r y�k�lma sistemi
public class ReactorDestruction : MonoBehaviour
{
    [Header("Y�k�lma Ayarlar�")]
    [SerializeField] private float destructionDelay = 2f;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private int debrisCount = 20;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float debrisLifetime = 3f;

    [Header("�atlak Sistemi")]
    [SerializeField] private List<SpriteRenderer> crackSprites;
    [SerializeField] private float crackInterval = 0.5f;

    [Header("Kamera Sarsma")]
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 2f;

    [Header("Partik�ller")]
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
}*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Basit Dialogue Sistemi - Mevcut baloncuğu kullanır
public class DialogueSystem2 : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject dialogueBalloon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject player;

    [Header("Daktilo Ayarları")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Konuşmalar")]
    [SerializeField]
    private string[] dialogueLines = new string[]
    {
        "Tamam reaktörün patlama anına geldik.",
        "Bakalım ne olmuş?"
    };

    [Header("İkinci Konuşma (Patlama Sonrası)")]
    [SerializeField]
    private string[] postExplosionDialogueLines = new string[]
    {
        "Hay aksi! Biraz daha geriye gitmeliyim."
    };

    [Header("Reaktör")]
    [SerializeField] private ReactorDestruction reactor;
    [SerializeField] private BackgroundExplosion backgroundExplosion;
    [SerializeField] private float delayBeforeExplosion = 1f;

    [Header("Ses")]
    [SerializeField] private AudioClip dialogueSound;
    [SerializeField] private float dialogueSoundVolume = 0.3f;

    private AudioSource audioSource;
    private int currentLine = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private bool isPostExplosionDialogue = false;
    private MonoBehaviour playerController;

    void Start()
    {
        playerController = player.GetComponent("PlayerMovement") as MonoBehaviour;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        dialogueBalloon.SetActive(false);

        StartDialogue();
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLine];
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
        dialogueActive = true;
        currentLine = 0;
        isPostExplosionDialogue = false;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        dialogueBalloon.SetActive(true);

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

            if (dialogueSound != null && audioSource != null && letter != ' ')
            {
                audioSource.PlayOneShot(dialogueSound, dialogueSoundVolume);
            }

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

        dialogueBalloon.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (!isPostExplosionDialogue)
        {
            StartCoroutine(TriggerReactorExplosion());
        }
    }

    IEnumerator TriggerReactorExplosion()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);

        if (backgroundExplosion != null)
        {
            backgroundExplosion.TriggerExplosion();

            float explosionDuration = backgroundExplosion.GetTotalExplosionDuration();
            yield return new WaitForSeconds(explosionDuration);

            StartPostExplosionDialogue();
        }

        if (reactor != null)
        {
            reactor.StartDestruction();
        }
    }

    public void StartPostExplosionDialogue()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        dialogueLines = postExplosionDialogueLines;
        currentLine = 0;
        dialogueActive = true;
        isPostExplosionDialogue = true;

        dialogueBalloon.SetActive(true);
        ShowLine();
    }
}

// Reaktör yıkılma sistemi
public class ReactorDestruction : MonoBehaviour
{
    [Header("Yıkılma Ayarları")]
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

    [Header("Ses")]
    [SerializeField] private AudioClip reactorSound;
    [SerializeField] private float reactorSoundVolume = 0.8f;

    private AudioSource audioSource;
    private SpriteRenderer mainRenderer;
    private Camera mainCamera;

    void Start()
    {
        mainRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

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

        if (reactorSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reactorSound, reactorSoundVolume);
        }

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