using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Sahne geçiþlerini yöneten ana manager
/// MainMenu -> Scene_Future2077 -> Scene_Past -> Scene_Result döngüsü
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Scene Ýsimleri")]
    public string mainMenuScene = "MainMenu";
    public string futureScene = "Scene_Future2077";
    public string pastScene = "Scene_Past";
    public string resultScene = "Scene_Result";

    [Header("Transition Ayarlarý")]
    public float transitionDuration = 1.5f;
    public Color fadeColor = Color.black;

    [Header("Efekt Ayarlarý (Opsiyonel)")]
    public bool useGlitchEffect = true;
    public float glitchDuration = 0.3f;

    // Oyun state'i
    private GameData currentGameData;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentGameData = new GameData();
    }

    #region Ana Geçiþ Metodlarý

    /// <summary>
    /// Main Menu'den Future sahnesine geç
    /// </summary>
    public void StartGame()
    {
        currentGameData.Reset();
        StartCoroutine(TransitionToScene(futureScene, TransitionType.FadeIn));
    }

    /// <summary>
    /// Future'dan Past'e zaman atlama (ipuçlarý toplandýktan sonra)
    /// </summary>
    public void TravelToPast(int cluesCollected, string targetYear)
    {
        currentGameData.cluesCollected = cluesCollected;
        currentGameData.targetYear = targetYear;

        if (useGlitchEffect)
        {
            StartCoroutine(TransitionWithGlitch(pastScene));
        }
        else
        {
            StartCoroutine(TransitionToScene(pastScene, TransitionType.TimeTravelEffect));
        }
    }

    /// <summary>
    /// Past'ten Future'a geri dön (görev tamamlandýktan sonra)
    /// </summary>
    public void ReturnToFuture(bool missionSuccess)
    {
        currentGameData.missionSuccess = missionSuccess;
        currentGameData.levelsCompleted++;

        StartCoroutine(TransitionToScene(futureScene, TransitionType.TimeTravelEffect));
    }

    /// <summary>
    /// Result sayfasýna git (level bitince veya oyun sonu)
    /// </summary>
    public void ShowResults()
    {
        StartCoroutine(TransitionToScene(resultScene, TransitionType.FadeIn));
    }

    /// <summary>
    /// Main Menu'ye geri dön
    /// </summary>
    public void ReturnToMainMenu()
    {
        StartCoroutine(TransitionToScene(mainMenuScene, TransitionType.FadeOut));
    }

    #endregion

    #region Transition Coroutine'leri

    private IEnumerator TransitionToScene(string sceneName, TransitionType type)
    {
        // Fade out efekti
        yield return StartCoroutine(FadeOut(transitionDuration / 2));

        // Sahne yükle
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade in efekti
        yield return StartCoroutine(FadeIn(transitionDuration / 2));
    }

    private IEnumerator TransitionWithGlitch(string sceneName)
    {
        // Glitch efekti (zaman atlama için)
        yield return StartCoroutine(GlitchEffect());

        // Normal transition
        yield return StartCoroutine(TransitionToScene(sceneName, TransitionType.TimeTravelEffect));
    }

    #endregion

    #region Efektler

    private IEnumerator FadeOut(float duration)
    {
        // Canvas Group veya baþka fade yöntemi kullanabilirsiniz
        // Basit versiyon için Camera'ya SpriteRenderer ekleyip alpha deðiþtirin

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / duration);

            // Burada fade efektinizi uygulayýn
            // Örnek: fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);

            // Burada fade efektinizi uygulayýn

            yield return null;
        }
    }

    private IEnumerator GlitchEffect()
    {
        // Zaman atlama için screen shake + glitch
        float elapsed = 0f;
        Vector3 originalPos = Camera.main.transform.position;

        while (elapsed < glitchDuration)
        {
            elapsed += Time.deltaTime;

            // Random shake
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            Camera.main.transform.position = originalPos + new Vector3(x, y, 0);

            yield return new WaitForSeconds(0.05f);
        }

        Camera.main.transform.position = originalPos;
    }

    #endregion

    #region Yardýmcý Metodlar

    public GameData GetCurrentGameData()
    {
        return currentGameData;
    }

    public void SaveProgress()
    {
        // PlayerPrefs veya baþka save sistemi
        PlayerPrefs.SetInt("LevelsCompleted", currentGameData.levelsCompleted);
        PlayerPrefs.Save();
    }

    #endregion
}

// Transition türleri
public enum TransitionType
{
    FadeIn,
    FadeOut,
    TimeTravelEffect,
    Instant
}

// Oyun verilerini tutan sýnýf
[System.Serializable]
public class GameData
{
    public int cluesCollected = 0;
    public string targetYear = "2040";
    public bool missionSuccess = false;
    public int levelsCompleted = 0;

    public void Reset()
    {
        cluesCollected = 0;
        targetYear = "2040";
        missionSuccess = false;
        levelsCompleted = 0;
    }
}

// ============================================
// KULLANIM ÖRNEKLERÝ
// ============================================

/// <summary>
/// Future2077 sahnesinde - Ýpuçlarý toplandýðýnda
/// </summary>
public class ClueCollector : MonoBehaviour
{
    private int totalClues = 0;
    private int requiredClues = 3;

    public void OnClueCollected()
    {
        totalClues++;

        if (totalClues >= requiredClues)
        {
            // Geçmiþe git!
            SceneTransitionManager.Instance.TravelToPast(totalClues, "2045");
        }
    }
}

/// <summary>
/// Past sahnesinde - Görev tamamlandýðýnda
/// </summary>
public class MissionController : MonoBehaviour
{
    public void OnMissionComplete(bool success)
    {
        // Geleceðe geri dön
        SceneTransitionManager.Instance.ReturnToFuture(success);
    }
}

/// <summary>
/// Result sahnesinde - Sonraki level veya main menu
/// </summary>
public class ResultScreen : MonoBehaviour
{
    public void OnNextLevel()
    {
        SceneTransitionManager.Instance.StartGame(); // Yeni level baþlat
    }

    public void OnMainMenu()
    {
        SceneTransitionManager.Instance.ReturnToMainMenu();
    }
}