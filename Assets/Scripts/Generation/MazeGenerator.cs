using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public abstract class MazeGenerator : MonoBehaviour
{
    [Header("Data")]
    [SerializeReference] protected List<Cell> grid = new List<Cell>();
    
    [Header("Settings")]
    [SerializeField] protected int rowAmount;
    [SerializeField] protected int columnAmount;
    [SerializeField] protected float tileScaleX;
    [SerializeField] protected float tileScaleY;
    [SerializeField] protected float tileScaleZ;
    [SerializeField] protected float spacing;
    [SerializeField] protected bool wallPerCell; 
    
    public UnityEvent onStartCreatingHexagons = new UnityEvent();
    public UnityEvent onUpdateCreatingHexagons = new UnityEvent();
    public UnityEvent onFinishCreatingHexagons = new UnityEvent();
    
    public UnityEvent onStartCreatingWalls = new UnityEvent();
    public UnityEvent onUpdateCreatingWalls  = new UnityEvent();
    public UnityEvent onFinishCreatingWalls  = new UnityEvent();
    
    public UnityEvent onStartGeneratingMaze = new UnityEvent();
    public UnityEvent onUpdateGeneratingMaze  = new UnityEvent();
    public UnityEvent onFinishGeneratingMaze  = new UnityEvent();

    public abstract void CreateTiles();
    public abstract void CreateWalls();
    public abstract void GenerateMaze();
    public abstract List<Cell> GetNeighboursOf(Cell cell);

    public void StartGeneration()
    {
        CreateTiles();
        CreateWalls();
        GenerateMaze();
    }

    public virtual void ResetMaze(bool destroyImmediate = false)
    {
        foreach (var cell in grid)
        {
            if (cell.MyGameObject != null)
            {
                if (destroyImmediate) DestroyImmediate(cell.MyGameObject);
                else { Destroy(cell.MyGameObject); }
            }

            foreach (var wall in cell.Walls){
                if (wall != null)
                {
                    if (destroyImmediate) DestroyImmediate(wall);
                    else { Destroy(wall); }
                }
            }
        }
        grid = new List<Cell>();
    }
    
    public virtual int GetIndexFromGridPosition(int x, int y)
    {
        return x + columnAmount * y;
    }

    public virtual Vector2Int GetGridPositionFromIndex(int index)
    {
        return grid[index].GridPosition;
    }

    public abstract GameObject InstantiateWall(int j, Vector3 center);
    public abstract GameObject InstantiateTile(Vector3 center, int gridX, int gridY);

    public abstract void IsBorderCell();
}


