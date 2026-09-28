using System.Collections;
using UnityEngine;

public class FireSkull : Enemy,IShooter
{
    private const string ATTACK_TRIGGER = "AttackTrigger";

    [Header("Shooter Settings")]
    [SerializeField] private float skillCoolDownMax = 3f;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private Transform FireDirection;


    [Header("Move Settings")]
    public Vector2 position;

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

    void Update()
    {
        if ( CanRun)
        {
            transform.position += new Vector3(MoveSpeed * Time.deltaTime, 0f, 0f);

        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void RotateToPlayer()
    {
        if (Player.Instance == null) return;
        Vector2 dir =  FireDirection.transform.position - Player.Instance.transform.position ;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        FireDirection.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public override IEnumerator Shoot1()
    {
        if (isDead) yield break;
        if (Player.Instance == null) yield break;
        StartCoroutine(SkillSignal(FireDirection));
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
        }
        animator.SetTrigger(ATTACK_TRIGGER);
        RotateToPlayer();
        
        yield return new WaitForSeconds(attackDelay);

        float distance = (Player.Instance.transform.position - FireDirection.position).magnitude; 

        GameObject beam = Instantiate(beamPrefab, FireDirection.position,FireDirection.rotation);

        Vector3 currentScale = beam.transform.localScale;
        beam.transform.localScale = new Vector3(distance/13, currentScale.y, currentScale.z);

        beam.SetActive(true);

        Beam projectile = beam.GetComponent<Beam>();
        if (projectile != null) projectile.SetEnemyReference(this);
        projectile.SetTarget(Player.Instance.gameObject);
        
    }

}
