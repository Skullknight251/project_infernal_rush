using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public float speed;
    private Vector2 direction;
    [SerializeField] private int damage = 4;
    void Start()
    {
        Destroy(gameObject, 2f);
    }
    public void Awake()
    {
    }
    void Update()
    {
        transform.position +=(Vector3)(direction * speed * Time.deltaTime);

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
                Destroy(gameObject);
            }
        }

    }
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    } 
}
