using UnityEngine;
using UnityEngine.UIElements;

public class Block : MonoBehaviour
{
    [SerializeField] private float blockSpace = 1f;
    
    public int x, y;
    public int colorID;


    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private BlockSet _blockSet;

    public Vector3 OriginalScale { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        HandleLayer();
    }

    public void Init(int x, int y, int colorID, BlockSet set)
    {
        this.x = x;
        this.y = y;
        this.colorID = colorID;

        this._blockSet = set;
        _spriteRenderer.sprite = _blockSet.defaultSprite;
        gameObject.name = $"Block {x},{y}";

        HandleBlockSpriteSize();
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
            Vector3 calculatedScale = new Vector3(newScale, newScale, 0.1f);
            transform.localScale = calculatedScale;
            OriginalScale = calculatedScale;
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

    public void UpdateVisualState(int groupSize, int condA, int condB, int condC)
    {
        if(groupSize >= condC)
        {
            if (_blockSet.iconCSprite != null) _spriteRenderer.sprite = _blockSet.iconCSprite;
        }
        else if (groupSize >= condB)
        {
            if (_blockSet.iconBSprite != null) _spriteRenderer.sprite = _blockSet.iconBSprite;
        }
        else if (groupSize >= condA)
        {
            if (_blockSet.iconASprite != null) _spriteRenderer.sprite = _blockSet.iconASprite;
        }
        else
        {
            _spriteRenderer.sprite = _blockSet.defaultSprite;
        }
    }
}