using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private Vector3 originalScale; // Orijinal boyutu kaydet

    void Start()
    {
        animator = GetComponent<Animator>();

        // Başlangıç scale'ini kaydet
        originalScale = transform.localScale;

        if (animator == null)
        {
            Debug.LogError("Animator bulunamadı! Karaktere Animator component ekle.");
        }
    }

    void Update()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1f;
        }

        // Hareketi uygula
        transform.position += new Vector3(horizontal * moveSpeed * Time.deltaTime, 0, 0);

        // Karakteri döndür (flip)
        if (horizontal > 0)
        {
            // Sağa gidiyorsa, orijinal boyutta
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (horizontal < 0)
        {
            // Sola gidiyorsa, X ekseninde aynala ama boyutu koru
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Animatora hız gönder
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
        }
    }
}