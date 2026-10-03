using UnityEngine;

public class Fireball : Spell
{
    public static Fireball Instance { get; private set; }
    [SerializeField] private FireballProjectile fireballPrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void CastSpell(PlayerMovement playerMovement)
    {
        base.CastSpell(playerMovement);

        Transform castPoint = playerMovement.spellCastSpot;

        GameObject fireballGO = SimplePoolManager.Instance.Spawn(fireballPrefab.gameObject, castPoint.position, castPoint.rotation);
        fireballGO.transform.localScale = fireballPrefab.transform.localScale;

        FireballProjectile fireball = fireballGO.GetComponent<FireballProjectile>();
        fireball.Setup(castPoint.right);
    }
}
