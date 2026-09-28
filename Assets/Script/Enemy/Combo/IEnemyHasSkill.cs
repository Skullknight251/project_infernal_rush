using System.Collections;
using UnityEngine;

public interface IEnemyHasSkill 
{
    public enum EnemySkillType
    {
        Skill1,
        Skill2
    }
    IEnumerator ExecuteSkill(EnemySkillType skillType);
}
