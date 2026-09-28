using System;
using UnityEngine;

public class LeftHandManager : MonoBehaviour
{
    public static LeftHandManager Instance { get; private set; }

    public Animator animator;

    private const string PLAYER_JUMPTRIGGER = "PlayerJump";
    private const string PLAYER_IS_FALLING = "PlayerIsFalling";
    private const string PLAYER_IS_RUNNING = "PlayerIsRunning";
    private const string CAST_SPELL = "SpellTrigger";

    private bool wasRunning;

    private void Awake()
    {


        Instance = this;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.onJumpAction += PlayerMovement_onJumpAction;
        }

        if (GameInput.Instance != null)
        {
            GameInput.Instance.onCastingSpellAction += GameInput_onCastingSpellAction;
        }
    }

    private void GameInput_onCastingSpellAction(object sender, EventArgs e)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(CAST_SPELL);

        if (PlayerMovement.Instance != null &&
            PlayerMovement.Instance.spawnedSpell != null)
        {
            PlayerMovement.Instance.spawnedSpell.HideSpellVisual();
        }
    }

    private void PlayerMovement_onJumpAction(object sender, EventArgs e)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(PLAYER_JUMPTRIGGER);
    }

    private void Update()
    {
        if (Player.Instance == null || PlayerVisual.Instance == null || animator == null)
        {
            return;
        }

        animator.SetBool(
            PLAYER_IS_FALLING,
            Player.Instance.state == Player.PlayerState.Falling
        );

        bool isRunning = Player.Instance.state == Player.PlayerState.Running;

        animator.SetBool(PLAYER_IS_RUNNING, isRunning);

        if (isRunning != wasRunning)
        {
            if (PlayerMovement.Instance != null &&
                PlayerMovement.Instance.spawnedSpell != null)
            {
                if (isRunning)
                {
                    PlayerMovement.Instance.spawnedSpell.ShowSpellVisual();
                }
                else
                {
                    PlayerMovement.Instance.spawnedSpell.HideSpellVisual();
                }
            }

            wasRunning = isRunning;
        }
    }

    private void OnDestroy()
    {
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.onJumpAction -= PlayerMovement_onJumpAction;
        }

        if (GameInput.Instance != null)
        {
            GameInput.Instance.onCastingSpellAction -= GameInput_onCastingSpellAction;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}