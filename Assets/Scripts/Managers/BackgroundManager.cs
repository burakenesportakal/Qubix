using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Camera mainCamera;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _spriteSize;
    private float _lastOrthographicSize;
    private float _lastAspect;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null && _spriteRenderer.sprite != null)
            _spriteSize = _spriteRenderer.sprite.bounds.size;
        
        FitBackgroundToCamera();
    }

    private void LateUpdate() 
    {
        if (mainCamera.orthographicSize != _lastOrthographicSize || mainCamera.aspect != _lastAspect)
            FitBackgroundToCamera();
    }

    private void FitBackgroundToCamera()
    {
        if (_spriteRenderer == null || mainCamera == null) return;
        
        Vector3 camPos = mainCamera.transform.position;
        transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);

        _lastOrthographicSize = mainCamera.orthographicSize;
        _lastAspect = mainCamera.aspect;

        float cameraHeight = 2f * _lastOrthographicSize;
        float cameraWidth = cameraHeight * _lastAspect;

        float finalScale = Mathf.Max(cameraWidth / _spriteSize.x, cameraHeight / _spriteSize.y);
        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}