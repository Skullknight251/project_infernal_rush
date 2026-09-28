using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEditor;
using UnityEngine;

public class BlackKnight : Enemy, IEnemyHasSkill
{
    
    [SerializeField] private float attack2StartDelay = .1f;
    [SerializeField] private GameObject blackSpearPrefab;
    [SerializeField] private Transform AttackDirection;
    [SerializeField] private CircleCollider2D attackRangeCollider;
    [SerializeField] private Vector2 jumpForce;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private CircleCollider2D jumpAndSlamHitBox;
    [SerializeField] private int jumpAndSlamDamage;
    [SerializeField] private float stunDelayTime;
    private List<Collider2D> hitsCollider = new List<Collider2D>();
    private ContactFilter2D filter;
    public float maxWaitTimeForPlayer = 3f;
    private const string ATTACK2_TRIGGER = "Attack2Trigger";
    private const string SKILL_TRIGGER = "SkillTrigger";

    protected override void Start()
    {
        base.Start();
        filter = ContactFilter2D.noFilter;
        filter.useTriggers = true;
    }

    void Update()
    {
        animator.SetBool(IS_STUNNED, isStunned);
    }

    

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public override IEnumerator Skill2()
    {
        if (isDead || Player.Instance == null)
            yield break;

        float timer = 0f;

        while (!IsPlayerInCQCRange())
        {
            if (isDead || Player.Instance == null)
                yield break;

            timer += Time.deltaTime;

            if (timer >= maxWaitTimeForPlayer)
                yield break;

            yield return null;
        }
        animator.SetTrigger(SKILL_TRIGGER);
        StartCoroutine(stunDelay());
    }
    public IEnumerator stunDelay()
    {
        yield return new WaitForSeconds(stunDelayTime);
        StartStun();
    }
    public void ExecuteJumpAndSlamDamage()
    {
        if (isDead || jumpAndSlamHitBox == null) return;

        hitsCollider.Clear();
        int count = jumpAndSlamHitBox.Overlap(filter, hitsCollider);

        foreach (Collider2D hit in hitsCollider)
        {
            CheckPlayerHit(hit, jumpAndSlamDamage);
        }
    }
    public override IEnumerator Skill1()
    {
        if (isDead || Player.Instance == null)
            yield break;

        float timer = 0f;
        while (!IsPlayerInCQCRange())
        {
            if (isDead || Player.Instance == null)
                yield break;

            timer += Time.deltaTime;

            if (timer >= maxWaitTimeForPlayer)
            {
                yield break;
            }

            yield return null;
        }
        animator.SetTrigger(ATTACK2_TRIGGER);
        StartCoroutine(SkillSignal(AttackDirection));
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
        }
        yield return new WaitForSeconds(attack2StartDelay);
        GameObject spear = Instantiate(blackSpearPrefab, AttackDirection);
        
        BlackSpear blackSpear = spear.GetComponent<BlackSpear>();

        if (blackSpear != null)
        {
            blackSpear.SetEnemyReference(this);
        }

        if (isDead)
            yield break;
        
    }
}
