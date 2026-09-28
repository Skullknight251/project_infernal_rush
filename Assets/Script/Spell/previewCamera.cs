using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    [SerializeField] private Transform previewSpot;
    [SerializeField] private float cameraOffsetZ = -5f;

    void LateUpdate()
    {
        if (previewSpot != null)
        {
            transform.position = new Vector3(
                previewSpot.position.x,
                previewSpot.position.y,
                cameraOffsetZ
            );
        }
    }
}