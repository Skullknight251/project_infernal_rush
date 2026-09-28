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
    void Start()
    {
        GameInput.Instance.onCastingSpellAction += GameInput_onCastingSpellReleaseAction;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        TryCastSpellCharging();
    }
    private void GameInput_onCastingSpellAction(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void GameInput_onCastingSpellReleaseAction(object sender, EventArgs e)
    {

        Vector2 direction = AimSkill.Instance.StopAim();

        charging = false;

        StartCoroutine(
            DelayBeforeAttack(speed, direction)
        );
        ConsumeManaFast(castSpellManaCost);
        charging = false;
        chargingTime = 0f;
    }

    public IEnumerator DelayBeforeAttack(float speed, Vector2 direction)
    {
        yield return new WaitForSeconds(delayTime);
        MeteoriteSummon(speed,direction);
        Debug.Log("waiting");
    }

    public void MeteoriteSummon(float speed, Vector2 direction)
    {
        Meteorite meteorite = Instantiate(meteoritePrefab,PlayerMovement.Instance.skySpawnSpot.transform.position, PlayerMovement.Instance.skySpawnSpot.transform.rotation);
        meteorite.speed = speed;
        meteorite.SetDirection(direction);
    }

    public void OnDestroy()
    {
        GameInput.Instance.onCastingSpellAction -= GameInput_onCastingSpellReleaseAction;
    }
}
