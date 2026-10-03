using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static IEnemyHasSkill;
using static IShooter;

public class Enemy : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private EnemySO enemySO;
    public float takeDamageDelay = 0.5f;
    public int damage;
    [SerializeField] public int health;
    [SerializeField] private float spawnTime = 1f;
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private float deathBounceForce = 8f;
    [SerializeField] private float deathHorizontalForce = 3f;
    [SerializeField] private float stunHorizontalForce = 5f;
    [SerializeField] private GameObject normalAttackTriggerPoint;
    [SerializeField] private GameObject skillSignal;
    public float cqcAttackRange = 2f;
    public LayerMask playerLayer;
    private const string DEAD_TRIGGER = "DeadTrigger";
    private const string HURT_TRIGGER = "HurtTrigger";
    private const string ATTACK1_TRIGGER = "Attack1Trigger";
    public const string IS_STUNNED = "IsStunned";
    public bool CanRun => PlayerMovement.Instance != null && PlayerMovement.Instance.CanRun;
    public GameObject hurtBoxCollider;
    private bool deadByPlayer;

    [Header("DropItem")]
    [SerializeField] private List<DropItemSO> dropItems;
    public Transform tutorialSpawnPoint;

    [Header("Enemy Separation")]
    [SerializeField] private float separationRadius = 10f;
    [SerializeField] private int maxEnemyPerColumn = 3;
    [SerializeField] private float columnDistance = 3f;
    [SerializeField] private float verticalDistance = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float nextTurnDistance = 3f;

    public EnemyComboSO combo;
    public bool isDead;
    public int index;
    public bool isStunned;
    public float stunDuration = 2f;
    public float delayDeathTime = 1.5f;
    public Animator animator;
    protected Rigidbody2D rb;
    public float attackDelay = 1f;
    public int currentHealth;
    public float MoveSpeed => BossEnemy.Instance.moveSpeed;
    public BossEnemy boss;

    private Coroutine skillCoroutineInstance;
    private Coroutine delayAfterSpawnInstance;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (hurtBoxCollider != null)
        {
            hurtBoxCollider.GetComponent<HurtBox>().SetEnemy(this);
        }
        if (normalAttackTriggerPoint != null)
        {
            normalAttackTriggerPoint.GetComponent<NormalAttackTrigger>().SetEnemy(this);
        }
    }

    protected virtual void OnEnable()
    {
        deadByPlayer = true;
        isDead = false;
        isStunned = false;
        currentHealth = health;

        if (hurtBoxCollider != null)
        {
            hurtBoxCollider.SetActive(true);
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        if (healthBar != null)
        {
            healthBar.UpdateVisual();
        }

        FlipToPlayer();

        if (gameObject.activeInHierarchy)
        {
            delayAfterSpawnInstance = StartCoroutine(delayAfterSpawn());
        }
    }

    protected virtual void OnDisable()
    {
        if (delayAfterSpawnInstance != null) StopCoroutine(delayAfterSpawnInstance);
        if (skillCoroutineInstance != null) StopCoroutine(skillCoroutineInstance);
        StopAllCoroutines();
    }

    protected virtual void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public IEnumerator delayAfterSpawn()
    {
        yield return new WaitForSeconds(spawnTime);
        if (combo != null && !isDead)
        {
            skillCoroutineInstance = StartCoroutine(SkillCoroutine());
        }
    }

    public void SetDeadByPlayer(bool res)
    {
        deadByPlayer = res;
    }

    public bool IsPlayerInCQCRange()
    {
        if (Player.Instance == null) return false;

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, cqcAttackRange, playerLayer);
        if (playerCollider == null) return false;

        Vector2 directionToPlayer = (Player.Instance.transform.position - transform.position).normalized;
        Vector2 forwardDirection = -transform.right;

        return Vector2.Dot(forwardDirection, directionToPlayer) > 0;
    }

    public IEnumerator SkillSignal(Transform position)
    {
        GameObject tmp = SimplePoolManager.Instance.Spawn(skillSignal, position.position, position.rotation);
        tmp.transform.SetParent(position);
        tmp.transform.localScale = skillSignal.transform.localScale;

        yield return new WaitForSeconds(0.6f);
        SimplePoolManager.Instance.Despawn(tmp);
    }

    public IEnumerator ExecuteSkill(EnemySkillType skillType)
    {
        if (!CanRun) yield break;
        switch (skillType)
        {
            case EnemySkillType.Skill1:
                yield return StartCoroutine(Skill1());
                break;
            case EnemySkillType.Skill2:
                yield return StartCoroutine(Skill2());
                break;
        }
    }

    public IEnumerator ExecuteShoot(EnemyShootType shootType)
    {
        if (!CanRun) yield break;
        switch (shootType)
        {
            case EnemyShootType.Shoot1:
                yield return StartCoroutine(Shoot1());
                break;
            case EnemyShootType.Shoot2:
                yield return StartCoroutine(Shoot2());
                break;
        }
    }

    public virtual IEnumerator Shoot1() { yield return null; }
    public virtual IEnumerator Shoot2() { yield return null; }
    public virtual IEnumerator Skill1() { yield return null; }
    public virtual IEnumerator Skill2() { yield return null; }

    public IEnumerator SkillCoroutine()
    {
        while (!isDead && Player.Instance != null)
        {
            foreach (EnemyActionSO enemyAction in combo.actionsInCombo)
            {
                if (isDead) yield break;
                yield return StartCoroutine(enemyAction.ExecuteAction(this));
            }
            yield return null;
        }
    }

    public virtual void OnHitPlayer()
    {
        if (animator != null) animator.SetTrigger(ATTACK1_TRIGGER);
    }

    public Vector2 GetSeparationDirection()
    {
        Vector2 position = transform.position;
        int columnIndex = index / maxEnemyPerColumn;
        int slotIndex = index % maxEnemyPerColumn;

        position.x -= columnIndex * nextTurnDistance;

        switch (slotIndex)
        {
            case 0: position.y += 0f; break;
            case 1: position.y += verticalDistance; break;
            case 2: position.y -= verticalDistance; break;
        }
        return position;
    }

    public void EnableHurtBoxWithDelay()
    {
        StopCoroutine(nameof(TakeDamageDelayRoutine));
        StartCoroutine(TakeDamageDelayRoutine());
    }

    public IEnumerator TakeDamageDelayRoutine()
    {
        yield return new WaitForSeconds(takeDamageDelay);
        if (hurtBoxCollider != null && !isDead)
        {
            hurtBoxCollider.SetActive(true);
        }
    }

    public void PositionCheck()
    {
        bool hasOverlap = true;
        int maxIterations = 10;
        int iteration = 0;

        while (hasOverlap && iteration < maxIterations)
        {
            hasOverlap = false;
            iteration++;
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, separationRadius, enemyLayer);

            foreach (Collider2D enemyCollider in enemies)
            {
                if (enemyCollider.gameObject == gameObject) continue;

                if (enemyCollider.TryGetComponent(out Enemy otherEnemy))
                {
                    if (otherEnemy.index == index)
                    {
                        transform.position -= new Vector3(nextTurnDistance, 0f, 0f);
                        hasOverlap = true;
                        break;
                    }
                }
            }
        }
    }

    public void CheckPlayerHit(Collider2D collision, int damage)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (isStunned)
        {
            currentHealth -= 100;
        }

        if (animator != null) animator.SetTrigger(HURT_TRIGGER);
        if (healthBar != null) healthBar.UpdateVisual();

        CheckHealth();
    }

    public void StartStun()
    {
        if (isDead || isStunned) return;

        isStunned = true;
        StartCoroutine(StunTime());
        StartCoroutine(StunTutorial());
    }

    public IEnumerator StunTime()
    {
        if (rb != null) rb.AddForce(Vector2.right * stunHorizontalForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(stunDuration);
        ResetStunState();
    }

    public void ResetStunState()
    {
        if (isDead) return;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isStunned = false;
    }

    public void CheckHealth()
    {
        if (currentHealth <= 0 && !isDead)
        {
            StartCoroutine(Death());
        }
    }

    public IEnumerator Death()
    {
        isDead = true;
        if (animator != null) animator.SetTrigger(DEAD_TRIGGER);

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = Vector2.zero;
            Vector2 deathForce = new Vector2(Random.Range(-deathHorizontalForce, deathHorizontalForce), deathBounceForce);
            rb.AddForce(deathForce, ForceMode2D.Impulse);
        }

        if (hurtBoxCollider != null) hurtBoxCollider.SetActive(false);

        if (deadByPlayer)
        {
            DropItem();
        }
        if (boss != null)
        {
            boss.NotifyEnemyDeath(this);
        }
        yield return new WaitForSeconds(delayDeathTime);
        SimplePoolManager.Instance.Despawn(gameObject);
    }
    public void DropItem()
    {
        foreach (DropItemSO dropItem in dropItems)
        {
            for (int i = 0; i < dropItem.quantity; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
                Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                Instantiate(dropItem.prefab, spawnPosition, Quaternion.identity);
            }
        }
        StartCoroutine(DropItemTutorial());
    }
    public IEnumerator StunTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.stun))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.stun, TutorialPoint.Instance.stunTutorialPoint);
        }
    }
    public IEnumerator DropItemTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.dropItem))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.dropItem, TutorialPoint.Instance.dropItemTutorialPoint);
        }
    }
    public void SetBoss(BossEnemy boss)
    {
        this.boss = boss;
    }
    public virtual void MoveSet()
    {
        foreach (DropItemSO dropItem in dropItems)
        {
            for (int i = 0; i < dropItem.quantity; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
                Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                Instantiate(dropItem.prefab, spawnPosition, Quaternion.identity);
            }
        }
    }
    public void FlipToPlayer()
    {
        if (Player.Instance == null) return;
        float directionToPlayer = Player.Instance.transform.position.x - transform.position.x;
        if (directionToPlayer != 0)
        {
            float directionSign = Mathf.Sign(directionToPlayer);
            transform.localScale = new Vector3(directionSign, transform.localScale.y, transform.localScale.z);
        }
    }
}