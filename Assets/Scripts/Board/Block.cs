using UnityEngine;

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

    public void Init(int x, int y, int colorID, BlockSet set)
    {
        this.x = x;
        this.y = y;
        this.colorID = colorID;
        _blockSet = set;
        _spriteRenderer.sprite = set.defaultSprite;

        if (_boxCollider != null) _boxCollider.size = _spriteRenderer.sprite.bounds.size;

        if (OriginalScale == Vector3.zero)
        {
            Bounds bounds = _spriteRenderer.bounds;
            float currentMaxSize = Mathf.Max(bounds.size.x, bounds.size.y);
            if (currentMaxSize > 0)
            {
                float newScale = blockSpace / currentMaxSize;
                OriginalScale = new Vector3(newScale, newScale, 0.1f);
            }
        }
        transform.localScale = OriginalScale;
        UpdateSortingOrder();
    }

    public void UpdateSortingOrder()
    {
        float posY = transform.position.y;
        _spriteRenderer.sortingOrder = (int)(posY * 10);
    }

    public void UpdateVisualState(int groupSize, int condA, int condB, int condC)
    {
        Sprite newSprite;
        if (groupSize >= condC && _blockSet.iconCSprite != null) newSprite = _blockSet.iconCSprite;
        else if (groupSize >= condB && _blockSet.iconBSprite != null) newSprite = _blockSet.iconBSprite;
        else if (groupSize >= condA && _blockSet.iconASprite != null) newSprite = _blockSet.iconASprite;
        else newSprite = _blockSet.defaultSprite;

        if (_spriteRenderer.sprite != newSprite) _spriteRenderer.sprite = newSprite;
    }
}