using System.Collections;
using System.IO.Pipes;
using UnityEngine;

public class HellHound : Enemy,IEnemyHasSkill
{
    [SerializeField] private float attack2StartDelay = 0f;
    [SerializeField] private GameObject houndFangPrefab;
    [SerializeField] private Transform AttackDirection;
    public float maxWaitTimeForPlayer = 3f;
    
    private const string ATTACK2_TRIGGER = "Attack2Trigger";
    
    protected override void Start()
    {
        base.Start();
        
    }

    void Update()
    {
        animator.SetBool(IS_STUNNED,isStunned);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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
        StartCoroutine(SkillSignal(AttackDirection));
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
        }
        animator.SetTrigger(ATTACK2_TRIGGER);
        
        yield return new WaitForSeconds(attack2StartDelay);
        GameObject fang = Instantiate(houndFangPrefab,AttackDirection);
        
        HoundFang houndFang = fang.GetComponent<HoundFang>();

        if (houndFang != null)
        {
            houndFang.SetEnemyReference(this);
        }
        

        if (isDead)
            yield break;
        
    }
}
