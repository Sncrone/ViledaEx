using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleTeleport : MonoBehaviour
{
    [Header("Hedef Scene")]
    public string targetScene = "Scene_Past";
    
    [Header("Portal Kontrolü")]
    public KeyCode openPortalKey = KeyCode.Q;
    public Transform player; // Player referansı
    
    [Header("Portal Pozisyonu")]
    public float distanceFromPlayer = 2f; // Karakterden ne kadar uzakta
    public bool followPlayer = true; // Player hareket edince portal da hareket etsin mi?
    
    [Header("Animasyon")]
    public float fadeSpeed = 3f;
    
    private bool portalOpen = false;
    private SpriteRenderer portalSprite;
    private Collider2D portalCollider;
    private float currentAlpha = 0f;
    private bool playerFacingRight = true; // Karakterin yönü

    void Start()
    {
        portalSprite = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();
        
        // Player'ı otomatik bul (eğer atanmadıysa)
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
        // Başlangıçta şeffaf
        if (portalSprite != null)
        {
            Color c = portalSprite.color;
            c.a = 0f;
            portalSprite.color = c;
        }
        
        if (portalCollider != null)
            portalCollider.enabled = false;
    }

    void Update()
    {
        // Player yönünü tespit et
        DetectPlayerDirection();
        
        // Q tuşu kontrolü
        if (Input.GetKeyDown(openPortalKey))
        {
            if (portalOpen)
                ClosePortal();
            else
                OpenPortal();
        }
        
        // Portal açıksa pozisyonu güncelle
        if (portalOpen && followPlayer && player != null)
        {
            UpdatePortalPosition();
        }
        
        // Fade animasyonu
        AnimatePortal();
    }

    void DetectPlayerDirection()
    {
        if (player == null) return;
        
        // Karakterin scale'inden yönü anla
        playerFacingRight = player.localScale.x > 0;
    }

    void OpenPortal()
    {
        portalOpen = true;
        Debug.Log("Portal açılıyor...");
        
        // Portal'ı player'ın önüne yerleştir
        UpdatePortalPosition();
    }

    void ClosePortal()
    {
        portalOpen = false;
        Debug.Log("Portal kapanıyor...");
    }

    void UpdatePortalPosition()
    {
        if (player == null) return;
        
        // Karakterin yönüne göre portal pozisyonu
        float direction = playerFacingRight ? 1f : -1f;
        Vector3 portalPos = player.position + new Vector3(direction * distanceFromPlayer, 0, 0);
        
        transform.position = portalPos;
    }

    void AnimatePortal()
    {
        if (portalSprite == null) return;
        
        float targetAlpha = portalOpen ? 1f : 0f;
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        
        Color c = portalSprite.color;
        c.a = currentAlpha;
        portalSprite.color = c;
        
        // Collider kontrolü
        if (portalCollider != null)
        {
            portalCollider.enabled = currentAlpha > 0.9f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentAlpha > 0.9f)
        {
            Debug.Log("Geçmişe ışınlanıyorsun...");
            SceneManager.LoadScene(targetScene);
        }
    }
}