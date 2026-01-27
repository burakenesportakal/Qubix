using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private float blockSpace = 1f;
    
    public int x, y;
    public int colorID;


    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Init(int x, int y, int colorID, Sprite sprite)
    {
        this.x = x;
        this.y = y;
        this.colorID = colorID;
        _spriteRenderer.sprite = sprite;
        gameObject.name = $"Block {x},{y}";

        HandleBlockSpriteSize();
        HandleLayer();
    }

    private void HandleBlockSpriteSize()
    {
        HandleColliderSize();

        transform.localScale = Vector3.one;
        Bounds bounds = _spriteRenderer.bounds;
        float targetSize = blockSpace;
        float currentMaxSize = Mathf.Max(bounds.size.x, bounds.size.y);

        if (currentMaxSize > 0)
        {
            float newScale = targetSize / currentMaxSize;
            transform.localScale = new Vector3(newScale, newScale, 0.1f);
        }
    }

    private void HandleColliderSize()
    {
        if (_boxCollider != null)
            _boxCollider.size = _spriteRenderer.sprite.bounds.size;
    }

    private void HandleLayer()
    {
        float posY = transform.position.y;
        _spriteRenderer.sortingOrder = (int)(posY * 10);
    }

    private void OnMouseDown()
    {
        Debug.Log($"Týklandý: {x},{y}");
    }
}