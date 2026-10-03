using System.Collections.Generic;
using UnityEngine;

public class ScytheTornado : MonoBehaviour
{
    [SerializeField] private CircleCollider2D hitBox;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;

    private Vector3 target;
    [SerializeField] private int baseDamage = 4;
    private int currentDamage;
    private List<Collider2D> hitsCollider = new List<Collider2D>();
    private ContactFilter2D contactFilter;
    private Scythe scythe;
    private bool isReturning;

    void Awake()
    {
        contactFilter = ContactFilter2D.noFilter;
        contactFilter.useTriggers = true;
    }

    public void Setup(Scythe scytheComponent, int bonusDamage)
    {
        scythe = scytheComponent;
        currentDamage = baseDamage + bonusDamage;
        isReturning = false;
        target = new Vector3(transform.position.x + attackRange, transform.position.y, 0);
    }

    public void ExecuteSlashDamage()
    {
        hitsCollider.Clear();
        int count = hitBox.Overlap(contactFilter, hitsCollider);
        foreach (Collider2D collider in hitsCollider)
        {
            if (collider.TryGetComponent<HurtBox>(out HurtBox hurtBox))
            {
                Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(currentDamage);
                }
            }
        }
    }

    void Update()
    {
        if (isReturning)
        {
            if (Player.Instance != null)
            {
                target = Player.Instance.transform.position;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (!isReturning && transform.position == target)
        {
            isReturning = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (scythe != null)
            {
                scythe.playerIsHolding = true;
            }
            SimplePoolManager.Instance.Despawn(gameObject);
        }
    }

    public int GetDamage()
    {
        return currentDamage;
    }
}
