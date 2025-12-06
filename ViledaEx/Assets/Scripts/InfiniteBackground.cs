using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform cam; // Kamera Transform'u (Inspector'dan sürükle)
    
    [Header("Parallax Ayarları")]
    [Range(0f, 1f)]
    public float parallaxEffect = 0.5f; // 0 = Sabıt kalır, 1 = Kamerayla aynı hızda hareket eder
    
    [Header("Arka Plan Ayarları")]
    public int cloneCount = 3; // Sahnede yan yana kaç tane kopya var? (Senin durumunda 3)
    
    private float spriteWidth; // Tek bir sprite'ın genişliği
    private float startPos; // Bu objenin başlangıç X pozisyonu

    void Start()
    {
        // Başlangıç pozisyonunu kaydet
        startPos = transform.position.x;
        
        // Sprite'ın gerçek genişliğini al
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            spriteWidth = sr.bounds.size.x - 0.3f;
        }
        else
        {
            Debug.LogError("SpriteRenderer bulunamadı! Bu script bir Sprite objesine eklenmeli.");
        }

        // Kamera referansı yoksa otomatik bul
        if (cam == null)
        {
            cam = Camera.main.transform;
        }
    }

    void Update()
    {
        if (cam == null) return;

        // Kameranın X pozisyonuna göre parallax hareketi hesapla
        float parallaxDistance = cam.position.x * parallaxEffect;
        
        // Objenin yeni pozisyonunu ayarla
        transform.position = new Vector3(startPos + parallaxDistance, transform.position.y, transform.position.z);

        // SONSUZ DÖNGÜ KONTROLÜ:
        // Kameranın parallax'sız takip ettiği sanal pozisyon
        float cameraRelativePos = cam.position.x * (1 - parallaxEffect);
        
        // Toplam döngü uzunluğu (3 sprite yan yana)
        float loopWidth = spriteWidth * cloneCount;

        // Eğer obje kameranın SAĞ tarafından tamamen çıktıysa
        if (cameraRelativePos > startPos + spriteWidth)
        {
            startPos += loopWidth; // Objeyi sıranın en sonuna taşı
        }
        // Eğer obje kameranın SOL tarafından tamamen çıktıysa
        else if (cameraRelativePos < startPos - spriteWidth)
        {
            startPos -= loopWidth; // Objeyi sıranın en başına taşı
        }
    }
}