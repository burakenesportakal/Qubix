using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    [Range(2,10)]
    public int width; //N 
    [Range(2,10)]
    public int height; //M 
    [Range(1,6)]
    public int colorCount; //K 

    [Header("Prefabs & Visuals")]
    public GameObject blockPrefab;
    public Sprite[] blockSprites;

    [Header("Conditions")]
    [SerializeField] private int conditionA;
    [SerializeField] private int conditionB;
    [SerializeField] private int conditionC;

    [Header("Camera Manager")]
    public CameraManager cameraManager;

    private Block[,] _allBlocks;
    private List<Block> _inactivePool;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider != null)
            {
                Block clickedBlock = hit.collider.GetComponent<Block>();

                if(clickedBlock != null)
                {
                    List<Block> connectedBlocks = GetConnectedBlocks(clickedBlock);

                    Debug.Log($"Týklanan Renk ID: {clickedBlock.colorID} | Baðlý Blok Sayýsý: {connectedBlocks.Count}");

                    if (connectedBlocks.Count >= 2)
                    {
                        RemoveMatches(connectedBlocks);
                        FillHoles();
                    }
                }
            }
        }
    }

    private void GeneratePool() {
        _inactivePool = new List<Block>();
        int poolSize = width * height * 2;

        for(int i = 0; i < poolSize; i++)
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

    public void CreateGrid() {
        GeneratePool();
        _allBlocks = new Block[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Block block = GetBlockFromPool();
                block.transform.position = new Vector2(x, y);
                int randomColorID = Random.Range(0, colorCount);
                block.Init(x, y, randomColorID, blockSprites[randomColorID]);
                _allBlocks[x,y] = block;
            }
        }
        if (cameraManager != null)
        {
            cameraManager.AdjustCamera(width, height);
        }
        else
        {
            Debug.LogError("CameraManager atanmamýþ");
        }
    }

    public Block GetBlockAt(int x, int y)
    {
        if (x  < 0 || x>=width || y < 0 || y >= height)
        {
            return null;
        }
        return _allBlocks[x,y];
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

    private void RemoveBlock (Block block)
    {
        _allBlocks[block.x, block.y] = null;
        block.gameObject.SetActive(false);
        _inactivePool.Add(block);
    }

    private void RemoveMatches (List<Block> blocksToRemove) {
        foreach (Block block in blocksToRemove)
        {
            RemoveBlock(block);
        }
    }

    private void FillHoles()
    {
        for(int x = 0; x < width; x++)
        {
            int floor = 0;
            for(int y = 0; y < height; y++)
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

                        block.transform.position = new Vector2(x,floor);
                        block.name = $"Block {x},{floor}";
                    }
                    floor++;
                }
            }

            for (int y = floor; y < height; y++)
            {
                Block newBlock = GetBlockFromPool();

                newBlock.transform.position = new Vector2(x,y);

                int randomColorID = Random.Range(0, colorCount);

                newBlock.Init(x, y, randomColorID, blockSprites[randomColorID]);
                _allBlocks[x,y] = newBlock;
            }
        }
    }
}