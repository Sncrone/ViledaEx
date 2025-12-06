using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosionTrigger : MonoBehaviour
{
    [Header("Patlama Ayarları")]
    public GameObject explosionPrefab; // Patlama animasyonu prefab'ı
    public Transform explosionSpawnPoint; // Patlamanın çıkacağı yer
    public float explosionDelay = 0.5f; // Karakterin durmasından sonra bekleme

    [Header("Kamera Efektleri")]
    public bool enableCameraShake = true;
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.5f;

    [Header("Sahne Geçişi")]
    public bool changeScene = true;
    public string nextSceneName = "Scene_Past"; // Geçmiş sahnesinin adı
    public float sceneChangeDelay = 3f; // Patlamadan sonra bekleme süresi

    [Header("Ses Efektleri")]
    public AudioClip explosionSound;

    [Header("Fade Efekti (İsteğe Bağlı)")]
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
        // Sadece Player ile çarpışınca tetikle
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("PLAYER BULUNDU! Patlama başlıyor!");
            hasTriggered = true;
            player = collision.gameObject;
            StartCoroutine(TriggerExplosionSequence());
        }
    }

    // Alternatif: Normal collision (trigger çalışmazsa bunu kullan)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("PLAYER ÇARPTI! Patlama başlıyor!");
            hasTriggered = true;
            player = collision.gameObject;
            StartCoroutine(TriggerExplosionSequence());
        }
    }

    IEnumerator TriggerExplosionSequence()
    {
        Debug.Log("Patlama sekansı başladı!");

        // 1. AŞAMA: Karakteri durdur
        DisablePlayerControls();

        // Kısa bekleme
        yield return new WaitForSeconds(explosionDelay);

        // 2. AŞAMA: Patlama efektini spawn et
        if (explosionPrefab != null)
        {
            Vector3 spawnPos = explosionSpawnPoint != null ? explosionSpawnPoint.position : transform.position;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);

            // Particle System ayarları
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.loop = false; // Loop kapalı - bir kez oynat
                // Not: stopAction None olduğu için patlama objesi sahnede kalacaktır.
                // Sahne değişeceği için sorun değil ama normalde Destroy etmek iyidir.
                main.stopAction = ParticleSystemStopAction.None;
            }
        }

        // 3. AŞAMA: Ses efekti çal
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // 4. AŞAMA: Kamera sarsıntısı
        if (enableCameraShake)
        {
            StartCoroutine(CameraShake());
        }

        // 5. AŞAMA: Sahne geçişi için bekle
        yield return new WaitForSeconds(sceneChangeDelay);

        // 6. AŞAMA: Sahneyi değiştir
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

    // DÜZELTİLEN METOT BURASI
    void DisablePlayerControls()
    {
        if (player == null) return;

        // Player hareketini durdur (Script adınızın PlayerMovement olduğundan emin olun)
        // Eğer farklıysa burayı kendi script adınızla değiştirin.
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.enabled = false;
        }

        // Rigidbody hızını sıfırla
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f; // Dönmeyi de durdurmak iyi olabilir.
        }

        // Yere düşme animasyonunu oynat
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            // Animator'daki state adı "Fall" olmalı
            anim.Play("Fall");
        }

        // Fall animasyonu için karakter boyutunu küçült
        StartCoroutine(ScalePlayerForFall());

        // Hatalı yerdeki Debug.Log buraya taşındı.
        Debug.Log("Player durduruldu ve yere düştü!");
    }

    IEnumerator ScalePlayerForFall()
    {
        if (player == null) yield break;

        // Kısa bir bekleme (animasyon başlasın)
        yield return new WaitForSeconds(0.1f);

        // Karakteri küçült (örnek: %80)
        player.transform.localScale = new Vector3(
            player.transform.localScale.x * 0.4f,
            player.transform.localScale.y * 0.4f,
            player.transform.localScale.z
        );
    } // BURADAKİ FAZLALIKLAR TEMİZLENDİ

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
        // Not: Burada gerçek bir fade işlemi yok, sadece bekleyip sahne yüklüyor.
        // Eğer fade istiyorsanız bir UI Canvas paneli ve onun alpha değerini
        // değiştiren bir kod eklemeniz gerekir.
        Debug.Log("Fade efekti süresi bekleniyor...");
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}