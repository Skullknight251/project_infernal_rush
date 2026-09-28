using UnityEngine;

public class IceShieldPrefab : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    public void Awake()
    {
        Destroy(gameObject, destroyTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyWeapon weapon) && weapon.weaponType == EnemyWeapon.WeaponType.Ranged)
        {
            Destroy(weapon.gameObject);
        }
    }

}
