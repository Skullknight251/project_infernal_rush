using UnityEngine;

[CreateAssetMenu()]
public class SkillSO : ScriptableObject
{

    public enum ButtonType
    {
        Jump,
        BasicAttack,
        Block,
        PlungeAttack,
        SpellSkill,
        SwapWeapon,
        SwapSpell
    }

    public ButtonType buttonType;
    public GameObject weapon;
    public GameObject prefab; 
}
