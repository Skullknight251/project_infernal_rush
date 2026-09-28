using Unity.VisualScripting;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private Enemy enemy;
   
    private int playerCollideCount;

    private void Awake()
    {
        playerCollideCount = 0;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "StartLine")
        {
            enemy.SetDeadByPlayer(false);
            enemy.TakeDamage(100);
            
        }
        if (collider.TryGetComponent(out Player player))
        {
            if (playerCollideCount == 0)
            {
                player.TakeDamage(enemy.damage);
                playerCollideCount++;
            }
        }
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }
}