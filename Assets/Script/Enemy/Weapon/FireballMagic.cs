using UnityEngine;

public class FireballMagic : EnemyWeapon
{
    public override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        if (enemy.CanRun)
        {
            Fire();
            base.Update();
        }
    }

    public void Fire()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector2 direction = (target.transform.position - transform.position).normalized;
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        RotateToDirection(direction);
    }
}
