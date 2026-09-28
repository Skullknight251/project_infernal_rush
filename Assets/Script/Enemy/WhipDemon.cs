using System.Collections;
using System.IO.Pipes;
using UnityEngine;

public class WhipDemon : Enemy,IEnemyHasSkill
{
    [SerializeField] private float attack2StartDelay = .1f;
    [SerializeField] private GameObject whipPrefab;
    [SerializeField] private Transform AttackDirection;
    [SerializeField] private float returnSpeed = 50;
    [SerializeField] private float delayAfterAttack= 1f;
    protected bool canMove = true;
    
    public float maxWaitTimeForPlayer = 3f;
    private const string ATTACK2_TRIGGER = "Attack2Trigger";
    private const string DIVE_TRIGGER = "DiveTrigger";
    [SerializeField] private Vector2 originPosition;
    [SerializeField] private Vector2 relativeOriginPosition;

    [Header("Dive Settings")]
    [SerializeField] private float targetVerticalOffset = 2f;
    [SerializeField] private float diveSpeed = 15f;
    [SerializeField] private float diveDuration = 1f;
    [SerializeField] private float diveStartDelay = 0.2f;
    private bool isDiving;
    protected override void Start()
    {
        base.Start();
        PositionCheck();
        originPosition = transform.position;
        relativeOriginPosition = transform.position - boss.transform.position;
        
    }

    private void Update()
    {
        animator.SetBool(IS_STUNNED, isStunned);

        originPosition = (Vector2)boss.transform.position + relativeOriginPosition;

        if (canMove && !isDiving && CanRun)
        {
            transform.position += new Vector3(MoveSpeed * Time.deltaTime, 0f, 0f);
        }
    }



    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override IEnumerator Skill1()
    {
        if (isDead || Player.Instance == null)
            yield break;

        canMove = false;
        rb.linearVelocity = Vector2.zero;

        while (!IsPlayerInCQCRange())
        {
            if (isDead || Player.Instance == null)
            {
                canMove = true;
                yield break;
            }

            Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;

            rb.linearVelocity = direction ;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        StartCoroutine(SkillSignal(AttackDirection));
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
        }
        animator.SetTrigger(ATTACK2_TRIGGER);
        
        yield return new WaitForSeconds(attack2StartDelay);

        if (isDead)
            yield break;

        GameObject whip = Instantiate(whipPrefab, AttackDirection);

        Whip demonWhip = whip.GetComponent<Whip>();

        if (demonWhip != null)
        {
            demonWhip.SetEnemyReference(this);
        }
        
        canMove = true;
        yield return new WaitForSeconds (delayAfterAttack);
        canMove = false;
        if (isDead)
            yield break;

        while (Vector2.Distance(transform.position, originPosition) > 0.05f)
        {
            if (isDead)
                yield break;

            transform.position = Vector2.MoveTowards(transform.position,originPosition,returnSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = originPosition;

        rb.linearVelocity = Vector2.zero;
        canMove = true;
    }

    public override IEnumerator Skill2()
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
            Vector2 direction = (targetPosition - rb.position).normalized;

            rb.linearVelocity = direction * diveSpeed;

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();


        }
        isDiving = false;
    }
}

