using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosionTrigger : MonoBehaviour
{
    [Header("Patlama Ayarları")]
    public GameObject explosionPrefab;
    public Transform explosionSpawnPoint;
    public float explosionDelay = 0.5f;

    [Header("Kamera Efektleri")]
    public bool enableCameraShake = true;
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.5f;

    [Header("Sahne Geçişi")]
    public bool changeScene = true;
    public string nextSceneName = "Scene_Past";
    public float sceneChangeDelay = 3f;

    [Header("Ses Efektleri")]
    public AudioClip explosionSound;

    [Header("Fade Efekti")]
    public bool useFadeEffect = true;
    public float fadeDuration = 2f;

    private GameObject player;
    private AudioSource audioSource;
    private bool hasTriggered = false;

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
        Debug.Log("Bir şey trigger'a girdi: " + collision.gameObject.name);

        if (collision.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("PLAYER BULUNDU! Patlama başlıyor!");
            hasTriggered = true;
            player = collision.gameObject;
            StartCoroutine(ExplosionSequence());
        }
    }

    IEnumerator ExplosionSequence()
    {
        Debug.Log("Patlama sekansı başladı!");

        // 1. Karakteri durdur
        DisablePlayerControls();

        yield return new WaitForSeconds(explosionDelay);

        // 2. Patlama efektini spawn et
        if (explosionPrefab != null)
        {
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

        // 3. Ses efekti çal
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // 4. Kamera sarsıntısı
        if (enableCameraShake)
        {
            StartCoroutine(CameraShake());
        }

        // 5. 3 saniye bekle, sonra Idle'a geç
        yield return new WaitForSeconds(3f);

        ReturnToIdle();

        // 6. Sahne geçişi için bekle
        yield return new WaitForSeconds(sceneChangeDelay);

        // 7. Sahneyi değiştir
        if (changeScene)
        {
            if (useFadeEffect)
            {
                StartCoroutine(FadeAndLoadScene());
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    void DisablePlayerControls()
    {
        if (player == null) return;

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.enabled = false;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play("Fall");
        }

        StartCoroutine(ScalePlayerForFall());

        Debug.Log("Player durduruldu ve yere düştü!");
    }

    IEnumerator ScalePlayerForFall()
    {
        if (player == null) yield break;

        yield return new WaitForSeconds(0.1f);

        player.transform.localScale = new Vector3(
            player.transform.localScale.x * 0.4f,
            player.transform.localScale.y * 0.4f,
            player.transform.localScale.z
        );
    }

    void ReturnToIdle()
    {
        if (player == null) return;

        // Karakteri eski boyutuna döndür
        player.transform.localScale = new Vector3(
            player.transform.localScale.x / 0.4f,  // 0.8 ile çarpmıştık, şimdi böl
            player.transform.localScale.y / 0.4f,
            player.transform.localScale.z
        );

        // Idle animasyonuna geç
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            // Speed'i 0 yap ki Walk'a geçmesin
            anim.SetFloat("Speed", 0f);

            // Sonra Idle'ı oynat
            anim.Play("PlayerIdle");
        }

        Debug.Log("Karakter Idle pozisyonuna döndü ve eski boyutuna geldi!");
    }

    IEnumerator CameraShake()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        Vector3 originalPos = mainCam.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            mainCam.transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.position = originalPos;
    }

    IEnumerator FadeAndLoadScene()
    {
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}