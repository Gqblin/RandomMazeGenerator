using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject topWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject bottomWall;
    [SerializeField] private GameObject leftWall;

    public bool visited { get; private set; }

    //index that contains this cell's location in de mazegrid array
    public Vector2Int cellLocationIndex;

    public void MarkAsVisited()
    {
        visited = true;
    }

    public void SetLocationIndex(int x, int y)
    {
        cellLocationIndex.x = x;
        cellLocationIndex.y = y;
    }

    //removes the walls
    public void RemoveTopWall()
    {
        topWall.SetActive(false);
    }

    public void RemoveRightWall()
    {
        rightWall.SetActive(false);
    }

    public void RemoveBottomWall()
    {
        bottomWall.SetActive(false);
    }

    public void RemoveLeftWall()
    {
        leftWall.SetActive(false);
    }

    //resets the cell by setting all walls active and marking it as not visited.
    public void ResetCell()
    {
        visited = false;
        topWall.SetActive(true);
        rightWall.SetActive(true);
        bottomWall.SetActive(true);
        leftWall.SetActive(true);
    }
}
