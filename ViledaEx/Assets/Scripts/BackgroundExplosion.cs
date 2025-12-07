/*using UnityEngine;
using System.Collections;

public class BackgroundExplosion : MonoBehaviour
{
    [Header("Background Referans�")]
    [SerializeField] private SpriteRenderer backgroundSprite;

    [Header("Patlama Ayarlar�")]
    [SerializeField] private int gridWidth = 8; // Ka�a ka� par�aya b�l�necek
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float explosionForce = 15f;
    [SerializeField] private float pieceLifetime = 3f;

    [Header("Kamera Efektleri")]
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private int shakeCount = 3; // Ka� defa sars�lacak

    [Header("Flash Efekti")]
    [SerializeField] private SpriteRenderer flashOverlay; // Beyaz flash i�in
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashCount = 3;

    [Header("Siyah Ekran")]
    [SerializeField] private SpriteRenderer blackScreen; // Patlama sonras� siyah ekran
    [SerializeField] private float fadeToBlackDuration = 1f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeExplosion = 0.5f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Flash overlay yoksa olu�tur
        if (flashOverlay == null)
        {
            CreateFlashOverlay();
        }
        else
        {
            flashOverlay.enabled = false;
        }

        // Siyah ekran yoksa olu�tur
        if (blackScreen == null)
        {
            CreateBlackScreen();
        }
        else
        {
            blackScreen.enabled = false;
        }
    }

    private void CreateFlashOverlay()
    {
        // Yeni GameObject olu�tur
        GameObject flashObj = new GameObject("FlashOverlay");
        flashOverlay = flashObj.AddComponent<SpriteRenderer>();

        // Beyaz texture olu�tur
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        // Sprite olu�tur
        Sprite whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        flashOverlay.sprite = whiteSprite;

        // Kameray� kaplas�n
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        flashOverlay.transform.localScale = new Vector3(width, height, 1);
        flashOverlay.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        // En �stte g�r�ns�n
        flashOverlay.sortingOrder = 1000;
        flashOverlay.enabled = false;
    }

    private void CreateBlackScreen()
    {
        // Yeni GameObject olu�tur
        GameObject blackObj = new GameObject("BlackScreen");
        blackScreen = blackObj.AddComponent<SpriteRenderer>();

        // Siyah texture olu�tur
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.black);
        tex.Apply();

        // Sprite olu�tur
        Sprite blackSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        blackScreen.sprite = blackSprite;

        // Kameray� kaplas�n
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        blackScreen.transform.localScale = new Vector3(width, height, 1);
        blackScreen.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        // En �stte g�r�ns�n (flash'tan bile �stte)
        blackScreen.sortingOrder = 1001;
        blackScreen.color = new Color(0, 0, 0, 0); // Ba�ta g�r�nmez
        blackScreen.enabled = false;
    }

    private IEnumerator FadeToBlack()
    {
        if (blackScreen == null) yield break;

        blackScreen.enabled = true;
        float elapsed = 0f;

        while (elapsed < fadeToBlackDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeToBlackDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = Color.black; // Tam siyah
    }

    public void TriggerExplosion()
    {
        StartCoroutine(ExplosionSequence());
    }

    // Toplam patlama s�resini hesapla
    public float GetTotalExplosionDuration()
    {
        float preShake = 1f; // PreExplosionShake s�resi
        float mainExplosion = delayBeforeExplosion;
        float fadeTime = fadeToBlackDuration;

        return preShake + mainExplosion + fadeTime + 0.5f; // K���k buffer
    }

    private IEnumerator ExplosionSequence()
    {
        // 1. �n titre�im
        yield return StartCoroutine(PreExplosionShake());

        // 2. Flash efektleri
        StartCoroutine(FlashEffect());

        // 3. Ana kamera sarsma
        StartCoroutine(CameraShake());

        yield return new WaitForSeconds(delayBeforeExplosion);

        // 4. Background'u gizle
        if (backgroundSprite != null)
            backgroundSprite.enabled = false;

        // 5. Direkt siyah ekrana ge�i�
        yield return StartCoroutine(FadeToBlack());
    }

    private IEnumerator PreExplosionShake()
    {
        Vector3 originalPos = mainCamera.transform.position;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float intensity = Mathf.Lerp(0.1f, 0.5f, elapsed / duration);
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

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

    private IEnumerator FlashEffect()
    {
        if (flashOverlay == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            // Flash a�
            flashOverlay.enabled = true;
            flashOverlay.color = new Color(1, 1, 1, 1);

            yield return new WaitForSeconds(flashDuration);

            // Flash kapat
            flashOverlay.enabled = false;

            yield return new WaitForSeconds(flashDuration * 0.5f);
        }
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.position;

        for (int i = 0; i < shakeCount; i++)
        {
            float currentIntensity = shakeIntensity * (1f - (i / (float)shakeCount));
            float elapsed = 0f;
            float duration = shakeDuration / shakeCount;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * currentIntensity;
                float y = Random.Range(-1f, 1f) * currentIntensity;

                mainCamera.transform.position = new Vector3(
                    originalPos.x + x,
                    originalPos.y + y,
                    originalPos.z
                );

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        mainCamera.transform.position = originalPos;
    }

    private void ShatterBackground()
    {
        if (backgroundSprite == null) return;

        Sprite sprite = backgroundSprite.sprite;
        Texture2D texture = sprite.texture;

        // Background'un d�nya pozisyonunu ve boyutunu al
        Bounds bounds = backgroundSprite.bounds;
        Vector3 center = bounds.center;
        float width = bounds.size.x;
        float height = bounds.size.y;

        float pieceWidth = width / gridWidth;
        float pieceHeight = height / gridHeight;

        // Her par�a i�in
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Par�a pozisyonu hesapla
                float posX = center.x - (width / 2f) + (x * pieceWidth) + (pieceWidth / 2f);
                float posY = center.y - (height / 2f) + (y * pieceHeight) + (pieceHeight / 2f);
                Vector3 piecePos = new Vector3(posX, posY, 0);

                // Par�a olu�tur
                GameObject piece = new GameObject($"BackgroundPiece_{x}_{y}");
                piece.transform.position = piecePos;

                // Sprite Renderer ekle
                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = backgroundSprite.sortingOrder;

                // Sadece bu par�an�n b�lgesini g�ster
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size = new Vector2(pieceWidth, pieceHeight);

                // Rigidbody2D ekle
                Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
                rb.gravityScale = 1f;

                // Patlama kuvveti ekle
                Vector2 direction = (piecePos - center).normalized;
                direction += new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                rb.angularVelocity = Random.Range(-360f, 360f);

                // Fade out ekle
                StartCoroutine(FadePiece(sr, pieceLifetime));

                // Otomatik yok et
                Destroy(piece, pieceLifetime);
            }
        }
    }

    private IEnumerator FadePiece(SpriteRenderer sr, float lifetime)
    {
        yield return new WaitForSeconds(lifetime * 0.7f);

        float fadeTime = lifetime * 0.3f;
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}*/
using UnityEngine;
using System.Collections;

public class BackgroundExplosion : MonoBehaviour
{
    [Header("Background Referansı")]
    [SerializeField] private SpriteRenderer backgroundSprite;

    [Header("Patlama Ayarları")]
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float explosionForce = 15f;
    [SerializeField] private float pieceLifetime = 3f;

    [Header("Kamera Efektleri")]
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private int shakeCount = 3;

    [Header("Flash Efekti")]
    [SerializeField] private SpriteRenderer flashOverlay;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashCount = 3;

    [Header("Siyah Ekran")]
    [SerializeField] private SpriteRenderer blackScreen;
    [SerializeField] private float fadeToBlackDuration = 1f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeExplosion = 0.5f;

    [Header("Ses")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private float explosionSoundVolume = 1f;

    private AudioSource audioSource;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (flashOverlay == null)
        {
            CreateFlashOverlay();
        }
        else
        {
            flashOverlay.enabled = false;
        }

        if (blackScreen == null)
        {
            CreateBlackScreen();
        }
        else
        {
            blackScreen.enabled = false;
        }
    }

    private void CreateFlashOverlay()
    {
        GameObject flashObj = new GameObject("FlashOverlay");
        flashOverlay = flashObj.AddComponent<SpriteRenderer>();

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        Sprite whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        flashOverlay.sprite = whiteSprite;

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        flashOverlay.transform.localScale = new Vector3(width, height, 1);
        flashOverlay.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        flashOverlay.sortingOrder = 1000;
        flashOverlay.enabled = false;
    }

    private void CreateBlackScreen()
    {
        GameObject blackObj = new GameObject("BlackScreen");
        blackScreen = blackObj.AddComponent<SpriteRenderer>();

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.black);
        tex.Apply();

        Sprite blackSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        blackScreen.sprite = blackSprite;

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        blackScreen.transform.localScale = new Vector3(width, height, 1);
        blackScreen.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        blackScreen.sortingOrder = 1001;
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.enabled = false;
    }

    private IEnumerator FadeToBlack()
    {
        if (blackScreen == null) yield break;

        blackScreen.enabled = true;
        float elapsed = 0f;

        while (elapsed < fadeToBlackDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeToBlackDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = Color.black;
    }

    public void TriggerExplosion()
    {
        StartCoroutine(ExplosionSequence());
    }

    public float GetTotalExplosionDuration()
    {
        float preShake = 1f;
        float mainExplosion = delayBeforeExplosion;
        float fadeTime = fadeToBlackDuration;

        return preShake + mainExplosion + fadeTime + 0.5f;
    }

    private IEnumerator ExplosionSequence()
    {
        yield return StartCoroutine(PreExplosionShake());

        StartCoroutine(FlashEffect());

        StartCoroutine(CameraShake());

        yield return new WaitForSeconds(delayBeforeExplosion);

        if (backgroundSprite != null)
            backgroundSprite.enabled = false;

        yield return StartCoroutine(FadeToBlack());
    }

    private IEnumerator PreExplosionShake()
    {
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound, explosionSoundVolume);
        }

        Vector3 originalPos = mainCamera.transform.position;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float intensity = Mathf.Lerp(0.1f, 0.5f, elapsed / duration);
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

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

    private IEnumerator FlashEffect()
    {
        if (flashOverlay == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            flashOverlay.enabled = true;
            flashOverlay.color = new Color(1, 1, 1, 1);

            yield return new WaitForSeconds(flashDuration);

            flashOverlay.enabled = false;

            yield return new WaitForSeconds(flashDuration * 0.5f);
        }
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.position;

        for (int i = 0; i < shakeCount; i++)
        {
            float currentIntensity = shakeIntensity * (1f - (i / (float)shakeCount));
            float elapsed = 0f;
            float duration = shakeDuration / shakeCount;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * currentIntensity;
                float y = Random.Range(-1f, 1f) * currentIntensity;

                mainCamera.transform.position = new Vector3(
                    originalPos.x + x,
                    originalPos.y + y,
                    originalPos.z
                );

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        mainCamera.transform.position = originalPos;
    }

    private void ShatterBackground()
    {
        if (backgroundSprite == null) return;

        Sprite sprite = backgroundSprite.sprite;
        Texture2D texture = sprite.texture;

        Bounds bounds = backgroundSprite.bounds;
        Vector3 center = bounds.center;
        float width = bounds.size.x;
        float height = bounds.size.y;

        float pieceWidth = width / gridWidth;
        float pieceHeight = height / gridHeight;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float posX = center.x - (width / 2f) + (x * pieceWidth) + (pieceWidth / 2f);
                float posY = center.y - (height / 2f) + (y * pieceHeight) + (pieceHeight / 2f);
                Vector3 piecePos = new Vector3(posX, posY, 0);

                GameObject piece = new GameObject($"BackgroundPiece_{x}_{y}");
                piece.transform.position = piecePos;

                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = backgroundSprite.sortingOrder;

                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size = new Vector2(pieceWidth, pieceHeight);

                Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
                rb.gravityScale = 1f;

                Vector2 direction = (piecePos - center).normalized;
                direction += new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                rb.angularVelocity = Random.Range(-360f, 360f);

                StartCoroutine(FadePiece(sr, pieceLifetime));

                Destroy(piece, pieceLifetime);
            }
        }
    }

    private IEnumerator FadePiece(SpriteRenderer sr, float lifetime)
    {
        yield return new WaitForSeconds(lifetime * 0.7f);

        float fadeTime = lifetime * 0.3f;
        float elapsed = 0f;
        Color originalColor = sr.color;

        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}