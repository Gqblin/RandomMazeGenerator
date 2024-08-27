
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMazeGenerator : MonoBehaviour
{
    public static RandomMazeGenerator instance;

    [SerializeField] private MazeCell mazeCell;
    [SerializeField] private GameObject mazeParent;

    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeHeight;

    private MazeCell[,] mazeGrid;

    private void Awake()
    {
        //creating an instance of itself
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        //creates all the cells in a 2d array at the start. this might reduce lag when generating mazes later on.
        mazeGrid = new MazeCell[250, 250];

        //spawns all cells in 250x250 array. the cell prefab is standard inactive
        for (int x = 0; x < 250; x++)
        {
            for (int y = 0; y < 250; y++)
            {
                mazeGrid[x, y] = Instantiate(mazeCell, new Vector2(x, y), Quaternion.identity);
                mazeGrid[x, y].SetLocationIndex(x, y);
                mazeGrid[x, y].transform.parent = mazeParent.transform;                             //makes all cells a child of the mazeparent so the maze can easily be scaled.
            }
        }

        //scales the maze so it fits on the screen.
        float scale = 10 / (float)250;
        mazeParent.transform.localScale = new Vector3(scale, scale, 1);
        mazeParent.transform.position = new Vector3(scale / 2f, scale / 2f, 0);
    }

    public void CreateMazeGrid(int width, int height, bool showGeneration)
    {
        StopAllCoroutines();
        ResetNeededMazeCells(width, height);

        //sets all needed cells active
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mazeGrid[x, y].gameObject.SetActive(true);
            }
        }

        //checks if the new maze is smaller than the last one. if so, sets all the active cells that are not needed(the ones above the new width or height value) inactive.
        if (width < mazeWidth)
        {
            for (int x = width; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    mazeGrid[x, y].gameObject.SetActive(false);
                }
            }
        }
        if (height < mazeHeight)
        {
            for (int y = height; y < mazeHeight; y++)
            {
                for (int x = 0; x < mazeWidth; x++)
                {
                    mazeGrid[x, y].gameObject.SetActive(false);
                }
            }
        }

        mazeWidth = width;
        mazeHeight = height;

        float scale = CalculateMazeScale(mazeWidth, mazeHeight);
        mazeParent.transform.localScale = new Vector3(scale, scale, 1);             //sets scale of maze so it is always 10mx10m big
        mazeParent.transform.position = new Vector3(scale / 2f, scale / 2f, 0);     //adjusts location of maze a bit depending on scale/size so it is fully visible on screen

        //if showGeneration is true, start coroutine so you can see the maze being made. else, calls the function that instantly generates the maze
        if (showGeneration)
        {
            StartCoroutine(GenerateRandomMaze(null, mazeGrid[0, 0]));
        }
        else
        {
            GenerateRandomMazeInstant(null, mazeGrid[0, 0]);
        }
        
    }

    private IEnumerator GenerateRandomMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.MarkAsVisited();
        RemoveWallsBetweenCells(previousCell, currentCell);
        yield return new WaitForSeconds(0.025f);
        MazeCell nextCell;

        do 
        {
            nextCell = GetRandomUnvisitedNeighbour(currentCell);
            if (nextCell != null)
            {
                yield return GenerateRandomMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private void GenerateRandomMazeInstant(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.MarkAsVisited();
        RemoveWallsBetweenCells(previousCell, currentCell);
        MazeCell nextCell;

        do
        {
            nextCell = GetRandomUnvisitedNeighbour(currentCell);
            if (nextCell != null)
            {
                GenerateRandomMazeInstant(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    //returns a random unvisited neighbour cell from the current cell.
    private MazeCell GetRandomUnvisitedNeighbour(MazeCell currentCell)
    {
        int x = currentCell.cellLocationIndex.x;
        int y = currentCell.cellLocationIndex.y;

        //unvisited neighbours are stored here.
        List<MazeCell> UnvisitedCells = new List<MazeCell>();
        MazeCell nextCell = null;

        //checks if there is an neightbour, then checks if it is active and if its already visited. if so, adds it to the possible next cells list
        if (x + 1 < mazeWidth)
        {
            if (mazeGrid[x + 1, y].gameObject.activeSelf &&!mazeGrid[x + 1, y].visited)
            {
                UnvisitedCells.Add(mazeGrid[x + 1, y]);
            }
        }

        if (x - 1 >= 0)
        {
            if (mazeGrid[x - 1, y].gameObject.activeSelf && !mazeGrid[x - 1, y].visited)
            {
                UnvisitedCells.Add(mazeGrid[x - 1, y]);
            }
        }

        if (y + 1 < mazeHeight)
        {
            if (mazeGrid[x, y + 1].gameObject.activeSelf && !mazeGrid[x, y + 1].visited)
            {
                UnvisitedCells.Add(mazeGrid[x, y + 1]);
            }
        }

        if (y - 1 >= 0)
        {
            if (mazeGrid[x, y - 1].gameObject.activeSelf && !mazeGrid[x, y - 1].visited)
            {
                UnvisitedCells.Add(mazeGrid[x, y - 1]);
            }
        }

        if (UnvisitedCells.Count > 0)
        {
            nextCell = UnvisitedCells[Random.Range(0, UnvisitedCells.Count)];
        }

        return nextCell;
    }

    //removes the walls between the previous cell and current cell
    private void RemoveWallsBetweenCells(MazeCell previousCell, MazeCell currentCell)
    {
        //does not remove any walls when checking first cell
        if (previousCell == null)
        {
            return;
        }

        //compares location of the previous and current cells and desides wich walls need to be removed
        if (previousCell.cellLocationIndex.x < currentCell.cellLocationIndex.x)
        {
            previousCell.RemoveRightWall();
            currentCell.RemoveLeftWall();
            return;
        }

        if (previousCell.cellLocationIndex.x > currentCell.cellLocationIndex.x)
        {
            previousCell.RemoveLeftWall();
            currentCell.RemoveRightWall();
            return;
        }

        if (previousCell.cellLocationIndex.y < currentCell.cellLocationIndex.y)
        {
            previousCell.RemoveTopWall();
            currentCell.RemoveBottomWall();
            return;
        }

        if (previousCell.cellLocationIndex.y > currentCell.cellLocationIndex.y)
        {
            previousCell.RemoveBottomWall();
            currentCell.RemoveTopWall();
            return;
        }
    }

    //calculates new scale for the maze by deviding 10 by the new size so the maze is always 10 big.
    private float CalculateMazeScale(int width, int height)
    {
        float scale;
        if (width > height)
        {
            scale = 10 / (float)width;
        }
        else
        {
            scale = 10 / (float)height;
        }
        return scale;
    }

    //resets all the cells. all cellWalls will be set to active and are marked as unvisited
    private void ResetNeededMazeCells(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mazeGrid[x, y].ResetCell();
            }
        }
    }
}