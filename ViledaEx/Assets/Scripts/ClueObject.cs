using UnityEngine;

public class ClueObject : MonoBehaviour
{
    [Header("Ayarlar")]
    public string clueName = "Ýpucu";
    public KeyCode collectKey = KeyCode.E;
    public float interactionDistance = 2f;

    [Header("Görsel")]
    public GameObject promptUI; // "E tuþuna bas" göstergesi (opsiyonel)

    private Transform player;
    private bool isCollected = false;
    private bool playerNearby = false;

    void Start()
    {
        // Player'ý bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isCollected || player == null) return;

        // Player'a mesafeyi kontrol et
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionDistance)
        {
            if (!playerNearby)
            {
                playerNearby = true;
                ShowPrompt();
            }

            // E tuþuna basýldý mý?
            if (Input.GetKeyDown(collectKey))
            {
                CollectClue();
            }
        }
        else
        {
            if (playerNearby)
            {
                playerNearby = false;
                HidePrompt();
            }
        }
    }

    void CollectClue()
    {
        isCollected = true;

        Debug.Log(clueName + " toplandý!");

        // ClueManager'a bildir
        if (ClueManager.Instance != null)
        {
            ClueManager.Instance.CollectClue();
        }

        HidePrompt();

        // Objeyi yok et veya gizle
        Destroy(gameObject);
    }

    void ShowPrompt()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(true);
        }
        Debug.Log("[E] tuþuna bas - " + clueName);
    }

    void HidePrompt()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    // Mesafeyi görsel olarak göster (Scene view'da)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}