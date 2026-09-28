using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public enum CanParry { True, False }
    public enum WeaponType { Melee, Ranged }
    [HideInInspector] public Vector2 direction; 
    [SerializeField] public EnemyWeaponSO weaponSO;
    [SerializeField] public CanParry canParry;
    [SerializeField] public Enemy enemy;
    [SerializeField] private int damage;
    [SerializeField] private bool canBeDestroy; 
    public float speedToPlayer;
    public float speedToOwner;
    public float lifeTimeMax;
    public WeaponType weaponType;
    public float lifeTime;
    public GameObject target;
    public float speed;
    

    public virtual void Start()
    {
        lifeTime = 0;
        //Debug.Log(this.name +" " +enemy);
    }
    protected virtual void Update()
    {
        if (enemy == null || Player.Instance == null)
        {
            Destroy(gameObject);
        }
        if (weaponType == WeaponType.Melee)
        {
            if (enemy != null && enemy.isStunned)
            {
                enemy.EnableHurtBoxWithDelay();
                Destroy(gameObject);
                return;
            }
        }
        lifeTime += Time.deltaTime;
        if (lifeTime >= lifeTimeMax)
        {
            Destroy(gameObject);
        }
    }
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (weaponType == WeaponType.Melee)
        {
            if (other.TryGetComponent(out Weapon weapon) && weapon.isParrying && canParry == CanParry.True && weapon.parryTarget == enemy)
            {
                if (enemy != null && !enemy.isStunned)
                {
                    enemy.StartStun();
                }

                weapon.SuccessfulParry();
                return;
            }
        }
        if (target == Player.Instance.gameObject && weaponType == WeaponType.Ranged)
        {
            
            if (other.TryGetComponent(out Weapon weapon)&& weapon.isParrying && canParry == CanParry.True)
            {
                if (enemy != null)
                {
                    SetTarget(enemy.gameObject);
                }
                weapon.SuccessfulParry();
                return;
            }
        }
        if (other.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
            if (canBeDestroy) Destroy(gameObject);
        }
        else if (enemy != null && other.TryGetComponent(out HurtBox hurtBox))
        {
            if (hurtBox.GetComponentInParent<Enemy>() is Enemy hurtBoxEnemy &&
                hurtBoxEnemy.gameObject == target)
            {
                hurtBoxEnemy.TakeDamage(damage);
                //Debug.Log("Bullet hit: " + hurtBoxEnemy.name);

                Destroy(gameObject);
            }
        }
    }

    
    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            lifeTime = 0;
            this.target = target;
            if (target.TryGetComponent(out Player player))
            {
                speed = speedToPlayer;
            }
            else speed = speedToOwner;
        }
    }

    public void SetEnemyReference(Enemy owner) { this.enemy = owner; }
    public void RotateToDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
