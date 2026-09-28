using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spear : Weapon {
    private const string BLOCK_TRIGGER = "BlockTrigger";
    public Animator animator;
    private ContactFilter2D filter;
    private List<Collider2D> hitsCollider = new List<Collider2D>();
    [SerializeField] private float delayTime = .5f;
    [SerializeField] private float defaultSpearSpeed = 30f;
    [SerializeField] private int OHAttackSpearDamage = 10;
    [SerializeField] private float OHAttackSpeed = 70;
    [SerializeField] private Transform spearClone;
    [SerializeField] private Transform spearCloneSpawner;
    [SerializeField] private int basicAttackDamage = 2;
    [SerializeField] private int overHeadAttackDamage = 4;
    [SerializeField] private GameObject blockRange;
    SpearBlockRange spearBlockRange;
    protected override void Start()
    {
        base.Start();
        spearBlockRange = blockRange.GetComponent<SpearBlockRange>();
        spearBlockRange.SetSpear(this);
        GameInput.Instance.onHeavyAttackReleaseAction += GameInput_onHeavyAttackReleaseAction;
        GameInput.Instance.onOverheadAttackReleaseAction += GameInput_onOverheadAttackReleaseAction;
        GameInput.Instance.onBlockAction += GameInput_onBlockAction;
    }

    private void GameInput_onBlockAction(object sender, EventArgs e)
    {
        animator.SetTrigger(BLOCK_TRIGGER);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameInput.Instance.onHeavyAttackReleaseAction -= GameInput_onHeavyAttackReleaseAction;
        GameInput.Instance.onOverheadAttackReleaseAction -= GameInput_onOverheadAttackReleaseAction; 
        GameInput.Instance.onBlockAction -= GameInput_onBlockAction;
    }
    private void GameInput_onOverheadAttackReleaseAction(object sender, EventArgs e)
    {
        if (chargingType != ChargingType.OverheadAttack)
            return;

        Debug.Log("OVERHEAD RELEASE");

        Vector2 direction = AimSkill.Instance.StopAim();

        overheadCharging = false;

        animator.SetTrigger(OVERHEAD_ATTACK_TRIG);

        StartCoroutine(DelayBeforeAttack(true,OHAttackSpearDamage,OHAttackSpeed,direction));

        ConsumeStaminaFast(overheadAttackStaminaCost);

        overheadAttackChargingTime = 0f;
        //overheadAttackChargingPlusDamaged = 0;

        chargingType = ChargingType.None;
    }

    private void GameInput_onHeavyAttackReleaseAction(object sender, EventArgs e)
    {
        
        if (chargingType != ChargingType.BasicAttack)
            return;

        animator.SetTrigger(HEAVY_BASIC_ATTACK_TRIGGER);

        StartCoroutine(
            DelayBeforeAttack(true,chargingPlusDamaged,defaultSpearSpeed, new Vector2(1, 0))
        );

        charging = false;
        chargingTime = 0f;
        //chargingPlusDamaged = 0;

        chargingType = ChargingType.None;
    }

    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Player.Instance != null)
        {
            animator.SetBool(PLAYER_IS_FALLING, Player.Instance.state == Player.PlayerState.Falling);
            animator.SetBool(PLAYER_IS_RUNNING, Player.Instance.state == Player.PlayerState.Running);

            TryBasicAttackCharging();
            animator.SetBool(HEAVY_ATTACK_CHARGING, charging);


            TryOverheadAttackCharging();
            animator.SetBool(OVERHEAD_ATTACK_CHARGING, overheadCharging);
        }
            

    }

    protected override void PlayerMovement_onJumpAction(object sender, EventArgs e)
    {
        animator.SetTrigger(PLAYER_JUMPTRIGGER);
    }
    public void SpearThrow(bool canPiercing,int bonusDamage,float speed,Vector2 direction)
    {
        Transform trans = Instantiate(spearClone, spearCloneSpawner.position, spearCloneSpawner.rotation);
        SpearClone sp = trans.GetComponent<SpearClone>();
        sp.canPiercingAttack = canPiercing;
        if (canPiercing) sp.SetPiercingShield(true);
        sp.AddDamage(bonusDamage);
        sp.speed = speed;
        sp.SetDirection(direction);
    }

    //protected override void PlayerMovement_onOverheadAttackAction(object sender, EventArgs e)
    //{
    //    animator.SetTrigger(OVERHEAD_ATTACK_TRIG);
    //    AimSkill.Instance.StartAim();
    //}

    protected override void PlayerMovement_onBasicAttackAction(object sender, EventArgs e)
    {
        animator.SetTrigger(BASIC_ATTACK_TRIG);
        StartCoroutine(DelayBeforeAttack(false,0,defaultSpearSpeed,new Vector2 (1,0)));
    }

    public IEnumerator DelayBeforeAttack(bool canPiercing,int bonusDamage,float speed, Vector2 direction)
    {
        yield return new WaitForSeconds(delayTime);
        SpearThrow(canPiercing,bonusDamage,speed,direction);
        Debug.Log("waiting");
    }


}
