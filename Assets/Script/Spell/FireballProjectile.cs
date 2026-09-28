using UnityEngine;

public class FireballProjectile : MonoBehaviour,IShieldBlockable
{
    [SerializeField] private bool canPiercingShield;

    public bool CanPierceShield => canPiercingShield;

    [SerializeField] private float speed = 30f;
    [SerializeField] private int damage = 1;
    private Rigidbody2D rb;

    private void Awake()
    {
        canPiercingShield = false;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

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
                Destroy(gameObject);

            }
        }
    }

    public void OnBlockedByShield() { 
        Destroy(gameObject);    
    }
}
