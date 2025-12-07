using System.Collections;
using UnityEngine;
using TMPro;

public class FinishSceneController : MonoBehaviour
{
    [Header("Player Ayarlarý")]
    public Transform player; // Player referansý
    public float autoWalkSpeed = 3f; // Otomatik yürüme hýzý

    [Header("Credits Baþlangýç")]
    public float walkDistanceToStartCredits = 5f; // Kaç birim yürüyünce credits baþlasýn

    [Header("Credits UI")]
    public GameObject creditsPanel; // Credits panel'i
    public TextMeshProUGUI creditsText; // Credits yazýsý
    public float scrollSpeed = 50f; // Yazýnýn kayma hýzý

    [Header("Credits Ýçeriði")]
    [TextArea(10, 30)]
    public string creditsContent = @"OYUN TAMAMLANDI!

Yapýmcý
[Ýsminiz]

Programlama
[Ýsminiz]

Sanat & Tasarým
[Ýsminiz]

Müzik
[Besteci Adý]

Özel Teþekkürler
[Yardýmcý Olanlar]

Oynamanýz Ýçin Teþekkürler!";

    [Header("Bitiþ")]
    public float creditsEndY = 2000f; // Credits ne kadar yukarý çýkýnca bitsin
    public string nextSceneName = "MainMenu"; // Credits bittikten sonra gidilecek sahne (boþ býrakýlýrsa kalýr)

    private Vector3 startPosition;
    private bool creditsStarted = false;
    private bool creditsFinished = false;
    private RectTransform creditsRect;
    private Rigidbody2D playerRb;
    private Animator playerAnim;

    void Start()
    {
        // Player'ý otomatik bul
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            startPosition = player.position;
            playerRb = player.GetComponent<Rigidbody2D>();
            playerAnim = player.GetComponent<Animator>();

            // Player'ý otomatik yürüt
            StartAutoWalk();
        }

        // Credits panel'ini hazýrla
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
            creditsRect = creditsPanel.GetComponent<RectTransform>();

            if (creditsText != null)
            {
                creditsText.text = creditsContent;
            }
        }
    }

    void Update()
    {
        // HER FRAME karakteri yürüt
        if (player != null && !creditsFinished && playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(autoWalkSpeed, playerRb.linearVelocity.y);
        }

        if (player != null && !creditsStarted && !creditsFinished)
        {
            // Player'ýn baþlangýçtan ne kadar yol aldýðýný hesapla
            float distanceTraveled = player.position.x - startPosition.x;

            if (distanceTraveled >= walkDistanceToStartCredits)
            {
                creditsStarted = true;
                StartCredits();
            }
        }

        // Credits akýþý
        if (creditsStarted && !creditsFinished && creditsRect != null)
        {
            // Credits'i yukarý kaydýr
            creditsRect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Credits bittiðinde
            if (creditsRect.anchoredPosition.y >= creditsEndY)
            {
                creditsFinished = true;
                EndCredits();
            }
        }
    }

    void LateUpdate()
    {
        // Animator parametrelerini son anda da zorla
        if (playerAnim != null && !creditsFinished)
        {
            playerAnim.SetFloat("Speed", autoWalkSpeed);
            playerAnim.SetBool("isWalking", true);
        }
    }

    void StartAutoWalk()
    {
        // Player kontrolünü kapat
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Otomatik saða yürüme
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(autoWalkSpeed, playerRb.linearVelocity.y);
        }

        // Yürüme animasyonu
        if (playerAnim != null)
        {
            playerAnim.SetFloat("Speed", autoWalkSpeed);
            playerAnim.SetBool("isWalking", true);
        }
    }

    void StartCredits()
    {
        Debug.Log("Credits baþladý!");

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);

            // Credits'i ekranýn alt kýsmýndan baþlat
            if (creditsRect != null)
            {
                creditsRect.anchoredPosition = new Vector2(0, -Screen.height);
            }
        }
    }

    void EndCredits()
    {
        Debug.Log("Credits tamamlandý!");

        // Bir sonraki sahneye geç (eðer ayarlanmýþsa)
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(3f); // 3 saniye bekle
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}