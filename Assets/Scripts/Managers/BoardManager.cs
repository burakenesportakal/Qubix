using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
        if (_isProcessing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Block clickedBlock = hit.collider.GetComponent<Block>();

                if (clickedBlock != null)
                {
                    List<Block> connectedBlocks = GetConnectedBlocks(clickedBlock);

                    Debug.Log($"Tiklanan Renk ID: {clickedBlock.colorID} | Bagli Blok Sayisi: {connectedBlocks.Count}");

                    if (connectedBlocks.Count >= 2)
                    {
                        RemoveMatches(connectedBlocks);
                        StartCoroutine(FillHolesAnimated());
                    }
                }
            }
        }
    }

    private void GeneratePool()
    {
        _inactivePool = new List<Block>();
        int poolSize = width * height * 2;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject newBlock = Instantiate(blockPrefab);
            newBlock.transform.parent = this.transform;
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
        else
        {
            GameObject newBlock = Instantiate(blockPrefab);
            newBlock.transform.parent = this.transform;
            Block newBlockScript = newBlock.GetComponent<Block>();
            return newBlockScript;
        }
    }

    public void CreateGrid()
    {
        GeneratePool();
        _allBlocks = new Block[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Block block = GetBlockFromPool();
                block.transform.position = new Vector2(x, y);
                int randomColorID = Random.Range(0, colorCount);
                block.Init(x, y, randomColorID, blockSets[randomColorID]);
                _allBlocks[x, y] = block;
            }
        }
        if (cameraManager != null)
        {
            cameraManager.AdjustCamera(width, height);
        }
        else
        {
            Debug.LogError("CameraManager atanmam��");
        }

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

        blocksToCheck.Enqueue(startBlock);
        result.Add(startBlock);

        int targetColorID = startBlock.colorID;

        Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

        while (blocksToCheck.Count > 0)
        {
            Block current = blocksToCheck.Dequeue();

            foreach (Vector2 direction in directions)
            {
                int nextX = current.x + (int)direction.x;
                int nextY = current.y + (int)direction.y;

                Block neighbor = GetBlockAt(nextX, nextY);
                if (neighbor != null && !result.Contains(neighbor) && neighbor.colorID == targetColorID && neighbor.gameObject.activeSelf)
                {
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
        float xCenter = (width - 1) / 2f;
        float yCenter = (height - 1) / 2f;

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
        bool crashOccured = false;
        try
        {
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
                            block.name = $"Block {x},{floor}";
                        }
                        floor++;
                    }
                }
                for (int y = floor; y < height; y++)
                {
                    Block newBlock = GetBlockFromPool();
                    Vector3 startPos = new Vector3(x, height + (y - floor) + 1, 0);
                    newBlock.transform.position = startPos;

                    int randomID = Random.Range(0, colorCount);
                    newBlock.Init(x, y, randomID, blockSets[randomID]);
                    _allBlocks[x, y] = newBlock;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("DATA HATASI: " + e.Message);
            crashOccured = true;
        }

        if (!crashOccured)
        {
            bool isMoving = true;
            while (isMoving)
            {
                isMoving = false;
                try
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Block block = _allBlocks[x, y];
                            if (block != null)
                            {
                                Vector3 targetPos = new Vector3(x, y, 0);
                                float dist = Vector3.Distance(block.transform.position, targetPos);
                                if (dist > 0.05f)
                                {
                                    block.transform.position = Vector3.MoveTowards(block.transform.position, targetPos, _fallSpeed * Time.deltaTime);
                                    isMoving = true;
                                }
                                else
                                {
                                    block.transform.position = targetPos;
                                }
                            }
                        }
                    }
                }
                catch { isMoving = false; }
                yield return null;
            }

            UpdateBoardVisuals();

            if (deadlockManager.isDeadlocked(_allBlocks, width, height))
            {
                yield return StartCoroutine(deadlockManager.ShuffleBoardProcces(_allBlocks, width, height, blockSets));
                UpdateBoardVisuals();
            }
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