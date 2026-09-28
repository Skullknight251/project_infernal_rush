using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SkillAction", menuName = "Enemy Actions/Skill Action")]
public class SkillAction : EnemyActionSO
{
    [Header("Skill Settings")]
    public float skillCoolDownTime = 1.5f;
    public IEnemyHasSkill.EnemySkillType skillType;
    public override IEnumerator ExecuteAction(Enemy enemy)
    {
        if (enemy.TryGetComponent(out IEnemyHasSkill enemySkill))
        {
            yield return enemy.StartCoroutine(enemySkill.ExecuteSkill(skillType));
            yield return new WaitForSeconds(skillCoolDownTime);
        }

    }
}
