using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Player;

public class PlayerMovement : MonoBehaviour
{
    public bool CanRun => canRun;
    public static PlayerMovement Instance { get; private set; }

    [Header("Components & Prefabs")]
    public Rigidbody2D rb;
    public Weapon weapon1;
    public Weapon weapon2;
    public Spell spell1;
    public Spell spell2;
    private float gravityInGame = 1.5f;
    public Weapon currentWeapon;
    public Spell currentSpell;
    private bool canRun;

    [SerializeField] private Transform shoulderJoint;
    [SerializeField] private Transform leftHandPoint;

    [Header("Movement Settings")]
    [SerializeField] private bool canDoubleJump = true;
    public float moveSpeed;
    private float playMoveSpeed;
    public float BaseSpeed => BossEnemy.Instance.moveSpeed;
    [SerializeField] private float distanceSpeedAdjustment = 0.5f;
    [SerializeField] private float minMoveSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float canSpinningAttackHeight = -1f;

    [Header("GameObject Colliders")]
    [SerializeField] private BoxCollider2D torsoCollider2D;
    [SerializeField] private CircleCollider2D legCollider2D;
    [SerializeField] public Transform spellCastSpot;
    public Transform skySpawnSpot;

    [Header("Action Cost")]
    [SerializeField] private int jumpCostStamina = 1;

    private int jumpCount;

    public event EventHandler onOverheadAttackAction;
    public event EventHandler onBasicAttackAction;
    public event EventHandler onCastingSpellAction;
    public event EventHandler onJumpAction;

    [HideInInspector] public Weapon spawnedWeapon;
    [HideInInspector] public Spell spawnedSpell;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = null;
        }
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        
        currentWeapon = weapon1;
        currentSpell = spell1;
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        if (GameInput.Instance != null)
        {
            GameInput.Instance.onJumpAction -= GameInput_onJumpAction;
            GameInput.Instance.onBasicAttackAction -= GameInput_onBasicAction;
            GameInput.Instance.onOverheadAttackAction -= GameInput_onOverheadAttackAction;
            GameInput.Instance.onCastingSpellAction -= GameInput_onCastingSpellAction;
            GameInput.Instance.onSwapWeaponAction -= GameInput_onSwapWeaponAction;
            GameInput.Instance.onSwapSpellAction -= GameInput_onSwapSpellAction;
        }
    }
    void Start()
    {

        if (GameInput.Instance != null)
        {
            GameInput.Instance.onJumpAction += GameInput_onJumpAction;
            GameInput.Instance.onBasicAttackAction += GameInput_onBasicAction;
            GameInput.Instance.onOverheadAttackAction += GameInput_onOverheadAttackAction;
            GameInput.Instance.onCastingSpellAction += GameInput_onCastingSpellAction;
            GameInput.Instance.onSwapWeaponAction += GameInput_onSwapWeaponAction;
            GameInput.Instance.onSwapSpellAction += GameInput_onSwapSpellAction;
        }

        SpawnWeapon();
        SpawnSpell();
    }

    private void FixedUpdate()
    {
        if (!canRun)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.gravityScale = 0;
            return;
        }

        //if (BossEnemy.Instance == null)
        //{
        //    rb.linearVelocity = new Vector2(
        //        playMoveSpeed,
        //        rb.linearVelocity.y
        //    );
        //    return;
        //}

        float bossX = BossEnemy.Instance.transform.position.x;
        float playerX = transform.position.x;
        rb.gravityScale = gravityInGame;
        float targetDistance = BossEnemy.Instance.PlayerBossDistance;
        float currentDistance = bossX - playerX;
        
        if (currentDistance <= 0f)
        {
            moveSpeed = minMoveSpeed;
        }
        else
        {
            
            moveSpeed = currentDistance * BaseSpeed  /  targetDistance ;

            if (moveSpeed <= BaseSpeed && moveSpeed >= BaseSpeed - distanceSpeedAdjustment) { 
                moveSpeed = BaseSpeed;
            }
        }

        rb.linearVelocity = new Vector2(
            moveSpeed,
            rb.linearVelocity.y
        );

        if (Player.Instance != null &&
            (Player.Instance.state == PlayerState.Jumping ||
             Player.Instance.state == PlayerState.DoubleJumping))
        {
            if (rb.linearVelocity.y <= 0f)
            {
                Player.Instance.SetPlayerState(PlayerState.Falling);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f && Player.Instance.state == PlayerState.Falling)
        {
            jumpCount = 0;
            Player.Instance.SetPlayerState(PlayerState.Running);
        }
    }

    public void SpawnWeapon()
    {
        ClearWeapon();
        if (currentWeapon == null) return;

        spawnedWeapon = Instantiate(currentWeapon, shoulderJoint);
        spawnedWeapon.transform.localPosition = Vector3.zero;
        spawnedWeapon.transform.localRotation = Quaternion.identity;
    }
    public bool EquipWeapon(Weapon weapon)
    {
        if (weapon == null)
            return false;

        if (weapon1 != null && weapon1.weaponSO == weapon.weaponSO)
            return false;

        if (weapon2 != null && weapon2.weaponSO == weapon.weaponSO)
            return false;

        if (weapon1 == null)
        {
            weapon1 = weapon;
        }
        else if (weapon2 == null)
        {
            weapon2 = weapon;
        }
        else
        {
            return false;
        }
        RefreshCurrentItem();


        if (ButtonManager.Instance != null)
        {
            ButtonManager.Instance.SetCurrentWeapon(currentWeapon);
        }

        return true;
    }
    public bool EquipSpell(Spell spell)
    {
        if (spell == null)
            return false;

        if (spell1 != null && spell1.spellSO == spell.spellSO)
            return false;

        if (spell2 != null && spell2.spellSO == spell.spellSO)
            return false;

        if (spell1 == null)
        {
            spell1 = spell;
        }
        else if (spell2 == null)
        {
            spell2 = spell;
        }
        else
        {
            return false;
        }

        RefreshCurrentItem();


        if (ButtonManager.Instance != null)
        {
            ButtonManager.Instance.SetCurrentSpell(currentSpell);
        }

        return true;
    }

    public bool UnequipSpell(string itemID)
    {
        if (spell1 == null && spell2 == null)
            return false;

        if (spell1 != null && spell1.spellSO.itemID == itemID)
        {
            if (spell2 != null)
            {
                spell1 = spell2;
                spell2 = null;
            }
            else
            {
                return false;
            }
        }
        else if (spell2 != null && spell2.spellSO.itemID == itemID)
        {
            spell2 = null;
        }
        else
        {
            return false;
        }

        RefreshCurrentItem();

        if (ButtonManager.Instance != null)
        {
            ButtonManager.Instance.SetCurrentSpell(currentSpell);
        }

        return true;
    }
    public bool UnequipWeapon(string itemID)
    {
        if (weapon1 == null && weapon2 == null)
            return false;

        if (weapon1 != null && weapon1.weaponSO.itemID == itemID)
        {
            if (weapon2 != null)
            {
                weapon1 = weapon2;
                weapon2 = null;
            }
            else
            {
                return false;
            }
        }
        else if (weapon2 != null && weapon2.weaponSO.itemID == itemID)
        {
            weapon2 = null;
        }
        else
        {
            return false;
        }

        RefreshCurrentItem();

        if (ButtonManager.Instance != null)
        {
            ButtonManager.Instance.SetCurrentWeapon(currentWeapon);
        }

        return true;
    }
    public void ClearWeapon()
    {
        if (spawnedWeapon != null)
        {
            Destroy(spawnedWeapon.gameObject);
            spawnedWeapon = null;
        }
    }

    public void SpawnSpell()
    {
        ClearSpell();
        if (currentSpell == null) return;

        spawnedSpell = Instantiate(currentSpell, leftHandPoint);
        spawnedSpell.transform.localPosition = Vector3.zero;
        spawnedSpell.transform.localRotation = Quaternion.identity;
    }

    public void ClearSpell()
    {
        if (spawnedSpell != null)
        {
            Destroy(spawnedSpell.gameObject);
            spawnedSpell = null;
        }
    }



    private void GameInput_onSwapWeaponAction(object sender, EventArgs e)
    {
        if (!canRun)
            return;
        if (weapon2 == null) return;
        Weapon temp = weapon1;
        weapon1 = weapon2;
        weapon2 = temp;

        RefreshCurrentItem();

        SpawnWeapon();

        if (ButtonManager.Instance != null)
        {
            ButtonManager.Instance.SetCurrentWeapon(currentWeapon);
        }
    }
    public void RefreshCurrentItem()
    {
        currentSpell = spell1;
        currentWeapon = weapon1;
        SpawnWeapon();
        SpawnSpell();
    }
    private void GameInput_onSwapSpellAction(object sender, EventArgs e)
    {
        if (!canRun)
            return;
        if (spell2 == null) return;
        Spell temp = spell1;
        spell1 = spell2;
        spell2 = temp;

        RefreshCurrentItem();

        SpawnSpell();

        if (ButtonManager.Instance != null)
        {
            ButtonManager.Instance.SetCurrentSpell(currentSpell);
        }
    }

    private void GameInput_onCastingSpellAction(object sender, EventArgs e)
    {
        if (!canRun)
            return;

        if (spawnedSpell == null || Player.Instance == null)
            return;

        if (Player.Instance.GetMana() < spawnedSpell.castSpellManaCost)
            return;

        Player.Instance.DecreaseMana(spawnedSpell.castSpellManaCost);
        spawnedSpell.CastSpell(this);
    }

    private void GameInput_onOverheadAttackAction(object sender, EventArgs e)
    {
        if (!canRun)
            return;
        if (Player.Instance != null && Player.Instance.GetStamina() >= spawnedWeapon.overheadAttackStaminaCost)
        {
            if (Player.Instance.state == PlayerState.Falling || Player.Instance.state == PlayerState.Jumping || Player.Instance.state == PlayerState.DoubleJumping)
            {
                Player.Instance.DecreaseStamina(spawnedWeapon.overheadAttackStaminaCost);
                onOverheadAttackAction?.Invoke(this, EventArgs.Empty);
            }
        }
    }


    private void GameInput_onBasicAction(object sender, EventArgs e)
    {
        if (!canRun)
            return;
        if (Player.Instance != null )
        {
            onBasicAttackAction?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_onJumpAction(object sender, EventArgs e)
    {
        if (!canRun)
            return;
        if (Player.Instance != null && Player.Instance.GetStamina() >= jumpCostStamina)
        {
            int maxJumps = canDoubleJump ? 2 : 1;

            if (jumpCount >= maxJumps)
                return;

            Jump();
            onJumpAction?.Invoke(this, EventArgs.Empty);
            Player.Instance.DecreaseStamina(jumpCostStamina);
            jumpCount++;

            if (jumpCount == 1)
                Player.Instance.SetPlayerState(PlayerState.Jumping);
            else
                Player.Instance.SetPlayerState(PlayerState.DoubleJumping);
        }
    }
    public void SetInstance(PlayerMovement player)
    {
        Instance = player;
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void SetCanRun(bool canRun)
    {
        this.canRun = canRun;
    }
    public void SetCurrentSpell(Spell spell)
    {
        currentSpell = spell;
    }

    
}