using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject optionsPanel;
    
    [Header("Volume")]
    public Slider volumeSlider;
    public TMPro.TextMeshProUGUI volumeText;

    void Start()
    {
        // Panel başlangıçta kapalı
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        
        // Slider ayarları
        SetupVolumeSlider();
    }

    void SetupVolumeSlider()
    {
        if (volumeSlider == null) return;
        
        // Kayıtlı değeri yükle
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        volumeSlider.value = savedVolume;
        
        // Listener ekle (önce temizle, çift ekleme olmasın)
        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.AddListener(SetVolume);
        
        // İlk değeri göster
        UpdateVolumeText(savedVolume);
    }

    void OnEnable()
    {
        // Panel her açıldığında slider'ı güncelle
        if (volumeSlider != null)
        {
            float currentVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            volumeSlider.value = currentVolume;
            UpdateVolumeText(currentVolume);
        }
    }

    public void OpenOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }
    
    public void SetVolume(float volume)
    {
        // MusicManager'ı güncelle
        MusicManager.SetVolume(volume);
        
        // Text güncelle
        UpdateVolumeText(volume);
        
        // Kaydet
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    
    void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            volumeText.text = percentage + "%";
        }
    }
}