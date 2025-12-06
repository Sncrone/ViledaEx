using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Ayarlarý")]
    [Tooltip("Hover'da ne kadar büyüsün (1.1 = %10 büyüme)")]
    public float hoverScale = 1.1f;

    [Tooltip("Animasyon hýzý")]
    public float animationSpeed = 0.2f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovering = false;

    void Start()
    {
        // Baþlangýç boyutunu kaydet
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // Smooth scale animasyonu
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * 10f * Time.deltaTime);
    }

    // Fare üzerine gelince
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = originalScale * hoverScale;
    }

    // Fare çýkýnca
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = originalScale;
    }
}