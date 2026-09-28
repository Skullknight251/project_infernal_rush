using System.Collections;
using System.Linq;
using UnityEngine;

public class HellBat : Enemy, IShooter, IEnemyHasSkill
{
    
    [SerializeField] private float targetVerticalOffset = 2f;

    private const string DIVE_TRIGGER = "DiveTrigger";

    private const string ATTACK_TRIGGER = "AttackTrigger";

    [Header("Shooter Settings")]
    [SerializeField] private float skillCoolDownMax = 3f;
    [SerializeField] private GameObject soundWavePrefab;
    [SerializeField] private Transform FireDirection;

    [Header("Dive Settings")]
    [SerializeField] private float diveSpeed = 15f;
    [SerializeField] private float diveDuration = 1f;
    [SerializeField] private float diveStartDelay = 0.2f; 
    

    [Header("Move Settings")]
    public Vector2 position;
    private bool isDiving;
    protected override void Start()
    {   
        base.Start();
        transform.position = GetSeparationDirection();
        PositionCheck();
        
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {

        if (!isDiving && CanRun)
        {
            transform.position += new Vector3(MoveSpeed * Time.deltaTime,0f,0f);
            
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override IEnumerator Shoot1()
    {
        if (isDead) yield break;
        if (Player.Instance == null) yield break;
        animator.SetTrigger(ATTACK_TRIGGER);
        StartCoroutine(SkillSignal(FireDirection));
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
        }
        yield return new WaitForSeconds(attackDelay);

        GameObject soundWave = Instantiate(soundWavePrefab, FireDirection.position, FireDirection.rotation);
        soundWave.SetActive(true);

        SoundWave projectile = soundWave.GetComponent<SoundWave>();
        if (projectile != null) projectile.SetEnemyReference(this);
        

        projectile.SetTarget(Player.Instance.gameObject);
        
    }

    

    public override IEnumerator Skill1()
    {
        if (isDead) yield break;
        if (Player.Instance == null) yield break;
        animator.SetTrigger(DIVE_TRIGGER);
        yield return new WaitForSeconds(diveStartDelay);
        isDiving = true;

        Vector2 targetPosition = (Vector2)Player.Instance.transform.position - Vector2.up * targetVerticalOffset;
        float elapsedTime = 0f;

        while (elapsedTime < diveDuration)
        {
            if (!CanRun)
                yield break;
            Vector2 direction = (targetPosition   - rb.position).normalized;

            rb.linearVelocity = direction * diveSpeed;

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();

            
        }
        isDiving = false;
    }
}