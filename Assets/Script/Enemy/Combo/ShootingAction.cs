using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAction", menuName = "Enemy Actions/Shoot Action")]
public class ShootAction : EnemyActionSO
{
    [Header("Shoot Settings")]
    public int bulletCount = 3;
    public float fireRate = 3f;
    public IShooter.EnemyShootType shootType;
    public override IEnumerator ExecuteAction(Enemy enemy)
    {
        if (enemy.TryGetComponent(out IShooter shooter))
        {
            for (int i = 0; i < bulletCount; i++)
            {
                yield return enemy.StartCoroutine(shooter.ExecuteShoot(shootType));
                yield return new WaitForSeconds(fireRate);
            }
        }
        
    }
}