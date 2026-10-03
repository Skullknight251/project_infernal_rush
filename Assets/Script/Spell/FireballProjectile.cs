using System.Collections;
using UnityEngine;

public class FireballProjectile : MonoBehaviour, IShieldBlockable
{
    [SerializeField] private bool canPierceShield;

    public bool CanPierceShield => canPierceShield;

    [SerializeField] private float speed = 30f;
    [SerializeField] private int damage = 1;
    private Rigidbody2D rb;
    private Coroutine deactivateCoroutine;

    private void Awake()
    {
        canPierceShield = false;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime(3f));
    }

    private IEnumerator DeactivateAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        SimplePoolManager.Instance.Despawn(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<HurtBox>(out HurtBox hurtBox))
        {
            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (deactivateCoroutine != null) StopCoroutine(deactivateCoroutine);
                SimplePoolManager.Instance.Despawn(gameObject);
            }
        }
    }

    public void OnBlockedByShield()
    {
        if (deactivateCoroutine != null) StopCoroutine(deactivateCoroutine);
        SimplePoolManager.Instance.Despawn(gameObject);
    }
}
