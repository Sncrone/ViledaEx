using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform cam;
    
    [Header("Parallax Ayarları")]
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f;
    
    [Header("Arka Plan Ayarları")]
    public int cloneCount = 3;
    
    private float spriteWidth;
    private Vector3 lastCamPos;

    void Start()
    {
        // Kamera referansını bul
        if (cam == null)
        {
            cam = Camera.main.transform;
        }

        // Sprite genişliğini al (hafif overlap için -0.01)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            spriteWidth = sr.bounds.size.x - 0.3f;
        }
        else
        {
            Debug.LogError("SpriteRenderer bulunamadı!");
            enabled = false;
            return;
        }

        // Kameranın başlangıç pozisyonunu kaydet
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Kameranın ne kadar hareket ettiğini hesapla
        Vector3 deltaMovement = cam.position - lastCamPos;
        
        // Parallax etkisiyle objeyi hareket ettir
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, 0, 0);

        // Son kamera pozisyonunu güncelle
        lastCamPos = cam.position;

        // Sonsuz döngü kontrolü - DAHA HASSAS
        float totalWidth = spriteWidth * cloneCount;
        float distanceFromCamera = transform.position.x - cam.position.x;

        // Eğer obje kameranın çok gerisinde kaldıysa, öne al
        if (distanceFromCamera < -spriteWidth * 1.5f)
        {
            transform.position += new Vector3(totalWidth, 0, 0);
        }
        // Eğer obje kameranın çok önüne geçtiyse, geriye al
        else if (distanceFromCamera > spriteWidth * 1.5f)
        {
            transform.position -= new Vector3(totalWidth, 0, 0);
        }
    }
}
