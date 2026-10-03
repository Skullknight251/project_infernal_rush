using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Intro")]
    public float playerBossDistance = 46f;
    [SerializeField] private float maxMoveSpeed;
    public float PlayerBossDistance => playerBossDistance;
    [SerializeField] private float accelerationValue = .001f;
    [SerializeField] private bool isIntro ;
    [SerializeField] private float firstComboDelay = 2f;

    private bool gameplayStarted;
    private bool introStarted;
    [Header("Spawn Points")]
    [SerializeField] private Transform enemyHighPlaceSpawnPoint;
    [SerializeField] private Transform enemyMediumPlaceSpawnPoint;
    [SerializeField] private Transform enemyLowPlaceSpawnPoint;
    [SerializeField] private float distanceAdjustment;
    private const string LOW_SPAWN = "Low_summon";
    private const string MEDIUM_SPAWN = "Medium_summon";
    private const string HIGH_SPAWN = "High_summon";
    private Animator animator;

    [Header("Boss Movement & Combos")]
    public float moveSpeed ;
    public float introMoveSpeed = 15f;
    public float playMoveSpeed = 10f;
    [SerializeField] private List<BossComboSO> easyComboList;
    [SerializeField] private List<BossComboSO> mediumComboList;
    [SerializeField] private List<BossComboSO> hardComboList;

    [Header("Combo Difficulty")]
    [SerializeField] private int mediumComboStart = 5;
    [SerializeField] private int hardComboStart = 10;

    private HashSet<Enemy> activeEnemies = new HashSet<Enemy>();

    private Rigidbody2D rb;

    private int index;
    private int comboCount;

    private bool isExecutingCombo = false;

    public static BossEnemy Instance { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        StartCoroutine(BossComboLoop());
    }

    public void StartIntro()
    {
        introStarted = true;
        isIntro = true;
        gameplayStarted = false;
    }

    private IEnumerator BossComboLoop()
    {
        while (true)
        {
            if (!gameplayStarted)
            {
                yield return null;
                continue;
            }

            if (PlayerMovement.Instance == null || !PlayerMovement.Instance.CanRun)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(firstComboDelay);

            break;
        }

        while (true)
        {
            if (!gameplayStarted)
            {
                yield return null;
                continue;
            }

            if (PlayerMovement.Instance == null || !PlayerMovement.Instance.CanRun)
            {
                yield return null;
                continue;
            }

            List<BossComboSO> currentComboList = GetCurrentComboList();

            if (currentComboList == null || currentComboList.Count == 0)
            {
                yield return null;
                continue;
            }

            if (!isExecutingCombo)
            {
                BossComboSO randomCombo =
                    currentComboList[Random.Range(0, currentComboList.Count)];

                if (randomCombo.waitForPreviousEnemiesToDie)
                {
                    yield return StartCoroutine(WaitForAllEnemiesToDie());
                }

                yield return StartCoroutine(ExecuteComboRoutine(randomCombo));

                comboCount++;
            }
        }
    }

    private List<BossComboSO> GetCurrentComboList()
    {
        if (comboCount >= hardComboStart)
        {
            return hardComboList;
        }

        if (comboCount >= mediumComboStart)
        {
            return mediumComboList;
        }

        return easyComboList;
    }

    private void FixedUpdate()
    {
        if (!introStarted)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isIntro)
        {
            moveSpeed = introMoveSpeed;
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

            CheckIntroFinished();
            return;
        }

        if (PlayerMovement.Instance == null || !PlayerMovement.Instance.CanRun)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        playMoveSpeed += accelerationValue * Time.deltaTime;
        
        if (playMoveSpeed <= maxMoveSpeed)
        {
            moveSpeed = playMoveSpeed;
        }
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
    }
    private void CheckIntroFinished()
    {
        if (gameplayStarted)
            return;

        if (PlayerMovement.Instance == null)
            return;

        float targetX = PlayerMovement.Instance.transform.position.x + playerBossDistance ;

        if (transform.position.x + distanceAdjustment >= targetX)
        {
            gameplayStarted = true;
            isIntro = false;

            GameManager.Instance.StartGameplay();
        }
    }
    private IEnumerator ExecuteComboRoutine(BossComboSO combo)
    {
        isExecutingCombo = true;
        index = 0;

        foreach (BossActionSO action in combo.actionsInCombo)
        {
            switch (action.spawnLocation)
            {
                case SpawnLocation.Low:
                    animator.SetTrigger(LOW_SPAWN);
                    break;
                case SpawnLocation.Medium:
                    animator.SetTrigger(MEDIUM_SPAWN);
                    break;
                case SpawnLocation.High:
                    animator.SetTrigger(HIGH_SPAWN);
                    break;
            }
            yield return StartCoroutine(ExecuteActionRoutine(action));

            if (action.cooldownAfterAction > 0)
            {
                yield return new WaitForSeconds(action.cooldownAfterAction);
            }
        }

        yield return new WaitForSeconds(combo.cooldownAfterCombo);

        isExecutingCombo = false;
    }

    private IEnumerator WaitForAllEnemiesToDie()
    {
        CleanupDeadEnemies();

        while (activeEnemies.Count > 0)
        {
            CleanupDeadEnemies();
            yield return null;
        }
    }

    private IEnumerator ExecuteActionRoutine(BossActionSO action)
    {
        Transform targetSpawnPoint = GetSpawnPoint(action.spawnLocation);

        if (targetSpawnPoint != null && action.enemyPrefab != null)
        {
            for (int i = 0; i < action.spawnCount; i++)
            {
                SpawnEnemy(action.enemyPrefab, targetSpawnPoint, index);
                index++;

                if (i < action.spawnCount - 1 &&
                    action.delayBetweenSpawns > 0)
                {
                    yield return new WaitForSeconds(action.delayBetweenSpawns);
                }
            }
        }
    }

    private void CleanupDeadEnemies()
    {
        
        activeEnemies.RemoveWhere(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
    }

    private void SpawnEnemy(GameObject prefab, Transform spawnPoint, int index)
    {
        GameObject obj = SimplePoolManager.Instance.Spawn(prefab, spawnPoint.position, Quaternion.identity);

        if (obj.TryGetComponent(out Enemy enemy))
        {
            enemy.SetBoss(this);
            enemy.index = index;
            activeEnemies.Add(enemy);
        }
    }

    public void NotifyEnemyDeath(Enemy enemy)
    {
        if (enemy == null)
            return;

        activeEnemies.Remove(enemy);
    }

    private Transform GetSpawnPoint(SpawnLocation location)
    {
        switch (location)
        {
            case SpawnLocation.High:
                return enemyHighPlaceSpawnPoint;

            case SpawnLocation.Medium:
                return enemyMediumPlaceSpawnPoint;

            case SpawnLocation.Low:
                return enemyLowPlaceSpawnPoint;

            default:
                return enemyLowPlaceSpawnPoint;
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    public void ResetBoss()
    {
        StopAllCoroutines();

        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                SimplePoolManager.Instance.Despawn(enemy.gameObject);
            }
        }

        activeEnemies.Clear();
        isExecutingCombo = false;
        gameplayStarted = false;
        introStarted = false;
        isIntro = false;
        index = 0;
        comboCount = 0;
        rb.linearVelocity = Vector2.zero;
    }
    public void ClearInstance()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}