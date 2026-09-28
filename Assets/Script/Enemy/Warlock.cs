using UnityEngine;
using System.Collections;

public class Warlock : Enemy, IShooter
{
    private const string SHOOT1_TRIGGER = "Shoot1Trigger";
    private const string SHOOT2_TRIGGER = "Shoot2Trigger";

    [Header("Shooter Settings")]
    [SerializeField] private float skillCoolDownMax = 3f;
    [SerializeField] private GameObject magicPrefab;
    [SerializeField] private GameObject fireballMagicPrefab;
    [SerializeField] private Transform FireDirection;
    [SerializeField] private Vector2 frontPlayerOffset;

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
        Vector2 dir = FireDirection.transform.position - Player.Instance.transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        FireDirection.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public override IEnumerator Shoot2()
    {
        if (isDead) yield break;
        if (Player.Instance == null) yield break;
        StartCoroutine(SkillSignal(FireDirection));
        if (!TutorialCheck.Instance.IsCompleted(TutorialCheck.TutorialStep.parry))
        {
            TutorialManager.Instance.SpawnTutorial(TutorialCheck.TutorialStep.parry, TutorialPoint.Instance.parryTutorialPoint);
        }
        animator.SetTrigger(SHOOT2_TRIGGER);
        
        yield return new WaitForSeconds(attackDelay);
        GameObject fireball = Instantiate(fireballMagicPrefab, FireDirection.position, FireDirection.rotation);
        fireball.SetActive(true);

        FireballMagic projectile = fireball.GetComponent<FireballMagic>();
        if (projectile != null) projectile.SetEnemyReference(this);
        projectile.SetTarget(Player.Instance.gameObject);
        
    }

    public override IEnumerator Shoot1()
    {
        if (isDead) yield break;
        if (Player.Instance == null) yield break;
        animator.SetTrigger(SHOOT1_TRIGGER);

        yield return new WaitForSeconds(attackDelay);
        GameObject fireMagic = Instantiate(magicPrefab, (Vector2)Player.Instance.transform.position + frontPlayerOffset, Player.Instance.transform.rotation);
        fireMagic.SetActive(true);

        FireMagic projectile = fireMagic.GetComponent<FireMagic>();
        if (projectile != null) projectile.SetEnemyReference(this);
        projectile.SetTarget(Player.Instance.gameObject);

    }
}
