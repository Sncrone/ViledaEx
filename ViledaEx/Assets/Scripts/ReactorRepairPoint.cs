using UnityEngine;

public class ReactorRepairPoint : MonoBehaviour
{
    public GameObject repairPrompt;
    public SpriteRenderer backgroundRenderer;
    public Sprite cleanBackground;

    private bool playerInRange = false;
    private RepairInventory inventory;

    void Start()
    {
        if (repairPrompt != null) repairPrompt.SetActive(false);
        inventory = FindObjectOfType<RepairInventory>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory != null && inventory.HasRepairKit())
            {
                if (repairPrompt != null) repairPrompt.SetActive(false);
                if (backgroundRenderer != null && cleanBackground != null)
                {
                    backgroundRenderer.sprite = cleanBackground;
                    Debug.Log("Reaktör tamir edildi!");
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (inventory != null && inventory.HasRepairKit())
            {
                if (repairPrompt != null) repairPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (repairPrompt != null) repairPrompt.SetActive(false);
        }
    }
}