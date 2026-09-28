using UnityEngine;
using UnityEngine.InputSystem;

public class AimSkill : MonoBehaviour
{
    public static AimSkill Instance { get; private set; }

    [SerializeField] private Transform arrow;
    [SerializeField] private Transform originDirection;
    [SerializeField] private float aimTimeScale = 0.1f;

    private Quaternion playerOriginDir;
    private Camera mainCamera;
    private bool isAiming;
    private Vector2 currentAimDirection = Vector2.right;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isAiming) return;

        UpdateAimDirection();
    }

    public void StartAim(Transform customOrigin = null)
    {
        if (customOrigin != null)
        {
            originDirection = customOrigin;
        }

        if (originDirection == null && PlayerMovement.Instance != null)
        {
            originDirection = PlayerMovement.Instance.spellCastSpot != null
                ? PlayerMovement.Instance.spellCastSpot
                : PlayerMovement.Instance.transform;
        }

        if (arrow != null)
        {
            arrow.gameObject.SetActive(true);
        }

        isAiming = true;
        Time.timeScale = aimTimeScale;

        if (PlayerMovement.Instance != null)
        {
            playerOriginDir = PlayerMovement.Instance.transform.rotation;
        }

        UpdateAimDirection();
    }

    public Vector2 StopAim()
    {
        if (isAiming)
        {
            UpdateAimDirection();
        }

        isAiming = false;
        Time.timeScale = 1f;

        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }

        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.transform.rotation = playerOriginDir;
        }

        return currentAimDirection;
    }

    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            if (touchPosition != Vector2.zero)
            {
                return touchPosition;
            }
        }

        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }

    private Vector2 GetClampedAimDirection()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                return Vector2.right;
            }
        }

        Vector3 originPos;

        if (originDirection != null)
        {
            originPos = originDirection.position;
        }
        else if (PlayerMovement.Instance != null)
        {
            originPos = PlayerMovement.Instance.transform.position;
        }
        else
        {
            originPos = transform.position;
        }

        Vector2 pointerPosition = GetPointerPosition();

        Vector3 pointerWorldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                pointerPosition.x,
                pointerPosition.y,
                Mathf.Abs(mainCamera.transform.position.z - originPos.z)
            )
        );

        Vector2 direction = pointerWorldPosition - originPos;

        if (direction.sqrMagnitude < 0.001f)
        {
            return currentAimDirection;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, -90f, 0f);

        return new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );
    }

    public void UpdateAimDirection()
    {
        if (arrow == null) return;

        currentAimDirection = GetClampedAimDirection();

        float angle = Mathf.Atan2(
            currentAimDirection.y,
            currentAimDirection.x
        ) * Mathf.Rad2Deg;

        arrow.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetOriginDirection(Transform newOrigin)
    {
        originDirection = newOrigin;
    }
}