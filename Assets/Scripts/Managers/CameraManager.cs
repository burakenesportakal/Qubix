using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    private float cameraOffsetZ = -10f; // Derinlik
    [Range(0f, 10f)]
    [SerializeField] private float cameraPadding = 2f;   // Kenar boþluðu


    public void AdjustCamera(int boardWidth, int boardHeight)
    {
        float xCenter = (boardWidth - 1) / 2f;
        float yCenter = (boardHeight - 1) / 2f;

        Camera.main.transform.position = new Vector3(xCenter, yCenter, cameraOffsetZ);
        Camera.main.orthographicSize = (boardHeight / 2f) + cameraPadding;
    }
}
