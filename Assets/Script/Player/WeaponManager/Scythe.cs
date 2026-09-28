using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
{

    public Animator animator;
    private ContactFilter2D filter;
    private List<Collider2D> hitsCollider = new List<Collider2D>();
    private string PLAYER_IS_HOLDING_SCYTHE = "IsHoldingScythe"; 
    public bool playerIsHolding;

    [SerializeField] private float delayTime = 0.2f;
    [SerializeField] private Transform scytheTornado;
    [SerializeField] private Transform scytheTornadoSpawner;
    [SerializeField] public CircleCollider2D attackHitBox;
    [SerializeField] private int slashDamage = 2;
    [SerializeField] private int spinningAttackDamage = 4;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        filter = ContactFilter2D.noFilter;
        filter.useTriggers = true;
    }
    protected override void Start()
    {
        base.Start();
        playerIsHolding = true;

        GameInput.Instance.onHeavyAttackReleaseAction += GameInput_onHeavyAttackReleaseAction;
    }

    private void GameInput_onOverheadAttackAction(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameInput.Instance.onHeavyAttackReleaseAction -= GameInput_onHeavyAttackReleaseAction;
    }
    private void GameInput_onHeavyAttackReleaseAction(object sender, EventArgs e)
    {

        if (playerIsHolding){
            animator.SetTrigger(HEAVY_BASIC_ATTACK_TRIGGER);

            StartCoroutine(DelayBeforeBasicAttack());

            playerIsHolding = false;

            charging = false;
            chargingTime = 0f;
            //chargingPlusDamaged = 0;
        }
    }

    void Update()
    {
        if (Player.Instance != null)
        {
            animator.SetBool(PLAYER_IS_FALLING, Player.Instance.state == Player.PlayerState.Falling);
            animator.SetBool(PLAYER_IS_RUNNING, Player.Instance.state == Player.PlayerState.Running);

            TryBasicAttackCharging();
            if (!playerIsHolding)
            {
                charging = false;
            }
            animator.SetBool(HEAVY_ATTACK_CHARGING, charging);

            animator.SetBool(PLAYER_IS_HOLDING_SCYTHE, playerIsHolding);
        }
    }
    protected override void PlayerMovement_onJumpAction(object sender, EventArgs e)
    {
        animator.SetTrigger(PLAYER_JUMPTRIGGER);
    }

    
    protected override void PlayerMovement_onOverheadAttackAction(object sender, EventArgs e)
    {
        if (playerIsHolding)
        {
            animator.SetTrigger(OVERHEAD_ATTACK_TRIG);
        }
    }

    protected override void PlayerMovement_onBasicAttackAction(object sender, EventArgs e)
    {
        if (playerIsHolding)
        {
             animator.SetTrigger(BASIC_ATTACK_TRIG);
        }
    }
    
    public void ScytheTornado()
    {
        Transform trans = Instantiate(scytheTornado, scytheTornadoSpawner.position,scytheTornadoSpawner.rotation);
        ScytheTornado scytheTor = trans.GetComponent<ScytheTornado>();
        scytheTor.AddDamage(chargingPlusDamaged);
        scytheTor.SetScythe(this);
        Debug.Log(scytheTor.GetDamage() + " " + chargingPlusDamaged);
    }
    public void ExecuteSlashDamage()
    {
        hitsCollider.Clear();
        int count = attackHitBox.Overlap(filter, hitsCollider);
        //Debug.Log(count);
        foreach (Collider2D col in hitsCollider)
        {
            if (col.TryGetComponent<HurtBox>(out HurtBox hurtBox))
            {
                Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(slashDamage);

                }
            }
        }
    }


    public void ExecuteSpinAttackDamage()
    {
        hitsCollider.Clear();
        int count = attackHitBox.Overlap(filter, hitsCollider);

        foreach (Collider2D col in hitsCollider)
        {
            if (col.TryGetComponent<HurtBox>(out HurtBox hurtBox))
            {
                Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(spinningAttackDamage);

                }
            }
        }
    }

    public IEnumerator DelayBeforeBasicAttack()
    {
        yield return new WaitForSeconds(delayTime);
        ScytheTornado();
        Debug.Log("waiting");
    }
}
