using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;
    
    [Header("Müzik Ayarları")]
    public AudioClip backgroundMusic;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            
            // AudioSource ayarları
            if (audioSource != null)
            {
                audioSource.loop = true;
                audioSource.playOnAwake = false;
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Sadece ilk instance'da çalışsın
        if (Instance != this) return;
        
        // Arka plan müziğini başlat
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
        
        // Kayıtlı ses seviyesini yükle
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        SetVolume(savedVolume);
    }

    public static void SetVolume(float volume)
    {
        if (Instance != null && Instance.audioSource != null)
        {
            Instance.audioSource.volume = volume;
        }
    }
}