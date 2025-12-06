using UnityEngine;
using UnityEngine.UI;

public class RainEffect : MonoBehaviour
{
    [Header("Yağmur Ayarları")]
    public int rainDropCount = 50; // KÜÇÜK BAŞLA!
    public float rainSpeed = 400f;
    public float rainLength = 25f;
    public float rainWidth = 1.5f;
    public Color rainColor = new Color(0.7f, 0.8f, 1f, 0.4f);
    
    [Header("Rüzgar")]
    public float windStrength = 15f;
    
    private GameObject[] rainDrops;
    private RectTransform canvasRect;

    void Start()
    {
        canvasRect = GetComponent<RectTransform>();
        CreateRain();
    }

    void CreateRain()
    {
        rainDrops = new GameObject[rainDropCount];
        
        for (int i = 0; i < rainDropCount; i++)
        {
            GameObject drop = new GameObject("Rain_" + i);
            drop.transform.SetParent(transform, false);
            
            Image img = drop.AddComponent<Image>();
            img.color = rainColor;
            img.raycastTarget = false;
            
            RectTransform rt = drop.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rainWidth, rainLength);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            
            // Rastgele başlangıç pozisyonu
            float randomX = Random.Range(-960f, 960f);
            float randomY = Random.Range(-540f, 800f);
            rt.anchoredPosition = new Vector2(randomX, randomY);
            
            rainDrops[i] = drop;
        }
    }

    void Update()
    {
        foreach (GameObject drop in rainDrops)
        {
            if (drop == null) continue;
            
            RectTransform rt = drop.GetComponent<RectTransform>();
            
            // Aşağı + rüzgar hareketi
            Vector2 movement = new Vector2(windStrength, -rainSpeed) * Time.deltaTime;
            rt.anchoredPosition += movement;
            
            // Ekranın altına ulaştıysa yukarı taşı
            if (rt.anchoredPosition.y < -600f)
            {
                float randomX = Random.Range(-960f, 960f);
                rt.anchoredPosition = new Vector2(randomX, 700f);
            }
            
            // Çok sağa gittiyse sola taşı
            if (rt.anchoredPosition.x > 1000f)
            {
                rt.anchoredPosition = new Vector2(-960f, rt.anchoredPosition.y);
            }
        }
    }
}
