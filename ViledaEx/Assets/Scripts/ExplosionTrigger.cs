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
        // Debug için her çarpışmayı logla
        Debug.Log("Bir şey trigger'a girdi: " + collision.gameObject.name);
        Debug.Log("Tag: " + collision.tag);

        // Sadece Player ile çarpışınca tetikle
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("PLAYER BULUNDU! Patlama başlıyor!");
            hasTriggered = true;
            player = collision.gameObject;
            StartCoroutine(TriggerExplosionSequence());
        }
        else if (!collision.CompareTag("Player"))
        {
            Debug.LogWarning("Giren obje Player değil! Tag: " + collision.tag);
        }
    }

    // Alternatif: Normal collision (trigger çalışmazsa bunu kullan)
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision tespit edildi: " + collision.gameObject.name);

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

            // Particle System'i durdur (tek seferlik oynat, sonra dur)
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.loop = false; // Loop kapalı - bir kez oynat
                // Patlamayı YOK ETME - ekranda kalsın
            }

            // NOT: Patlama artık silinmiyor, ekranda kalıyor
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
                // Fade efekti ile geçiş (eğer fade panel varsa)
                StartCoroutine(FadeAndLoadScene());
            }
            else
            {
                // Direkt sahne geçişi
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    void DisablePlayerControls()
    {
        if (player == null) return;

        // Player hareketini durdur
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
        }

        // Animator'ı durdur (idle pozisyonunda kal)
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0);
        }

        Debug.Log("Player kontrolleri devre dışı bırakıldı!");
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
        // Basit fade efekti (eğer CanvasGroup varsa)
        // Daha gelişmiş fade için ayrı sistem gerekebilir
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}