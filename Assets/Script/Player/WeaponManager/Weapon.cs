using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static Weapon;

public class Weapon : Item,IHasSkill
{
    public enum CanWeaponParry
    {
        True,
        False
    }
    public enum ChargingType
    {
        None,
        BasicAttack,
        OverheadAttack
    }

    public List<SkillSO> skillListSO;

    public List<SkillSO> SkillListSO => skillListSO;
    public enum HasHeavyAttack { True, False }
    public float holdThreshold = 0.25f;
    public float overheadAttackHoldThreshold = 0f;
    [Header("Parry Settings")]
    [SerializeField] private float parryWindowDuration = 0.2f;
    [SerializeField] private float parryCooldown = 0.5f;
    public Enemy parryTarget;

    public ChargingType chargingType = ChargingType.None;
    public const string BASIC_ATTACK_TRIG = "BasicAttackTrigger";
    public const string HEAVY_ATTACK_CHARGING = "HeavyAttackCharging";
    public const string OVERHEAD_ATTACK_TRIG = "OverheadAttackTrigger";
    public const string OVERHEAD_ATTACK_CHARGING = "OverheadAttackCharging";

    public const string PLAYER_JUMPTRIGGER = "PlayerJumpTrigger";
    public const string PLAYER_IS_FALLING = "PlayerIsFalling";
    public const string PLAYER_IS_RUNNING = "PlayerIsRunning";
    public const string HEAVY_BASIC_ATTACK_TRIGGER = "HeavyBasicAttackTrigger";

    public CanWeaponParry canWeaponParry;
    public HasHeavyAttack hasHeavyAttack;

    public float chargingTime;
    public float overheadAttackChargingTime;
    
    [SerializeField] public float chargingStaminaCost = 6;
    [SerializeField] public float overheadAttackStaminaCost = 6;
    
    [SerializeField] public float maxChargingTime = 2f;
    [SerializeField] public float overheadAttackMaxChargingTime = 2f;
    [SerializeField] public int maxChargeDamage = 10;
    [SerializeField] public int overheadAttackMaxChargingDamage = 10;
    //[SerializeField] private float 
    public WeaponSO weaponSO;
    [SerializeField] private bool isEquipped;

    public Skill swapWeaponButtonPrefab;
    public bool isParrying = false;
    public bool canParry = true;
    public int chargingPlusDamaged;
    public int overheadAttackChargingPlusDamaged;
    public bool charging;
    public bool overheadCharging;
    public Coroutine parryCoroutine;
    private float currentChargingStaminaCost;

    protected virtual void Start()
    {
        PlayerMovement.Instance.onBasicAttackAction += PlayerMovement_onBasicAttackAction;
        PlayerMovement.Instance.onOverheadAttackAction += PlayerMovement_onOverheadAttackAction;
        PlayerMovement.Instance.onJumpAction += PlayerMovement_onJumpAction;
    }

    protected virtual void OnDestroy()
    {
        if (PlayerMovement.Instance == null)
            return;
        PlayerMovement.Instance.onBasicAttackAction -= PlayerMovement_onBasicAttackAction;
        PlayerMovement.Instance.onOverheadAttackAction -= PlayerMovement_onOverheadAttackAction;
        PlayerMovement.Instance.onJumpAction -= PlayerMovement_onJumpAction;
    }

    protected virtual void PlayerMovement_onJumpAction(object sender, EventArgs e)
    {
    }

    protected virtual void PlayerMovement_onOverheadAttackAction(object sender, EventArgs e)
    {
    }

    protected virtual void PlayerMovement_onBasicAttackAction(object sender, EventArgs e)
    {
    }


    public void HidePlayerVisual()
    {
        if (PlayerVisual.Instance != null)
        {
            PlayerVisual.Instance.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void ShowPlayerVisual()
    {
        if (PlayerVisual.Instance != null)
        {
            PlayerVisual.Instance.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    private void Awake()
    {
    }
    void Update()
    {
        
    }
    public void TryBasicAttackCharging()
    {
        if (chargingType == ChargingType.OverheadAttack)
        {
            charging = false;
            return;
        }

        if (GameInput.Instance.IsHeavyAttackCharging )
        {
            if (chargingType == ChargingType.None)
            {
                chargingType = ChargingType.BasicAttack;
            }

            charging = true;

            chargingTime += Time.deltaTime;
            chargingTime = Mathf.Min(chargingTime, maxChargingTime);

            ConsumeStaminaSlowly(chargingStaminaCost);
            if (Player.Instance.GetStamina() <= 0 )
            {
                GameInput.Instance.BasicAttackCanceled();
            }
            chargingPlusDamaged = Mathf.FloorToInt(chargingTime / maxChargingTime * maxChargeDamage);
        }
        else
        {
            if (chargingType == ChargingType.BasicAttack)
            {
                charging = false;

                chargingTime = 0f;
                //chargingPlusDamaged = 0;
                currentChargingStaminaCost = 0f;

                chargingType = ChargingType.None;
            }
        }
    }
    public void TryOverheadAttackCharging()
    {
        if (chargingType == ChargingType.BasicAttack)
        {
            overheadCharging = false;
            return;
        }
        if ( Player.Instance.state != Player.PlayerState.Running)
        {
            if (GameInput.Instance.IsOverheadAttackCharging && Player.Instance.GetStamina() >= overheadAttackStaminaCost )
            {
                if (chargingType == ChargingType.None)
                {
                    chargingType = ChargingType.OverheadAttack;
                }

                if (!overheadCharging)
                {
                    overheadCharging = true;

                    if (AimSkill.Instance != null)
                    {
                        AimSkill.Instance.StartAim();
                        Debug.Log("StartAim");
                    }
                }

                overheadAttackChargingTime += Time.deltaTime;
                overheadAttackChargingTime = Mathf.Min(
                    overheadAttackChargingTime,
                    overheadAttackMaxChargingTime
                );

                overheadAttackChargingPlusDamaged = Mathf.FloorToInt(
                    overheadAttackChargingTime / overheadAttackMaxChargingTime
                    * overheadAttackMaxChargingDamage
                );
            }
            else
            {
                if (chargingType == ChargingType.OverheadAttack)
                {
                    overheadCharging = false;
                    overheadAttackChargingTime = 0f;
                    //overheadAttackChargingPlusDamaged = 0;
                    chargingType = ChargingType.None;
                }
            }
        }
            
        
    }

    public void ConsumeStaminaSlowly(float staminaCost)
    {
        float targetStaminaCost = chargingTime / maxChargingTime * staminaCost;
        float staminaCostThisFrame = targetStaminaCost - currentChargingStaminaCost;

        if (staminaCostThisFrame > 0f)
        {
            Player.Instance.DecreaseStamina(staminaCostThisFrame);
        }

        currentChargingStaminaCost = targetStaminaCost;
    }

    public void ConsumeStaminaFast(float staminaCost)
    {
        Player.Instance.DecreaseStamina(staminaCost);
    }

    public void TryParry()
    {
        if (canWeaponParry == CanWeaponParry.False || !canParry)
            return;

        if (parryCoroutine != null)
            StopCoroutine(parryCoroutine);

        parryCoroutine = StartCoroutine(TriggerParryWindow());
        //Debug.Log("Parrying");
    }

    public void DoDamage(Enemy enemy,int damage)
    {
        enemy.TakeDamage(damage);
    }

    public Enemy CheckParryCollider(List<Collider2D> colliders)
    {
        HashSet<Enemy> enemiesHitByBody = new HashSet<Enemy>();
        List<EnemyWeapon> enemyWeapons = new List<EnemyWeapon>();

        foreach (Collider2D col in colliders)
        {
            if (col.TryGetComponent(out HurtBox hurtBox))
            {
                Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

                if (enemy != null)
                {
                    enemiesHitByBody.Add(enemy);
                }
            }

            if (col.TryGetComponent(out EnemyWeapon enemyWeapon))
            {
                if (enemyWeapon.enemy != null &&
                    enemyWeapon.canParry == EnemyWeapon.CanParry.True)
                {
                    enemyWeapons.Add(enemyWeapon);
                }
            }
        }

        foreach (EnemyWeapon enemyWeapon in enemyWeapons)
        {
            Enemy enemy = enemyWeapon.enemy;
            if (!enemiesHitByBody.Contains(enemy))
            {
                return enemy;
            }
        }

        return null;
    }

    public void SuccessfulParry()
    {
        if (parryCoroutine != null)
        {
            StopCoroutine(parryCoroutine);
        }

        isParrying = false;
        canParry = true;

    }



    public IEnumerator TriggerParryWindow()
    {
        canParry = false;
        isParrying = true;

        yield return new WaitForSeconds(parryWindowDuration);

        isParrying = false;


        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }
    public void RotateToDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
