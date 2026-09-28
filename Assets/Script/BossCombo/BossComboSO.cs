using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossCombo", menuName = "Boss/Boss Combo")]

public class BossComboSO : ScriptableObject
{
    [Header("Combo Settings")]
    public bool waitForPreviousEnemiesToDie = false;

    public string comboName;
    public List<BossActionSO> actionsInCombo; 
    public float cooldownAfterCombo = 3f;    
}