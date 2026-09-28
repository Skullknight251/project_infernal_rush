using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class BlackShieldKnight : Enemy, IEnemyHasSkill, IShooter
{

    [SerializeField] private float attack2StartDelay = .1f;
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject canonBallPrefab;
    [SerializeField] private Transform attackDirection;
    [SerializeField] private Transform fireDirection;
    [SerializeField] private GameObject shieldCollider;

    public float maxWaitTimeForPlayer = 3f;
    private const string ATTACK2_TRIGGER = "Attack2Trigger";
    private const string SKILL_TRIGGER = "SkillTrigger";

    protected override void Start()
    {
        base.Start();
        ShieldCollider shield = shieldCollider.GetComponent<ShieldCollider>();
        shield.SetEnemy(this);
        
        StartCoroutine(BlockTutorial());
    }

    void Update()
    {
        shieldCollider.SetActive(!isStunned);
        animator.SetBool(IS_STUNNED, isStunned);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public IEnumerator BlockTutorial()
    {
        yield return new WaitForSeconds(1f);
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.block))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.block, TutorialPoint.Instance.basicSkillTutorialPoint);
        }
    }
    public override IEnumerator Shoot1()
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
        if (IsPlayerInCQCRange())
        {
            animator.SetTrigger(SKILL_TRIGGER);
            StartCoroutine(SkillSignal(fireDirection));
            if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
            {
                TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
            }
            yield return new WaitForSeconds(attackDelay);
            GameObject canonBall = Instantiate(canonBallPrefab, fireDirection.position, fireDirection.rotation);
            canonBall.SetActive(true);

            CanonBall projectile = canonBall.GetComponent<CanonBall>();
            if (projectile != null) projectile.SetEnemyReference(this);
            projectile.SetTarget(Player.Instance.gameObject);
            
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
        StartCoroutine(SkillSignal(attackDirection));
        animator.SetTrigger(ATTACK2_TRIGGER);
        
        yield return new WaitForSeconds(attack2StartDelay);
        GameObject shield = Instantiate(shieldPrefab, attackDirection);

        BlackShield blackShield = shield.GetComponent<BlackShield>();

        if (blackShield != null)
        {
            blackShield.SetEnemyReference(this);
        }

        if (isDead)
            yield break;
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, Player.Instance.tutorialSpawnPoint);
        }
    }
}
