using UnityEngine;

public class Paladin : Enemy
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private Collider2D weaponCollider;
    protected override void Start()
    {
        base.Start();
    }
    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        Suicide();
    }

    public void Suicide()
    {
        rb.linearVelocity = new Vector2(
           -moveSpeed,
           rb.linearVelocity.y
        );
    }
}
