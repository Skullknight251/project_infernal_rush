using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private const string TOTAL_SOUL = "total_soul";
    public event Action OnStatsChanged;
    public float statsDrainSpeed = 1.5f;
    private Inventory inventory;
    [SerializeField] private float vanishDelayTime;
    public Transform tutorialSpawnPoint;

    public enum PlayerState
    {
        Running,
        Jumping,
        DoubleJumping,
        Falling,
        Died
    }
    [Header("Stats")]
    public float maxHealth;
    public float maxStamina;
    public float maxMana;
    public PlayerState state { get; private set; }
    private float health;
    private float stamina;
    private float mana;
    private int currentSoul;
    public PlayerState lastState;
    public float recoveryStatsCircle = 10;
    
    private void Awake()
    {
        if (Instance != this)
        {
            Instance = null;
        }
        
        Instance = this;
        health = maxHealth;
        stamina = maxStamina;
        mana = maxMana;
        currentSoul = 0;
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        
    }
    public void Update()
    {
        
    }
    public void SetInstance(Player player)
    {
        Instance = player;
    }
    public IEnumerator recoveryStatsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(recoveryStatsCircle);
            //Debug.Log($"RECOVERY at {Time.time}");
            if (stamina < maxStamina)
            {
                AddStamina(1);
            }

            if (mana < maxMana)
            {
                AddMana(1);
            }
        }
    }

    private void Start()
    {
        state = PlayerState.Running;
        lastState = state;
        StartCoroutine(recoveryStatsCoroutine());
        OnStatsChanged?.Invoke();
    }
    public void AddHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        OnStatsChanged?.Invoke();
    }
    public void DecreaseHealth(float amount)
    {
        health = Mathf.Clamp(health - amount, 0, maxHealth);
        OnStatsChanged?.Invoke();
    }
    public void AddStamina(float amount)
    {
        stamina = Mathf.Clamp(stamina + amount, 0,maxStamina);
        OnStatsChanged?.Invoke();
    }
    public void DecreaseStamina(float amount)
    {
        stamina = Mathf.Clamp(stamina - amount, 0, maxStamina);
        OnStatsChanged?.Invoke();
    }
    public void AddSoul(int amount)
    {
        currentSoul = Mathf.Max(currentSoul + amount, 0);
        OnStatsChanged?.Invoke();
    }
    public void AddMana(float amount)
    {
        mana = Mathf.Clamp(mana + amount, 0,maxMana);
        OnStatsChanged?.Invoke();
    }
    public void DecreaseMana(float amount)
    {
        mana = Mathf.Clamp(mana - amount, 0, maxMana);
        OnStatsChanged?.Invoke();
    }
    public void TakeDamage(float damage)
    {
        DecreaseHealth(damage);
        Debug.Log($"Player took {damage} damage. Current health: {health}");
        OnStatsChanged?.Invoke();
        if (health <= 0)
        {
            Die();
        }
    }

    public void StaminaDrain()
    {
        DecreaseStamina(statsDrainSpeed * Time.deltaTime);
    }
    public void ManaDrain()
    {
        DecreaseMana(statsDrainSpeed * Time.deltaTime);
    }
    private void Die()
    {
        if (PlayerVisual.Instance != null && PlayerVisual.Instance.animator != null)
        {
            PlayerVisual.Instance.animator.SetBool(PlayerVisual.IS_DEAD, true);
        }
        SetPlayerState(PlayerState.Died);
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.SetCanRun(false);
        }
        HideHands();
        StartCoroutine(DelayBeforeVanish());

    }
    private void HideHands()
    {
        Transform leftHand = transform.Find("Left_hand");
        Transform rightHand = transform.Find("Right_hand");
        foreach (SpriteRenderer renderer in leftHand.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }
        foreach (SpriteRenderer renderer in rightHand.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;

        }

    }
    public void ClearInstance()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    public IEnumerator DelayBeforeVanish()
    {
        yield return new WaitForSeconds(vanishDelayTime);
        Destroy(gameObject);
    }
    public void SetPlayerState(PlayerState newState)
    {
        state = newState;

    }
    public int GetSoulsEarned()
    {
        return currentSoul;
    }
    public float GetHealth()
    {
        return health;
    }
    
    public float GetStamina() { return stamina;}
    public float GetMana() { return mana;}
}