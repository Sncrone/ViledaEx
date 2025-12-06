using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect;
    
    // YENİ: Sahnede yan yana kaç tane kopya resim var? (Sen 3 tane yaptın)
    // Bunu Inspector'dan değiştirebilirsin ama varsayılan 3 olsun.
    public int cloneCount = 3; 

    void Start()
    {
        startpos = transform.position.x;
        // Resmin genişliğini alıyoruz
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Kameranın parallax efektine göre sanal pozisyonu (Temp)
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        
        // Objenin ne kadar kayacağı (Dist)
        float dist = (cam.transform.position.x * parallaxEffect);

        // Hareketi uygula
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // MATEMATİK DÜZELTME KISMI:
        // Eğer resim kameranın görüşünden çıktıysa, onu sadece 1 birim değil,
        // toplam kopya sayısı kadar (3 birim) uzağa fırlat ki sıranın EN SONUNA geçsin.
        
        // Sınırı biraz geniş tutuyoruz (length/2 yerine direkt length kullandık ki titreme yapmasın)
        if (temp > startpos + length) 
        {
            startpos += length * cloneCount; // BURASI DEĞİŞTİ: length yerine (length * 3)
        }
        else if (temp < startpos - length) 
        {
            startpos -= length * cloneCount; // BURASI DEĞİŞTİ
        }
    }
}