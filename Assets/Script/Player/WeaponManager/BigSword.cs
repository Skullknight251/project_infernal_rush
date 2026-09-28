using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSword : Weapon
{
    
    public Animator animator;
    private ContactFilter2D filter;
    private List<Collider2D> slashHitsCollider = new List<Collider2D>();
    private List<Collider2D> overheadAttackHitsCollider = new List<Collider2D>();
    [SerializeField] private float overheadAttackTime;    
    
    [SerializeField] public CapsuleCollider2D slashHitBox;
    [SerializeField] public BoxCollider2D overheadAttackHitBox; 
    [SerializeField] private int slashDamage = 2;
    [SerializeField] private int overheadAttackDamage = 4;
    [SerializeField] private GameObject playerVisual;
    [SerializeField] private float overheadAttackFallSpeed = 20f;
    private bool isOverheadAttacking;
    private bool overheadAttackHit;
    private HashSet<Enemy> overheadAttackHitEnemies = new HashSet<Enemy>();
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void PlayerMovement_onJumpAction(object sender, EventArgs e)
    {
        animator.SetTrigger(PLAYER_JUMPTRIGGER);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        filter = ContactFilter2D.noFilter;
        filter.useTriggers = true;
    }
    protected override void PlayerMovement_onOverheadAttackAction(object sender, EventArgs e)
    {
        if (isOverheadAttacking)
            return;

        if (Player.Instance.state != Player.PlayerState.Jumping &&
            Player.Instance.state != Player.PlayerState.DoubleJumping &&
            Player.Instance.state != Player.PlayerState.Falling)
            return;

        StartOverheadAttack();
    }
    private void StartOverheadAttack()
    {
        isOverheadAttacking = true;
        overheadAttackHit = false;
        HidePlayerVisual();

        if (AimSkill.Instance != null)
            AimSkill.Instance.StopAim();

        animator.SetTrigger(OVERHEAD_ATTACK_TRIG);
        
    }
    protected override void PlayerMovement_onBasicAttackAction(object sender, EventArgs e)
    {
        animator.SetTrigger(BASIC_ATTACK_TRIG);
        TryParry();

    }
    void Update()
    {
        if (Player.Instance != null)
        {
            animator.SetBool(PLAYER_IS_FALLING, Player.Instance.state == Player.PlayerState.Falling);
            animator.SetBool(PLAYER_IS_RUNNING, Player.Instance.state == Player.PlayerState.Running);
            if (isOverheadAttacking)
            {
                if (Player.Instance.state == Player.PlayerState.Running)
                {
                    FinishOverheadAttack();
                    return;
                }
                ExecuteOverheadAttackDamage();
                PlayerMovement.Instance.rb.linearVelocity = new Vector2(
                    PlayerMovement.Instance.rb.linearVelocity.x,
                    -overheadAttackFallSpeed
                );
            }
        }
    }

    public void ExecuteSlashDamage()
    {
        slashHitsCollider.Clear();
        int count = slashHitBox.Overlap(filter, slashHitsCollider);

        parryTarget = CheckParryCollider(slashHitsCollider);
        
        if (parryTarget != null && isParrying)
        {
            parryTarget.StartStun();
            SuccessfulParry();
        }
        foreach (Collider2D col in slashHitsCollider)
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

    public void FinishOverheadAttack()
    {
        if (!isOverheadAttacking)
            return;

        isOverheadAttacking = false;

        overheadAttackHitEnemies.Clear();
        overheadAttackHitsCollider.Clear();

        PlayerMovement.Instance.rb.linearVelocity = new Vector2(
            PlayerMovement.Instance.rb.linearVelocity.x,
            0f
        );

        ShowPlayerVisual();
    }
    public void ExecuteOverheadAttackDamage()
    {

        int count = overheadAttackHitBox.Overlap(
            filter,
            overheadAttackHitsCollider
        );

        foreach (Collider2D col in overheadAttackHitsCollider)
        {
            if (!col.TryGetComponent<HurtBox>(out HurtBox hurtBox))
                continue;

            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

            if (enemy == null)
                continue;

            if (overheadAttackHitEnemies.Contains(enemy))
                continue;

            overheadAttackHitEnemies.Add(enemy);
            enemy.TakeDamage(overheadAttackDamage);
        }
    }
    //public void ExecuteSpinAttackDamage()
    //{
    //    spinningAttackHitsCollider.Clear();
    //    int count = spinningAttackHitBox.Overlap(filter, spinningAttackHitsCollider);

    //    parryTarget = CheckParryCollider(spinningAttackHitsCollider);

    //    if (parryTarget != null && isParrying)
    //    {
    //        parryTarget.StartStun();
    //        SuccessfulParry();
    //        return;
    //    }
    //    foreach (Collider2D col in spinningAttackHitsCollider)
    //    {
    //        if (col.TryGetComponent<HurtBox>(out HurtBox hurtBox))
    //        {
    //            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

    //            if (enemy != null)
    //            {
    //                enemy.TakeDamage(spinningAttackDamage);
    //            }
    //        }
    //    }
    //}

}
