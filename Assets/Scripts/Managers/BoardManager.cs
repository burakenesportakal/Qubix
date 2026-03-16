using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public struct BlockSet
{
    public string name;
    public Sprite defaultSprite;
    public Sprite iconASprite;
    public Sprite iconBSprite;
    public Sprite iconCSprite;
}

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    [Range(2, 10)]
    public int width; //N 
    [Range(2, 10)]
    public int height; //M 
    [Range(1, 6)]
    public int colorCount; //K 

    [Header("Prefabs & Visuals")]
    public GameObject blockPrefab;
    public BlockSet[] blockSets;

    [Header("Conditions")]
    [SerializeField] private int _conditionA;
    [SerializeField] private int _conditionB;
    [SerializeField] private int _conditionC;

    [Header("Managers")]
    public CameraManager cameraManager;
    public DeadlockManager deadlockManager;

    [Header("Border")]
    public GameObject borderPrefab;

    private Block[,] _allBlocks;
    private List<Block> _inactivePool;
    private bool _isProcessing = false;

    [Header("Animation")]
    [SerializeField] private float _fallSpeed = 10f;

    private IEnumerator Start()
    {
        _isProcessing = true;

        CreateGrid();

        while (deadlockManager.isDeadlocked(_allBlocks, width, height))
        {
            yield return StartCoroutine(deadlockManager.ShuffleBoardProcces(_allBlocks, width, height, blockSets));
            UpdateBoardVisuals();
            yield return new WaitForSeconds(0.1f);
        }

        UpdateBoardVisuals();

        _isProcessing = false;
    }

    private void Update()
    {
        if (_isProcessing || !Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Block clickedBlock = hit.collider.GetComponent<Block>();
            if (clickedBlock != null)
            {
                List<Block> connectedBlocks = GetConnectedBlocks(clickedBlock);
                if (connectedBlocks.Count >= 2)
                {
                    if (SoundManager.Instance != null)
                        SoundManager.Instance.PlayConditionSounds(connectedBlocks.Count, _conditionA, _conditionB, _conditionC);

                    RemoveMatches(connectedBlocks);
                    StartCoroutine(FillHolesAnimated());
                }
            }
        }
    }

    private void GeneratePool()
    {
        int poolSize = width * height;
        _inactivePool = new List<Block>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject newBlock = Instantiate(blockPrefab, transform);
            Block blockScript = newBlock.GetComponent<Block>();
            newBlock.SetActive(false);
            _inactivePool.Add(blockScript);
        }
    }

    public Block GetBlockFromPool()
    {
        if (_inactivePool.Count > 0)
        {
            int lastIndex = _inactivePool.Count - 1;
            Block selectedBlock = _inactivePool[lastIndex];
            _inactivePool.RemoveAt(lastIndex);
            selectedBlock.gameObject.SetActive(true);
            return selectedBlock;
        }

        GameObject newBlock = Instantiate(blockPrefab, transform);
        return newBlock.GetComponent<Block>();
    }

    public void CreateGrid()
    {
        if (PlayerPrefs.HasKey("BoardWidth"))
        {
            width = PlayerPrefs.GetInt("BoardWidth");
            height = PlayerPrefs.GetInt("BoardHeight");
            colorCount = PlayerPrefs.GetInt("ColorCount");

            _conditionA = PlayerPrefs.GetInt("CondA");
            _conditionB = PlayerPrefs.GetInt("CondB");
            _conditionC = PlayerPrefs.GetInt("CondC");
        }

        GeneratePool();
        _allBlocks = new Block[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Block block = GetBlockFromPool();
                int randomColorID = Random.Range(0, colorCount);
                block.transform.position = new Vector2(x, y);
                block.Init(x, y, randomColorID, blockSets[randomColorID]);
                _allBlocks[x, y] = block;
            }
        }
        if (cameraManager != null)
            cameraManager.AdjustCamera(width, height);

        CreateBoardFrame();
        UpdateBoardVisuals();
    }

    public Block GetBlockAt(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return _allBlocks[x, y];
    }

    private List<Block> GetConnectedBlocks(Block startBlock)
    {
        List<Block> result = new List<Block>();
        Queue<Block> blocksToCheck = new Queue<Block>();
        HashSet<Block> visited = new HashSet<Block>();

        blocksToCheck.Enqueue(startBlock);
        visited.Add(startBlock);
        result.Add(startBlock);

        int targetColorID = startBlock.colorID;
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (blocksToCheck.Count > 0)
        {
            Block current = blocksToCheck.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nextX = current.x + dx[i];
                int nextY = current.y + dy[i];

                Block neighbor = GetBlockAt(nextX, nextY);
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.colorID == targetColorID)
                {
                    visited.Add(neighbor);
                    result.Add(neighbor);
                    blocksToCheck.Enqueue(neighbor);
                }
            }
        }
        return result;
    }

    private void RemoveBlock(Block block)
    {
        _allBlocks[block.x, block.y] = null;
        block.gameObject.SetActive(false);
        _inactivePool.Add(block);
    }

    private void RemoveMatches(List<Block> blocksToRemove)
    {
        foreach (Block block in blocksToRemove) RemoveBlock(block);
    }

    private void CreateBoardFrame()
    {
        float xCenter = (width - 1) * 0.5f;
        float yCenter = (height - 1) * 0.5f;

        GameObject frame = Instantiate(borderPrefab, transform);
        frame.transform.position = new Vector3(xCenter, yCenter, 0);

        SpriteRenderer sr = frame.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced;

            frame.transform.localScale = Vector3.one;

            sr.size = new Vector2(width + 1f, height + 1f);
        }
    }

    private IEnumerator FillHolesAnimated()
    {
        _isProcessing = true;

        for (int x = 0; x < width; x++)
        {
            int floor = 0;
            for (int y = 0; y < height; y++)
            {
                Block block = _allBlocks[x, y];
                if (block != null)
                {
                    if (y > floor)
                    {
                        _allBlocks[x, y] = null;
                        _allBlocks[x, floor] = block;
                        block.x = x;
                        block.y = floor;
                    }
                    floor++;
                }
            }
            for (int y = floor; y < height; y++)
            {
                Block newBlock = GetBlockFromPool();
                newBlock.transform.position = new Vector3(x, height + (y - floor) + 1, 0);
                int randomID = Random.Range(0, colorCount);
                newBlock.Init(x, y, randomID, blockSets[randomID]);
                _allBlocks[x, y] = newBlock;
            }
        }

        bool isMoving = true;
        float fallDelta = _fallSpeed * Time.deltaTime;

        while (isMoving)
        {
            isMoving = false;
            fallDelta = _fallSpeed * Time.deltaTime;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Block block = _allBlocks[x, y];
                    if (block != null)
                    {
                        Vector3 targetPos = new Vector3(x, y, 0);
                        Vector3 currentPos = block.transform.position;

                        if (Mathf.Abs(currentPos.y - targetPos.y) > 0.05f)
                        {
                            block.transform.position = Vector3.MoveTowards(currentPos, targetPos, fallDelta);
                            isMoving = true;
                        }
                        else
                        {
                            block.transform.position = targetPos;
                            block.UpdateSortingOrder();
                        }
                    }
                }
            }
            yield return null;
        }

        UpdateBoardVisuals();

        if (deadlockManager.isDeadlocked(_allBlocks, width, height))
        {
            yield return StartCoroutine(deadlockManager.ShuffleBoardProcces(_allBlocks, width, height, blockSets));
            UpdateBoardVisuals();
        }

        _isProcessing = false;
    }

    private void UpdateBoardVisuals()
    {
        bool[,] visited = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!visited[x, y] && _allBlocks[x, y] != null)
                {
                    Block currentBlock = _allBlocks[x, y];
                    List<Block> group = GetConnectedBlocks(currentBlock);

                    int groupSize = group.Count;
                    foreach (Block member in group)
                    {
                        visited[member.x, member.y] = true;

                        member.UpdateVisualState(groupSize, _conditionA, _conditionB, _conditionC);
                    }
                }
            }
        }
    }
}