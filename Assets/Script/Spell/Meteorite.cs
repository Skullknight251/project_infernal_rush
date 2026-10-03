using System.Collections;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public float speed;
    private Vector2 direction;
    [SerializeField] private int damage = 4;
    private Coroutine deactivateCoroutine;

    public void Setup(float speed, Vector2 direction)
    {
        this.speed = speed;
        this.direction = direction.normalized;

        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime(2f));
    }

    private IEnumerator DeactivateAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        SimplePoolManager.Instance.Despawn(gameObject);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void OnTriggerEnter2D(Collider2D other)
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
}
