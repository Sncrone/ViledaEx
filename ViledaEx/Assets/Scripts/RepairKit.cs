using UnityEngine;

public class RepairKit : MonoBehaviour
{
    public GameObject ePrompt;

    private bool playerInRange = false;

    void Start()
    {
        if (ePrompt != null) ePrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            RepairInventory inventory = FindObjectOfType<RepairInventory>();
            if (inventory != null)
            {
                inventory.SetHasRepairKit(true);
                Debug.Log("Tamir kiti alýndý!");
            }

            if (ePrompt != null) ePrompt.SetActive(false);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (ePrompt != null) ePrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (ePrompt != null) ePrompt.SetActive(false);
        }
    }
}