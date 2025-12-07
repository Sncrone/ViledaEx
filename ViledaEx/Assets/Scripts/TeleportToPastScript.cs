using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleTeleport : MonoBehaviour
{
    [Header("Hedef Scene")]
    public string targetScene = "Scene_Past";
    
    [Header("Portal Kontrolü")]
    public KeyCode openPortalKey = KeyCode.Q;
    public Transform player;
    
    [Header("Portal Pozisyonu")]
    public float distanceFromPlayer = 2f;
    public bool followPlayer = true;
    
    [Header("Animasyon")]
    public float fadeSpeed = 3f;
    
    [Header("Ses")]
    public AudioClip portalSound;
    public float portalSoundVolume = 0.7f;
    
    private AudioSource audioSource;
    private bool portalOpen = false;
    private SpriteRenderer portalSprite;
    private Collider2D portalCollider;
    private float currentAlpha = 0f;
    private bool playerFacingRight = true;

    void Start()
    {
        portalSprite = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        
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
        DetectPlayerDirection();
        
        if (Input.GetKeyDown(openPortalKey))
        {
            if (portalOpen)
                ClosePortal();
            else
                OpenPortal();
        }
        
        if (portalOpen && followPlayer && player != null)
        {
            UpdatePortalPosition();
        }
        
        AnimatePortal();
    }

    void DetectPlayerDirection()
    {
        if (player == null) return;
        
        playerFacingRight = player.localScale.x > 0;
    }

    void OpenPortal()
    {
        portalOpen = true;
        Debug.Log("Portal açılıyor...");
        
        if (portalSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(portalSound, portalSoundVolume);
        }
        
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