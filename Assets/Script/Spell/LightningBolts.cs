using UnityEngine;

public class LightningBolts : Spell
{
    public static LightningBolts Instance { get; private set; }
    [SerializeField] private LightningBolt lightningBoltPrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void CastSpell(PlayerMovement playerMovement)
    {
        base.CastSpell(playerMovement);

        Transform castPoint = playerMovement.spellCastSpot;

        GameObject lightningGO = SimplePoolManager.Instance.Spawn(lightningBoltPrefab.gameObject, castPoint.position, castPoint.rotation);
        lightningGO.transform.SetParent(castPoint);
        lightningGO.transform.localScale = lightningBoltPrefab.transform.localScale;
        lightningGO.transform.localPosition = Vector3.zero;
        lightningGO.transform.localRotation = Quaternion.identity;

        LightningBolt lightningBolt = lightningGO.GetComponent<LightningBolt>();
        lightningBolt.Setup();
    }
}
