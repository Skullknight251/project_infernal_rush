using System.Collections;
using UnityEngine;

public interface IShooter
{
    public enum EnemyShootType
    {
        Shoot1,
        Shoot2
    }
    IEnumerator ExecuteShoot(EnemyShootType shootType);
}