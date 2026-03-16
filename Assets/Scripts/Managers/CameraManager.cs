using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    private const float CAMERA_OFFSET_Z = -10f;
    [Range(0f, 10f)]
    [SerializeField] private float cameraPadding = 2f;

    public void AdjustCamera(int boardWidth, int boardHeight)
    {
        float xCenter = (boardWidth - 1) * 0.5f;
        float yCenter = (boardHeight - 1) * 0.5f;

        Camera.main.transform.position = new Vector3(xCenter, yCenter, CAMERA_OFFSET_Z);
        Camera.main.orthographicSize = (boardHeight * 0.5f) + cameraPadding;
    }
}
