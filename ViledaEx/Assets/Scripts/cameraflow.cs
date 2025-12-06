using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Takip edilecek obje (Senin karakterin)
    public float smoothSpeed = 0.125f; // Kameranın kayma yumuşaklığı
    public Vector3 offset; // Kameranın karakterden ne kadar uzakta duracağı

    void LateUpdate() // Hareketten sonra çalışması için LateUpdate kullanılır
    {
        if (target == null) return;

        // Hedef pozisyon: Sadece X ekseninde takip et, Y ve Z sabit kalsın (Arka plan bozulmasın diye)
        // Eğer karakter zıpladığında kamera da kalksın istiyorsan 'transform.position.y' yerine 'target.position.y' yazabilirsin.
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);
        
        // Yumuşak geçiş (Lerp) ile kamerayı taşı
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}