using UnityEngine;

public class NormalAttackTrigger : MonoBehaviour
{
    private Enemy enemy;
   
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            if (enemy.combo == null )
            {
                enemy.OnHitPlayer();
            }
        }
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }
}
