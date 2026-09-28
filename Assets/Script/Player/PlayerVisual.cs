using System;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    public Animator animator;
    public bool CanRun => PlayerMovement.Instance != null && PlayerMovement.Instance.CanRun;
    public static PlayerVisual Instance {  get; private set; }
    private const string IS_FALLING = "isFalling";
    private const string JUMP_TRIG = "JumpTrigger";
    private const string IS_RUNNING = "isRunning";
    public const string IS_IDLE = "isIdle";
    public const string IS_DEAD = "isDead";
    void Awake()
    {
        if (Instance != this)
        {
            Instance = null;
        }
        Instance = this;
        animator = GetComponent<Animator>();
        Instance = this;
    }


    private void Start()
    {
        

        
    }
    public void SetInstance(PlayerVisual player)
    {
        Instance = player;
    }
    public void SetIdle()
    {
        PlayerMovement.Instance.SetCanRun(false);
        PlayerMovement.Instance.rb.gravityScale = 0f;
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

    }

    private void Update()
    {
        if (player.state == Player.PlayerState.Died)
        {
            animator.SetBool(IS_DEAD, true);
            animator.SetBool(IS_RUNNING, false);
            animator.SetBool(IS_FALLING, false);
            animator.SetBool(IS_IDLE, false);
            return;
        }

        if (!CanRun)
        {
            animator.SetBool(IS_RUNNING, false);
            animator.SetBool(IS_FALLING, false);
            animator.SetBool(IS_IDLE, true);
            return;
        }

        if (player.state == Player.PlayerState.Jumping &&
            player.lastState != Player.PlayerState.Jumping)
        {
            animator.SetTrigger(JUMP_TRIG);
        }

        if (player.state == Player.PlayerState.DoubleJumping &&
            player.lastState != Player.PlayerState.DoubleJumping)
        {
            animator.SetTrigger(JUMP_TRIG);
        }

        animator.SetBool(IS_IDLE, false);
        animator.SetBool(IS_FALLING, player.state == Player.PlayerState.Falling);
        animator.SetBool(IS_RUNNING, player.state == Player.PlayerState.Running);

        player.lastState = player.state;
    }

}
