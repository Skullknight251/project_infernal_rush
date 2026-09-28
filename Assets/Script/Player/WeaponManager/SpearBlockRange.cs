using UnityEngine;

public class SpearBlockRange : MonoBehaviour
{
    [SerializeField] private Spear spear;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyWeapon weapon) && weapon.weaponType == EnemyWeapon.WeaponType.Ranged)
        {
            Destroy(weapon.gameObject);
        }
    }

    public void SetSpear(Spear spear)
    {
        this.spear = spear;
    }

}
