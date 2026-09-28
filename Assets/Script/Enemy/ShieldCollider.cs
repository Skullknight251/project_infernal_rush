using UnityEngine;

public class ShieldCollider : MonoBehaviour
{
    private Enemy enemy;

    public void Awake()
    {
        
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy == null)
            return;

        if (collision.TryGetComponent(out IShieldBlockable blockable))
        {
            if (!blockable.CanPierceShield)
            {
                blockable.OnBlockedByShield();
                Debug.Log("Blocked: " + blockable);
            }
        }
    }
}
