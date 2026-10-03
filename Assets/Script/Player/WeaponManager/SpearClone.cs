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
    [SerializeField] private int baseDamage = 10;
    private int currentDamage;
    private Vector2 direction;
    public bool canPiercingAttack;

    private Vector3 startPosition;

    void OnEnable()
    {
        startPosition = transform.position;
        currentDamage = baseDamage;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (Vector3.Distance(startPosition, transform.position) >= attackRange)
        {
            SimplePoolManager.Instance.Despawn(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<HurtBox>(out HurtBox hurtBox))
        {
            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(currentDamage);

                if (!canPiercingAttack)
                {
                    SimplePoolManager.Instance.Despawn(gameObject);
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
        currentDamage += plusDamage;
    }

    public void OnBlockedByShield()
    {
        SimplePoolManager.Instance.Despawn(gameObject);
    }
}
