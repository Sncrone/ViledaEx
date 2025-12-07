using UnityEngine;
using System.Collections;

public class BackgroundExplosion : MonoBehaviour
{
    [Header("Background Referansý")]
    [SerializeField] private SpriteRenderer backgroundSprite;

    [Header("Patlama Ayarlarý")]
    [SerializeField] private int gridWidth = 8; // Kaça kaç parçaya bölünecek
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float explosionForce = 15f;
    [SerializeField] private float pieceLifetime = 3f;

    [Header("Kamera Efektleri")]
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private int shakeCount = 3; // Kaç defa sarsýlacak

    [Header("Flash Efekti")]
    [SerializeField] private SpriteRenderer flashOverlay; // Beyaz flash için
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private int flashCount = 3;

    [Header("Siyah Ekran")]
    [SerializeField] private SpriteRenderer blackScreen; // Patlama sonrasý siyah ekran
    [SerializeField] private float fadeToBlackDuration = 1f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeExplosion = 0.5f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Flash overlay yoksa oluþtur
        if (flashOverlay == null)
        {
            CreateFlashOverlay();
        }
        else
        {
            flashOverlay.enabled = false;
        }

        // Siyah ekran yoksa oluþtur
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
        // Yeni GameObject oluþtur
        GameObject flashObj = new GameObject("FlashOverlay");
        flashOverlay = flashObj.AddComponent<SpriteRenderer>();

        // Beyaz texture oluþtur
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        // Sprite oluþtur
        Sprite whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        flashOverlay.sprite = whiteSprite;

        // Kamerayý kaplasýn
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        flashOverlay.transform.localScale = new Vector3(width, height, 1);
        flashOverlay.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        // En üstte görünsün
        flashOverlay.sortingOrder = 1000;
        flashOverlay.enabled = false;
    }

    private void CreateBlackScreen()
    {
        // Yeni GameObject oluþtur
        GameObject blackObj = new GameObject("BlackScreen");
        blackScreen = blackObj.AddComponent<SpriteRenderer>();

        // Siyah texture oluþtur
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.black);
        tex.Apply();

        // Sprite oluþtur
        Sprite blackSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        blackScreen.sprite = blackSprite;

        // Kamerayý kaplasýn
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        blackScreen.transform.localScale = new Vector3(width, height, 1);
        blackScreen.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        // En üstte görünsün (flash'tan bile üstte)
        blackScreen.sortingOrder = 1001;
        blackScreen.color = new Color(0, 0, 0, 0); // Baþta görünmez
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

    // Toplam patlama süresini hesapla
    public float GetTotalExplosionDuration()
    {
        float preShake = 1f; // PreExplosionShake süresi
        float mainExplosion = delayBeforeExplosion;
        float fadeTime = fadeToBlackDuration;

        return preShake + mainExplosion + fadeTime + 0.5f; // Küçük buffer
    }

    private IEnumerator ExplosionSequence()
    {
        // 1. Ön titreþim
        yield return StartCoroutine(PreExplosionShake());

        // 2. Flash efektleri
        StartCoroutine(FlashEffect());

        // 3. Ana kamera sarsma
        StartCoroutine(CameraShake());

        yield return new WaitForSeconds(delayBeforeExplosion);

        // 4. Background'u gizle
        if (backgroundSprite != null)
            backgroundSprite.enabled = false;

        // 5. Direkt siyah ekrana geçiþ
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
            // Flash aç
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

        // Background'un dünya pozisyonunu ve boyutunu al
        Bounds bounds = backgroundSprite.bounds;
        Vector3 center = bounds.center;
        float width = bounds.size.x;
        float height = bounds.size.y;

        float pieceWidth = width / gridWidth;
        float pieceHeight = height / gridHeight;

        // Her parça için
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Parça pozisyonu hesapla
                float posX = center.x - (width / 2f) + (x * pieceWidth) + (pieceWidth / 2f);
                float posY = center.y - (height / 2f) + (y * pieceHeight) + (pieceHeight / 2f);
                Vector3 piecePos = new Vector3(posX, posY, 0);

                // Parça oluþtur
                GameObject piece = new GameObject($"BackgroundPiece_{x}_{y}");
                piece.transform.position = piecePos;

                // Sprite Renderer ekle
                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = backgroundSprite.sortingOrder;

                // Sadece bu parçanýn bölgesini göster
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
}