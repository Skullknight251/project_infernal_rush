using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    private const string ANIM1_TRIG = "LightningTrig1";
    private const string ANIM2_TRIG = "LightningTrig2";
    [SerializeField] private int damage = 4;
    [SerializeField] private Animator animator;
    [SerializeField] private float lifeTime;
    private string[] attackTriggers = new string[] { ANIM1_TRIG, ANIM2_TRIG };

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        TriggerRandomLightningAnimation();
        Destroy(gameObject, lifeTime);
    }

    private void TriggerRandomLightningAnimation()
    {
        if (animator == null) return;
        int randomIndex = Random.Range(0, attackTriggers.Length);
        string selectedTrigger = attackTriggers[randomIndex];

        animator.SetTrigger(selectedTrigger);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<HurtBox>(out HurtBox hurtBox))
        {
            Enemy enemy = hurtBox.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                
            }
        }
    }
}