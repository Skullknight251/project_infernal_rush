using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyCombo", menuName = "Enemy/Enemy Combo")]
public class EnemyComboSO : ScriptableObject
{
    public string comboName;
    public List<EnemyActionSO> actionsInCombo;
}