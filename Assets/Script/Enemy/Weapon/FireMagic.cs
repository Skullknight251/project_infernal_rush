using UnityEngine;

public class FireMagic : EnemyWeapon
{
    public override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        if (enemy.CanRun)
        {
            base.Update();
        }
    }
}
