using UnityEngine;

[CreateAssetMenu()]
public class SpellSO : ItemSO
{
    public enum SpellType
    {
        Fire,
        Ice,
        Lightning,
        Wind,
        Meteorite
    }
    public Spell spellPrefab;
    [SerializeField] public SpellType type;
}
