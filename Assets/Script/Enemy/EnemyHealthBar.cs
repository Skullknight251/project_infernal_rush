using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Image healthAmount;
    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    //public void SetEnemy(Enemy enemy)
    //{
    //    this.enemy = enemy;
    //    Debug.Log(enemy.name);
    //}
    void Update()
    {
        
    }
    public void UpdateVisual()
    {
        if (enemy == null) return;
        healthAmount.fillAmount = (float)enemy.currentHealth / enemy.health;
    }
}
