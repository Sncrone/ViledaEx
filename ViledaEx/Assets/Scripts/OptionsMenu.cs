using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsPanel;

    void Start()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}