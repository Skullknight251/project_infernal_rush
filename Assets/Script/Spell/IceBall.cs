using UnityEngine;

public class IceBall : Spell
{
    public static IceBall Instance { get; private set; }
    [SerializeField] private IceShieldPrefab iceShieldPrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void CastSpell(PlayerMovement playerMovement)
    {
        base.CastSpell(playerMovement);

        Transform castPoint = playerMovement.spellCastSpot;

        GameObject iceShieldGO = SimplePoolManager.Instance.Spawn(iceShieldPrefab.gameObject, castPoint.position, castPoint.rotation);

        iceShieldGO.transform.SetParent(castPoint);
        iceShieldGO.transform.localScale = iceShieldPrefab.transform.localScale;
        iceShieldGO.transform.localPosition = Vector3.zero;
        iceShieldGO.transform.localRotation = Quaternion.identity;

        IceShieldPrefab iceShield = iceShieldGO.GetComponent<IceShieldPrefab>();
        iceShield.Setup();
    }
}
