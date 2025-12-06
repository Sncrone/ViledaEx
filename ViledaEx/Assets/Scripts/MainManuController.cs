using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Butona týklandýðýnda çalýþacak fonksiyon
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Direkt Scene_Future'a gitmek için
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene_Future2077");
    }

    // Oyundan çýkmak için (isteðe baðlý)
    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýlýyor...");
        Application.Quit();
    }
}