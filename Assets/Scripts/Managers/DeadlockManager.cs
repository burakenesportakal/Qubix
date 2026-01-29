using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class DeadlockManager : MonoBehaviour
{
    public bool isDeadlocked(Block[,] allBlocks, int width, int height) 
    {
        for(int x = 0;  x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Block current = allBlocks[x, y];
                if (current == null) continue;
                if (x < width - 1 && allBlocks[x + 1, y] != null && allBlocks[x + 1, y].colorID == current.colorID) return false;
                if (y < height - 1 && allBlocks[x, y + 1] != null && allBlocks[x, y + 1].colorID == current.colorID) return false;
            }
        } 
        return true;
    }

    public IEnumerator ShuffleBoardProcces(Block[,] allBlocks, int width, int height, BlockSet[] blockSets)
    {

        float animDuration = 0.4f;
        float timer = 0;

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float ratio = Mathf.Lerp(1f, 0f, timer / animDuration);
            foreach (Block block in allBlocks)
            {
                if (block != null) block.transform.localScale = block.OriginalScale * ratio;
            }
            yield return null;
        }

        List<int> colorsOnBoard = new List<int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBlocks[x, y] != null)
                    colorsOnBoard.Add(allBlocks[x, y].colorID);
            }
        }
        
        bool canSolveWithShuffle = false;
        var colorCounts = colorsOnBoard.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());

        if (colorCounts.Any(c => c.Value >= 2))
        {
            canSolveWithShuffle = true;
        }

        int safeX = Random.Range(0, width - 1);
        int safeY = Random.Range(0, height);
        List<int> finalBlockList = new List<int>();
        int guaranteedColorID = 0;

        if (canSolveWithShuffle)
        {
            guaranteedColorID = colorCounts.First(c=> c.Value >= 2).Key;

            colorsOnBoard.Remove(guaranteedColorID);
            colorsOnBoard.Remove(guaranteedColorID);

            finalBlockList = colorsOnBoard;
            ShuffleList(finalBlockList);
        }
        else
        {
            finalBlockList.Clear();
            int totalBlockNeeded = (width*height) - 2;

            guaranteedColorID = Random.Range(0, blockSets.Length);

            for (int i = 0; i < totalBlockNeeded; i++)
            {
                finalBlockList.Add(Random.Range(0, blockSets.Length));
            }
        }


        int listIndex = 0;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Block block = allBlocks[x, y];
                if (block == null) continue;

                int newID;

                if ((x == safeX && y == safeY) || (x == safeX + 1 && y == safeY))
                {
                    newID = guaranteedColorID;
                }
                else
                {
                    if (listIndex < finalBlockList.Count)
                    {
                        newID = finalBlockList[listIndex];
                        listIndex++;
                    }
                    else newID = 0;
                }

                block.Init(x, y, newID, blockSets[newID]);
                block.transform.localScale = Vector3.zero;
            }
        }

        yield return new WaitForSeconds(0.1f);

        timer = 0;
        while(timer< animDuration)
        {
            timer += Time.deltaTime;
            float ratio = Mathf.Lerp(0f, 1f, timer / animDuration);

            foreach(Block block in allBlocks)
            {
                if (block != null) block.transform.localScale = block.OriginalScale * ratio;
            }
            yield return null;
        }

        foreach (Block block in allBlocks)
        {
            if (block != null) block.transform.localScale = block.OriginalScale;
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
