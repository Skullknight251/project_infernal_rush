using System;
using System.Collections;
using UnityEngine;
using static Weapon;

public class Celestial : Spell
{
    public static Celestial Instance { get; private set; }
    [SerializeField] private Meteorite meteoritePrefab;
    public const string METEORITE_TRIG = "Meteorite_trigger";
    private Animator animator;
    public float delayTime;
    public float speed;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    void Start()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.onCastingSpellAction += GameInput_onCastingSpellReleaseAction;
        }
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        TryCastSpellCharging();
    }

    private void GameInput_onCastingSpellReleaseAction(object sender, EventArgs e)
    {
        Vector2 direction = AimSkill.Instance.StopAim();
        charging = false;

        StartCoroutine(DelayBeforeAttack(speed, direction));
        ConsumeManaFast(castSpellManaCost);

        charging = false;
        chargingTime = 0f;
    }

    public IEnumerator DelayBeforeAttack(float speed, Vector2 direction)
    {
        if (PlayerMovement.Instance != null)
        {
            base.CastSpell(PlayerMovement.Instance);
        }

        yield return new WaitForSeconds(delayTime);
        MeteoriteSummon(speed, direction);
    }

    public void MeteoriteSummon(float speed, Vector2 direction)
    {
        Transform spawnSpot = PlayerMovement.Instance.skySpawnSpot;

        GameObject meteoriteGO = SimplePoolManager.Instance.Spawn(meteoritePrefab.gameObject, spawnSpot.position, spawnSpot.rotation);
        meteoriteGO.transform.localScale = meteoritePrefab.transform.localScale;

        Meteorite meteorite = meteoriteGO.GetComponent<Meteorite>();
        meteorite.Setup(speed, direction);
    }

    public void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.onCastingSpellAction -= GameInput_onCastingSpellReleaseAction;
        }
    }
}
