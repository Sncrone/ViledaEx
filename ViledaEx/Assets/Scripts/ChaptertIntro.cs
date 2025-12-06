using System.Collections;
using UnityEngine;
using TMPro;

public class ChapterIntro : MonoBehaviour
{
    [Header("Metin Ayarlarý")]
    public string chapterText = "BÖLÜM 1: FIRTINA";
    public float typingSpeed = 0.05f; // Her harfin yazýlma hýzý
    public float displayDuration = 3f; // Metnin ekranda kalma süresi
    public float erasingSpeed = 0.03f; // Silme hýzý

    [Header("UI Referanslarý")]
    public TextMeshProUGUI chapterTextUI; // Text bileþeni
    public GameObject playerObject; // Player objesi (hareket için)
    public MonoBehaviour[] scriptsToDisable; // Devre dýþý býrakýlacak scriptler

    [Header("Ses Efektleri (Ýsteðe Baðlý)")]
    public AudioClip typingSound;
    public AudioClip eraseSound;

    private AudioSource audioSource;
    private bool introFinished = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Baþlangýçta tüm mekanikleri kapat
        DisableGameplay();

        // Intro'yu baþlat
        StartCoroutine(PlayChapterIntro());
    }

    void DisableGameplay()
    {
        // Player hareketini durdur
        if (playerObject != null)
        {
            PlayerMovement pm = playerObject.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false;
        }

        // Diðer scriptleri kapat
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    void EnableGameplay()
    {
        // Player hareketini aktif et
        if (playerObject != null)
        {
            PlayerMovement pm = playerObject.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = true;
        }

        // Diðer scriptleri aç
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }
    }

    IEnumerator PlayChapterIntro()
    {
        // Metni temizle
        chapterTextUI.text = "";

        // 1. AÞAMA: Daktilo efekti ile yazma
        foreach (char letter in chapterText.ToCharArray())
        {
            chapterTextUI.text += letter;

            // Ses efekti çal
            if (audioSource != null && typingSound != null)
            {
                audioSource.PlayOneShot(typingSound, 0.3f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // 2. AÞAMA: Metni ekranda tut
        yield return new WaitForSeconds(displayDuration);

        // 3. AÞAMA: Daktilo efekti ile silme
        while (chapterTextUI.text.Length > 0)
        {
            chapterTextUI.text = chapterTextUI.text.Substring(0, chapterTextUI.text.Length - 1);

            // Ses efekti çal
            if (audioSource != null && eraseSound != null)
            {
                audioSource.PlayOneShot(eraseSound, 0.2f);
            }

            yield return new WaitForSeconds(erasingSpeed);
        }

        // 4. AÞAMA: Mekanikleri aktif et
        yield return new WaitForSeconds(0.5f);
        EnableGameplay();

        introFinished = true;

        // Bu objeyi kapat (artýk gerekmez)
        gameObject.SetActive(false);
    }
}