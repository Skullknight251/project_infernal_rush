using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    private PlayerInputAction inputActions;

    public event EventHandler onJumpAction;
    public event EventHandler onBasicAttackAction;
    public event EventHandler onHeavyAttackReleaseAction;
    public event EventHandler onOverheadAttackReleaseAction;
    public event EventHandler onCastingSpellAction;
    public event EventHandler onOverheadAttackAction;
    public event EventHandler onBlockAction;
    public event EventHandler onSwapWeaponAction;
    public event EventHandler onSwapSpellAction;
    public event EventHandler onEscape;

    public bool IsHeavyAttackCharging { get; private set; }
    public bool IsSpellCasting { get; private set; }
    public bool IsOverheadAttackCharging { get; private set; }

    private float overheadAttackPressTime;
    private float basicAttackPressTime;
    private float castSpellPressTime;

    private bool isHoldingSpellCasting;
    private bool isHoldingBasicAttack;
    private bool isHoldingOverHeadAttack;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        inputActions = new PlayerInputAction();
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Escape.performed += Escape_performed;

        inputActions.Player.BasicAttack.started += BasicAttack_started;
        inputActions.Player.BasicAttack.canceled += BasicAttack_canceled;

        inputActions.Player.OverHeadAttack.started += OverHeadAttack_started;
        inputActions.Player.OverHeadAttack.canceled += OverHeadAttack_canceled;

        inputActions.Player.CastSpell.started += CastSpell_started;
        inputActions.Player.CastSpell.canceled += CastSpell_canceled;

        inputActions.Player.Block.performed += Block_performed;
        inputActions.Player.SwapWeapon.performed += SwapWeapon_performed;
        inputActions.Player.SwapSpell.performed += SwapSpell_performed;
    }

    void Update()
    {
        Weapon currentWeapon = PlayerMovement.Instance != null ? PlayerMovement.Instance.currentWeapon : null;
        Spell currentSpell = PlayerMovement.Instance != null ? PlayerMovement.Instance.currentSpell : null;

        if (isHoldingBasicAttack && currentWeapon != null)
        {
            if (!IsHeavyAttackCharging && Time.time - basicAttackPressTime >= currentWeapon.holdThreshold)
            {
                IsHeavyAttackCharging = true;
            }
        }

        if (isHoldingOverHeadAttack && currentWeapon != null)
        {
            if (!IsOverheadAttackCharging && Time.time - overheadAttackPressTime >= currentWeapon.overheadAttackHoldThreshold)
            {
                IsOverheadAttackCharging = true;
            }
        }

        if (isHoldingSpellCasting && currentSpell != null)
        {
            if (!IsSpellCasting && Time.time - castSpellPressTime >= currentSpell.spellCastingHoldThreshold)
            {
                IsSpellCasting = true;
            }
        }
    }

    public void BasicAttackStarted()
    {
        Skill basicSkill = ButtonManager.Instance?.GetBasicSkill();
        if (basicSkill != null && basicSkill.GetControlType() == Skill.ControlType.CanCharging)
        {
            basicAttackPressTime = Time.time;
            isHoldingBasicAttack = true;
        }
        else
        {
            onBasicAttackAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void OverheadAttackStarted()
    {
        Skill plungeSkill = ButtonManager.Instance?.GetPlungeSkill();
        if (plungeSkill != null && plungeSkill.GetControlType() == Skill.ControlType.CanCharging)
        {
            overheadAttackPressTime = Time.time;
            isHoldingOverHeadAttack = true;
        }
        else
        {
            onOverheadAttackAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void CastSpellStarted()
    {
        Skill spellSkill = ButtonManager.Instance?.GetSpellCastSkill();
        if (spellSkill != null && spellSkill.GetControlType() == Skill.ControlType.CanCharging)
        {
            castSpellPressTime = Time.time;
            isHoldingSpellCasting = true;
        }
        else
        {
            onCastingSpellAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void CastSpellCanceled()
    {
        if (!isHoldingSpellCasting) return;
        float holdTime = Time.time - castSpellPressTime;
        isHoldingSpellCasting = false;

        Spell currentSpell = PlayerMovement.Instance != null ? PlayerMovement.Instance.currentSpell : null;
        float threshold = currentSpell != null ? currentSpell.spellCastingHoldThreshold : 0.5f;

        if (IsSpellCasting)
        {
            IsSpellCasting = false;
            TriggerReleaseCastingSpell();
        }
        else if (holdTime < threshold)
        {
            onCastingSpellAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void BasicAttackCanceled()
    {
        if (!isHoldingBasicAttack) return;
        float holdTime = Time.time - basicAttackPressTime;
        isHoldingBasicAttack = false;

        Weapon currentWeapon = PlayerMovement.Instance != null ? PlayerMovement.Instance.currentWeapon : null;
        float threshold = currentWeapon != null ? currentWeapon.holdThreshold : 0.5f;

        if (IsHeavyAttackCharging)
        {
            IsHeavyAttackCharging = false;
            TriggerReleaseBasicAttack();
        }
        else if (holdTime < threshold)
        {
            onBasicAttackAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void OverheadAttackCanceled()
    {
        if (!isHoldingOverHeadAttack) return;
        float holdTime = Time.time - overheadAttackPressTime;
        isHoldingOverHeadAttack = false;

        Weapon currentWeapon = PlayerMovement.Instance != null ? PlayerMovement.Instance.currentWeapon : null;
        float threshold = currentWeapon != null ? currentWeapon.overheadAttackHoldThreshold : 0.5f;

        if (IsOverheadAttackCharging)
        {
            IsOverheadAttackCharging = false;
            TriggerReleaseOverheadAttack();
        }
        else if (holdTime < threshold)
        {
            onOverheadAttackAction?.Invoke(this, EventArgs.Empty);
        }
    }

    private void BasicAttack_started(InputAction.CallbackContext context) => BasicAttackStarted();
    public void BasicAttack_canceled(InputAction.CallbackContext context) => BasicAttackCanceled();
    private void OverHeadAttack_started(InputAction.CallbackContext obj) => OverheadAttackStarted();
    private void OverHeadAttack_canceled(InputAction.CallbackContext obj) => OverheadAttackCanceled();
    private void CastSpell_started(InputAction.CallbackContext obj) => CastSpellStarted();
    private void CastSpell_canceled(InputAction.CallbackContext obj) => CastSpellCanceled();
    private void Escape_performed(InputAction.CallbackContext obj) => onEscape?.Invoke(this, EventArgs.Empty);
    private void SwapSpell_performed(InputAction.CallbackContext obj) => onSwapSpellAction?.Invoke(this, EventArgs.Empty);
    private void SwapWeapon_performed(InputAction.CallbackContext obj) => onSwapWeaponAction?.Invoke(this, EventArgs.Empty);
    private void Block_performed(InputAction.CallbackContext obj) => onBlockAction?.Invoke(this, EventArgs.Empty);
    public void Jump_performed(InputAction.CallbackContext obj) => onJumpAction?.Invoke(this, EventArgs.Empty);

    public void SwapWeapon() => onSwapWeaponAction?.Invoke(this, EventArgs.Empty);
    public void SwapSpell() => onSwapSpellAction?.Invoke(this, EventArgs.Empty);
    public void Jump() => onJumpAction?.Invoke(this, EventArgs.Empty);
    public void Block() => onBlockAction?.Invoke(this, EventArgs.Empty);
    public void TriggerReleaseBasicAttack() => onHeavyAttackReleaseAction?.Invoke(this, EventArgs.Empty);
    public void TriggerReleaseCastingSpell() => onCastingSpellAction?.Invoke(this, EventArgs.Empty);
    public void TriggerReleaseOverheadAttack() => onOverheadAttackReleaseAction?.Invoke(this, EventArgs.Empty);
    public void CastSpell() => onCastingSpellAction?.Invoke(this, EventArgs.Empty);

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
        }
    }
}