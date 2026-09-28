using UnityEngine;

public class IceBall : Spell
{
    public static IceBall Instance { get; private set; }
    [SerializeField] private IceShieldPrefab iceShieldPrefab;

    public override void CastSpell(PlayerMovement playerMovement)
    {
        Transform castPoint = playerMovement.spellCastSpot;

        IceShieldPrefab iceShield = Instantiate(iceShieldPrefab, castPoint);
        iceShield.transform.localScale = iceShieldPrefab.transform.localScale;

    }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    void Update()
    {
        
    }
}
