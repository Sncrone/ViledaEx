using UnityEngine;
using TMPro;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    [Header("Ipucu Ayarlari")]
    public int totalClues = 3;
    private int collectedClues = 0;

    [Header("UI")]
    public TextMeshProUGUI clueText;

    [Header("Gecis Ayarlari")]
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

        Debug.Log("Ipucu toplandi! Toplam: " + collectedClues + "/" + totalClues);

        if (collectedClues >= totalClues)
        {
            AllCluesCollected();
        }
    }

    void UpdateUI()
    {
        if (clueText != null)
        {
            clueText.text = "Ipuclari: " + collectedClues + "/" + totalClues;
        }
    }

    void AllCluesCollected()
    {
        Debug.Log("Tüm ipuclari toplandi! Gecmise gidiliyor...");

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