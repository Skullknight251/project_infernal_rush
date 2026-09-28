using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public enum BackgroundType
    {
        Parallax,
        Infinite
    }

    [SerializeField] private float parallaxEffectValue;
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private bool infiniteHorizontal;
    [SerializeField] private bool infiniteVertical;
    [SerializeField] private BackgroundType backgroundType;

    [Header("Flame")]
    [SerializeField] private GameObject flamePrefab;
    [SerializeField] private Transform[] flamePoints;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private float textureUnitSizeX;
    private float textureUnitSizeY;

    public void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;

        SpawnFlames();
    }

    private void SpawnFlames()
    {
        if (flamePrefab == null || flamePoints == null)
            return;

        foreach (Transform point in flamePoints)
        {
            if (point == null)
                continue;

            GameObject flame = Instantiate(
                flamePrefab,
                point.position,
                Quaternion.identity,
                transform
            );

            flame.transform.localPosition = point.localPosition;
        }
    }

    private void LateUpdate()
    {
        if (backgroundType == BackgroundType.Parallax)
        {
            Vector3 deltaMovement =
                cameraTransform.position - lastCameraPosition;

            transform.position += new Vector3(
                deltaMovement.x * parallaxEffectMultiplier.x,
                0f,
                0f
            );

            lastCameraPosition = cameraTransform.position;

            if (infiniteHorizontal)
            {
                if (Mathf.Abs(
                    cameraTransform.position.x - transform.position.x
                ) >= textureUnitSizeX)
                {
                    float offsetPositionX =
                        (cameraTransform.position.x - transform.position.x)
                        % textureUnitSizeX;

                    transform.position = new Vector3(
                        cameraTransform.position.x + offsetPositionX,
                        transform.position.y,
                        transform.position.z
                    );
                }
            }
        }
    }
    private void OnDestroy()
    {
        Debug.Log($"BackgroundController destroyed: {gameObject.name}", this);
    }
}