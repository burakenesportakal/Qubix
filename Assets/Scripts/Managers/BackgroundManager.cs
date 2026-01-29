using UnityEngine; 
public class BackgroundManager : MonoBehaviour
{
    public Camera mainCamera;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate() 
    {
        FitBackgroundToCamera();
    }

    private void FitBackgroundToCamera()
    {
        if (_spriteRenderer == null || mainCamera == null) return;
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, transform.position.z);

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;

        float scaleY = cameraHeight / spriteSize.y;
        float scaleX = cameraWidth / spriteSize.x;

        float finalScale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}