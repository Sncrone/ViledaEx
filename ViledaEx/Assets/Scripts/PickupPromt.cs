using UnityEngine;
using TMPro;

// Sadece E tuþu bildirimi gösteren script
public class PickupPrompt : MonoBehaviour
{
    [Header("Bildirim UI")]
    [SerializeField] private GameObject promptUI; // "E tuþuna bas" yazýsý

    void Start()
    {
        // Baþlangýçta gizle
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Player objeye yaklaþtý
        if (other.CompareTag("Player"))
        {
            // Bildirimi göster
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Player uzaklaþtý
        if (other.CompareTag("Player"))
        {
            // Bildirimi gizle
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }
}

// Alternatif: Objenin üstünde sprite göster
public class PickupPromptSprite : MonoBehaviour
{
    [Header("E Sprite")]
    [SerializeField] private GameObject eSprite; // E harfi sprite'ý (child obje)

    void Start()
    {
        // Baþlangýçta gizle
        if (eSprite != null)
        {
            eSprite.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (eSprite != null)
            {
                eSprite.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (eSprite != null)
            {
                eSprite.SetActive(false);
            }
        }
    }
}