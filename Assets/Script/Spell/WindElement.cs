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
        base.CastSpell(playerMovement);
        Transform castPoint = playerMovement.spellCastSpot;
        GameObject windSlashGO = SimplePoolManager.Instance.Spawn(windSlashPrefab.gameObject, castPoint.position, castPoint.rotation);
        windSlashGO.transform.localScale = windSlashPrefab.transform.localScale;

        WindSlash windSlash = windSlashGO.GetComponent<WindSlash>();
        windSlash.Setup(castPoint.right);
        
    }
}
