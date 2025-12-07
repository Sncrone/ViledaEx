using UnityEngine;

public class RepairInventory : MonoBehaviour
{
    private bool hasRepairKit = false;

    public void SetHasRepairKit(bool value)
    {
        hasRepairKit = value;
    }

    public bool HasRepairKit()
    {
        return hasRepairKit;
    }
}