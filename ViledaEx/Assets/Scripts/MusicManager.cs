using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;
    
    [Header("Müzik Ayarları")]
    public AudioClip backgroundMusic;
    
    [Header("UI Referansı")]
    [SerializeField] public Slider volumeSlider;

    private void Awake()
    {
        // Singleton pattern - Sadece bir MusicManager olsun
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Scene değişince yok olmasın
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Arka plan müziğini başlat
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }

        // Slider varsa, ses değişikliklerini dinle
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(delegate { SetVolume(volumeSlider.value); });
            
            // Kayıtlı ses seviyesini yükle (varsa)
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
    }

    public static void SetVolume(float volume)
    {
        if (Instance != null && Instance.audioSource != null)
        {
            Instance.audioSource.volume = volume;
            // Ses seviyesini kaydet
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }

    public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        // Yeni clip verilmişse ata
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }

        // AudioSource'da clip yoksa hata ver
        if (audioSource.clip == null)
        {
            Debug.LogWarning("No audio clip assigned to AudioSource.");
            return;
        }

        // Şarkıyı sıfırlayıp baştan başlat
        if (resetSong)
        {
            audioSource.Stop();
            audioSource.Play();
        }
        // Şarkı çalmıyorsa başlat
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
