using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosionTrigger : MonoBehaviour
{
    [Header("Patlama Ayarları")]
    public GameObject explosionPrefab;
    public Transform explosionSpawnPoint;
    public float explosionDelay = 0.5f;

    [Header("Kamera Sarsıntısı")]
    public bool enableCameraShake = true;
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.5f;

    [Header("Ses Efekti")]
    public AudioClip explosionSound;

    [Header("Konuşma Sistemi")]
    public DialogueSystem dialogueSystem;
    public float waitBeforeDialogue = 3f; // Düşmeden sonra ne kadar beklensin

    private GameObject player;
    private AudioSource audioSource;
    private bool hasTriggered = false;
    private Vector3 originalPlayerScale;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            player = collision.gameObject;
            originalPlayerScale = player.transform.localScale;
            StartCoroutine(ExplosionSequence());
        }
    }

    IEnumerator ExplosionSequence()
    {
        Debug.Log("=== PATLAMA BAŞLADI ===");

        // 1. Karakteri durdur
        StopPlayer();

        yield return new WaitForSeconds(explosionDelay);

        // 2. Patlama efekti
        SpawnExplosion();

        // 3. Ses efekti
        PlayExplosionSound();

        // 4. Kamera sarsıntısı
        if (enableCameraShake)
        {
            StartCoroutine(ShakeCamera());
        }

        // 5. Bekle (karakter yerde)
        Debug.Log("Karakter düştü, bekleniyor...");
        yield return new WaitForSeconds(waitBeforeDialogue);

        // 6. Karakteri kaldır (Idle'a döndür)
        StandUpPlayer();

        // 7. Konuşmaları başlat
        if (dialogueSystem != null)
        {
            Debug.Log("Konuşmalar başlıyor...");
            dialogueSystem.StartDialogues();

            // Konuşmalar bitene kadar bekle
            yield return new WaitUntil(() => dialogueSystem.AreDialoguesFinished());
        }

        // 8. Konuşmalar bitti - Karakteri tekrar kontrol edilebilir yap
        Debug.Log("Konuşmalar bitti! Kontroller geri veriliyor.");
        EnablePlayer();

        Debug.Log("=== SEKANS TAMAMLANDI ===");
    }

    void StopPlayer()
    {
        if (player == null) return;

        // Hareketi kapat
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Hızı sıfırla
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Düşme animasyonu
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play("Fall");
        }

        // Karakteri küçült
        StartCoroutine(ScalePlayer(0.4f));

        Debug.Log("Player durduruldu ve düştü!");
    }

    void StandUpPlayer()
    {
        if (player == null) return;

        // Eski boyuta döndür
        player.transform.localScale = originalPlayerScale;

        // Idle animasyonuna geç
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.Play("PlayerIdle");
        }

        Debug.Log("Karakter ayağa kalktı!");
    }

    void EnablePlayer()
    {
        if (player == null) return;

        // Hareketi geri aç
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        Debug.Log("Player kontrolleri aktif!");
    }

    void SpawnExplosion()
    {
        if (explosionPrefab == null) return;

        Vector3 spawnPos = explosionSpawnPoint != null ? explosionSpawnPoint.position : transform.position;
        GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);

        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.None;
        }
    }

    void PlayExplosionSound()
    {
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }

    IEnumerator ScalePlayer(float scaleMultiplier)
    {
        if (player == null) yield break;

        yield return new WaitForSeconds(0.1f);

        player.transform.localScale = new Vector3(
            originalPlayerScale.x * scaleMultiplier,
            originalPlayerScale.y * scaleMultiplier,
            originalPlayerScale.z
        );
    }

    IEnumerator ShakeCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Vector3 originalPos = cam.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            cam.transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = originalPos;
    }
}