using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private Transform target;

    [SerializeField] private float playerOffSetX;
    [SerializeField] private float bossOffSetX;
    [SerializeField] private float followBossSpeed = 1f;
    [SerializeField] private float followPlayerSpeed = 5f;
    [SerializeField] private float followTransitionDuration = 1.5f;
    [SerializeField] private Transform cameraStartPoint;
    private float followSpeed;
    private float offsetX;
    private bool isFollowing;
    private bool isTransitioning;

    private float transitionTimer;
    private float transitionStartX;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / followTransitionDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            float targetX = target.position.x + offsetX;
            float newX = Mathf.Lerp(transitionStartX, targetX, smoothT);

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            if (t >= 1f)
            {
                isTransitioning = false;
                isFollowing = true;
            }
            return;
        }

        if (isFollowing)
        {
            transform.position = new Vector3(
                target.position.x + offsetX,
                transform.position.y,
                transform.position.z
            );
        }
    }

    public void ChangeTarget()
    {
        if (Player.Instance != null && target == Player.Instance.transform)
        {
            followSpeed = followPlayerSpeed;
            offsetX = playerOffSetX;
        }
        else
        {
            followSpeed = followBossSpeed;
            offsetX = bossOffSetX;
        }
    }

    public void StartFollowingPlayer()
    {
        if (Player.Instance == null) return;

        target = Player.Instance.transform;
        ChangeTarget();

        //transitionStartX = transform.position.x;
        //transitionTimer = 0f;
        //isTransitioning = true;
        //isFollowing = false;
    }

    public void StartFollowingBoss()
    {
        if (BossEnemy.Instance == null) return;

        target = BossEnemy.Instance.transform;
        ChangeTarget();

        transitionStartX = transform.position.x;
        transitionTimer = 0f;
        isTransitioning = true;
        isFollowing = false;
    }
    public void ResetCamera()
    {
        isFollowing = false;
        isTransitioning = false;
        target = null;

        transform.SetPositionAndRotation(
            cameraStartPoint.position,
            cameraStartPoint.rotation
        );
    }
}