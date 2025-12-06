using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplosionTrigger : MonoBehaviour
{
    [Header("Patlama Ayarlar�")]
    public GameObject explosionPrefab; // Patlama animasyonu prefab'�
    public Transform explosionSpawnPoint; // Patlaman�n ��kaca�� yer
    public float explosionDelay = 0.5f; // Karakterin durmas�ndan sonra bekleme

    [Header("Kamera Efektleri")]
    public bool enableCameraShake = true;
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.5f;

    [Header("Sahne Ge�i�i")]
    public bool changeScene = true;
    public string nextSceneName = "Scene_Past"; // Ge�mi� sahnesinin ad�
    public float sceneChangeDelay = 3f; // Patlamadan sonra bekleme s�resi

    [Header("Ses Efektleri")]
    public AudioClip explosionSound;

    [Header("Fade Efekti (�ste�e Ba�l�)")]
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
        // Sadece Player ile �arp���nca tetikle
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            player = collision.gameObject;
            StartCoroutine(TriggerExplosionSequence());
        }
    }

    IEnumerator TriggerExplosionSequence()
    {
        Debug.Log("Patlama sekans� ba�lad�!");

        // 1. A�AMA: Karakteri durdur
        DisablePlayerControls();

        // K�sa bekleme
        yield return new WaitForSeconds(explosionDelay);

        // 2. A�AMA: Patlama efektini spawn et
        if (explosionPrefab != null)
        {
            Vector3 spawnPos = explosionSpawnPoint != null ? explosionSpawnPoint.position : transform.position;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);

            // Patlamay� otomatik yok et (animasyon s�resi kadar)
            Animator anim = explosion.GetComponent<Animator>();
            if (anim != null)
            {
                AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    float animLength = clipInfo[0].clip.length;
                    Destroy(explosion, animLength);
                }
            }
            else
            {
                Destroy(explosion, 2f); // Varsay�lan 2 saniye
            }
        }

        // 3. A�AMA: Ses efekti �al
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // 4. A�AMA: Kamera sars�nt�s�
        if (enableCameraShake)
        {
            StartCoroutine(CameraShake());
        }

        // 5. A�AMA: Sahne ge�i�i i�in bekle
        yield return new WaitForSeconds(sceneChangeDelay);

        // 6. A�AMA: Sahneyi de�i�tir
        if (changeScene)
        {
            if (useFadeEffect)
            {
                // Fade efekti ile ge�i� (e�er fade panel varsa)
                StartCoroutine(FadeAndLoadScene());
            }
            else
            {
                // Direkt sahne ge�i�i
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

        // Rigidbody h�z�n� s�f�rla
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Animator'� durdur (idle pozisyonunda kal)
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0);
        }

        Debug.Log("Player kontrolleri devre d��� b�rak�ld�!");
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
        // Basit fade efekti (e�er CanvasGroup varsa)
        // Daha geli�mi� fade i�in ayr� sistem gerekebilir
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}