using Unity.VisualScripting;
using UnityEngine;

public class WindElement : Spell
{
    public static WindElement Instance { get; private set; }
    [SerializeField] private WindSlash windSlashPrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    public override void CastSpell(PlayerMovement playerMovement)
    {
        Transform castPoint = playerMovement.spellCastSpot;
        WindSlash windSlash = Instantiate(windSlashPrefab,castPoint.transform.position,castPoint.transform.rotation);
        windSlash.Setup(castPoint.right);
    }
}
