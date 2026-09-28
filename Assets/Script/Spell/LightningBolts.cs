using UnityEngine;

public class LightningBolts : Spell
{
    public static LightningBolts Instance {  get; private set; }
    [SerializeField] private LightningBolt lightningBoltPrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    public override void CastSpell(PlayerMovement playerMovement)
    {
        Transform castPoint = playerMovement.spellCastSpot;

        LightningBolt lightningBolt = Instantiate(lightningBoltPrefab, castPoint);
        lightningBolt.transform.localPosition = Vector3.zero;
        lightningBolt.transform.localRotation = Quaternion.identity;
    }
}
