using System;
using System.Collections.Generic;
using UnityEngine;

public class SpearClone : MonoBehaviour, IShieldBlockable
{
    [SerializeField] private BoxCollider2D hitBox;
    public float speed;
    [SerializeField] private bool canPierceShield;

    public bool CanPierceShield => canPierceShield;

    [SerializeField] private float attackRange;

    [SerializeField] private Vector3 target;
    [SerializeField] private int damage;
    private Vector2 direction;
    public bool canPiercingAttack;

    void Start()
    {
        target = new Vector3(transform.position.x + attackRange, transform.position.y, 0);
    }

    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        //if (transform.position.x >= target.x)
        //{
        //    Destroy(gameObject);
        //}
        transform.position +=
        (Vector3)(direction * speed * Time.deltaTime);

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<HurtBox>(out HurtBox hurtBox))
        {
            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (!canPiercingAttack)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetPiercingShield(bool check)
    {
        canPierceShield = check;
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    public void AddDamage(int plusDamage)
    {
        damage += plusDamage;
    }

    public void OnBlockedByShield()
    {
        Destroy(gameObject);
    }
}
