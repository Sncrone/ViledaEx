using UnityEngine;
using TMPro;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    [Header("Ýpucu Ayarlarý")]
    public int totalClues = 3;
    private int collectedClues = 0;

    [Header("UI")]
    public TextMeshProUGUI clueText;

    [Header("Geçiþ Ayarlarý")]
    public bool autoTravelToPast = true;
    public float delayBeforeTravel = 2f;
    public string targetYear = "2045";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void CollectClue()
    {
        collectedClues++;
        UpdateUI();

        Debug.Log("Ýpucu toplandý! Toplam: " + collectedClues + "/" + totalClues);

        if (collectedClues >= totalClues)
        {
            AllCluesCollected();
        }
    }

    void UpdateUI()
    {
        if (clueText != null)
        {
            clueText.text = "Ýpuçlarý: " + collectedClues + "/" + totalClues;
        }
    }

    void AllCluesCollected()
    {
        Debug.Log("Tüm ipuçlarý toplandý! Geçmiþe gidiliyor...");

        if (autoTravelToPast)
        {
            Invoke("TravelToPast", delayBeforeTravel);
        }
    }

    void TravelToPast()
    {
        SceneTransitionManager.Instance.TravelToPast(collectedClues, targetYear);
    }
}