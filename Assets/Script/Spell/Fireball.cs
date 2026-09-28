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
        Transform castPoint = playerMovement.spellCastSpot;

        FireballProjectile fireball = Instantiate(fireballPrefab,castPoint.position,castPoint.rotation);
        fireball.transform.localScale = fireballPrefab.transform.localScale;
        fireball.Setup(castPoint.right);
    }
}