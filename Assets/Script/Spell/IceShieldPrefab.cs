using System.Collections;
using UnityEngine;

public class IceShieldPrefab : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    private Coroutine deactivateCoroutine;

    public void Setup()
    {
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime(destroyTime));
    }

    private IEnumerator DeactivateAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        SimplePoolManager.Instance.Despawn(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyWeapon weapon) && weapon.weaponType == EnemyWeapon.WeaponType.Ranged)
        {
            Destroy(weapon.gameObject);
        }
    }
}
