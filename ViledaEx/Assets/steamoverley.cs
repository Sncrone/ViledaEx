using UnityEngine;
using UnityEngine.UI;

public class SteamEffect : MonoBehaviour
{
    [Header("Buhar Ayarları")]
    public int steamCount = 20;
    public Sprite steamSprite;
    public float minSize = 50f;
    public float maxSize = 150f;
    public float steamSpeed = 30f;
    public Color steamColor = new Color(0.8f, 0.9f, 1f, 0.15f);
    
    [Header("Animasyon")]
    public float fadeSpeed = 0.5f;
    public float rotationSpeed = 10f;
    public float scaleSpeed = 0.3f;
    
    private GameObject[] steamClouds; // SteamCloud[] DEĞİL, GameObject[] OLMALI
    private float[] lifetimes; // BU EKLENMİŞTİ AMA TANIMLANMAMIŞ
    private RectTransform canvasRect;

    void Start()
    {
        canvasRect = GetComponent<RectTransform>();
        CreateSteam();
    }

    void CreateSteam()
    {
        // Sprite kontrolü
        if (steamSprite == null)
        {
            Debug.LogError("Steam Sprite atanmadı! Inspector'dan buhar sprite'ını ekle.");
            return;
        }
        
        steamClouds = new GameObject[steamCount];
        lifetimes = new float[steamCount];
        
        for (int i = 0; i < steamCount; i++)
        {
            GameObject cloud = new GameObject("Steam_" + i);
            cloud.transform.SetParent(transform, false);
            
            Image img = cloud.AddComponent<Image>();
            img.sprite = steamSprite;
            img.color = steamColor;
            img.raycastTarget = false;
            
            RectTransform rt = cloud.GetComponent<RectTransform>();
            float size = Random.Range(minSize, maxSize);
            rt.sizeDelta = new Vector2(size, size);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            
            float randomX = Random.Range(-960f, 960f);
            rt.anchoredPosition = new Vector2(randomX, -600f);
            rt.localScale = Vector3.one * Random.Range(0.5f, 1f);
            
            steamClouds[i] = cloud;
            lifetimes[i] = 0f;
        }
    }

    void Update()
    {
        for (int i = 0; i < steamClouds.Length; i++)
        {
            if (steamClouds[i] == null) continue;
            
            RectTransform rt = steamClouds[i].GetComponent<RectTransform>();
            Image img = steamClouds[i].GetComponent<Image>();
            
            // Yukarı hareket
            rt.anchoredPosition += Vector2.up * steamSpeed * Time.deltaTime;
            
            // Yatay dalgalanma
            float wave = Mathf.Sin(Time.time * 2f + i) * 10f;
            rt.anchoredPosition += Vector2.right * wave * Time.deltaTime;
            
            // Dönme
            rt.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            // Büyüme
            rt.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
            
            // Fade out
            lifetimes[i] += Time.deltaTime;
            float alpha = Mathf.Lerp(0.2f, 0f, lifetimes[i] / 10f);
            img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
            
            // Yeniden başlatma
            if (rt.anchoredPosition.y > 700f || lifetimes[i] > 10f)
            {
                float randomX = Random.Range(-960f, 960f);
                rt.anchoredPosition = new Vector2(randomX, -600f);
                rt.localScale = Vector3.one * Random.Range(0.5f, 1f);
                img.color = steamColor;
                lifetimes[i] = 0f;
            }
        }
    }
}