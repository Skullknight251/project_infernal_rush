using Unity.VisualScripting;
using UnityEngine;

public class WindSlash : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    [SerializeField] private float speed = 30f;

    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized* speed;

        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<HurtBox>(out HurtBox hurtBox))
        {
            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                
            }
        }
    }
}
