
using System.Collections.Generic;
using UnityEngine;

public class ScytheTornado : MonoBehaviour
{
    [SerializeField] private CircleCollider2D hitBox;
    [SerializeField] private float speed;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float attackRange;
    
    [SerializeField] private Vector3 target;
    [SerializeField] private int damage;
    private List<Collider2D> hitsCollider = new List<Collider2D>();
    private ContactFilter2D contactFilter;
    private Scythe scythe;
    void Start()
    {

        target = new Vector3(transform.position.x + attackRange,transform.position.y, 0);
        contactFilter = ContactFilter2D.noFilter;
        contactFilter.useTriggers = true;
    }

    public void ExecuteSlashDamage()
    {
        hitsCollider.Clear();
        int count = hitBox.Overlap(contactFilter,hitsCollider);
        foreach (Collider2D collider in hitsCollider)
        {
            if (collider.TryGetComponent<HurtBox>(out HurtBox hurtBox))
            {
                Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    
                }
            }
        }
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,target,speed* Time.deltaTime);
        if (transform.position == target)
        {
            target = Player.Instance.transform.position;
        }
    }

    public void SetScythe(Scythe scythetmp)
    {
        scythe = scythetmp;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            scythe.playerIsHolding = true;
            Destroy(gameObject);
        }

    }
    public void AddDamage(int plusDamage)
    {
        damage += plusDamage;
    }

    public int GetDamage()
    {
        return damage;
    }
}
