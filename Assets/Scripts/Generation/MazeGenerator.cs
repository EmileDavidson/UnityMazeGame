
using System.Collections.Generic;
using Generation.Grid;
using Toolbox.MethodExtensions;
using UnityEngine;
using UnityEngine.Events;

public abstract class MazeGenerator : MonoBehaviour
{
    [Header("Data")] 
    [SerializeReference] protected Grid2D grid2D;

    [Header("Settings")] 
    [SerializeField, Min(1)] protected int rowAmount = 1;
    [SerializeField, Min(1)] protected int columnAmount = 1;
    [SerializeField] protected float tileScaleX = 1;
    [SerializeField] protected float tileScaleY = 1;
    [SerializeField] protected float tileScaleZ = 1;
    [SerializeField] protected float spacing;
    [SerializeField] protected bool wallPerCell;

    public UnityEvent onStartCreatingHexagons = new UnityEvent();
    public UnityEvent onUpdateCreatingHexagons = new UnityEvent();
    public UnityEvent onFinishCreatingHexagons = new UnityEvent();

    public UnityEvent onStartCreatingWalls = new UnityEvent();
    public UnityEvent onUpdateCreatingWalls = new UnityEvent();
    public UnityEvent onFinishCreatingWalls = new UnityEvent();

    public UnityEvent onStartGeneratingMaze = new UnityEvent();
    public UnityEvent onUpdateGeneratingMaze = new UnityEvent();
    public UnityEvent onFinishGeneratingMaze = new UnityEvent();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (grid2D == null || grid2D.Cells.IsEmpty()) grid2D = new Grid2D(rowAmount, columnAmount);
    }
#endif

    public abstract void CreateTiles();
    public abstract void CreateWalls();
    public abstract void GenerateMaze();
    public abstract List<Cell> GetNeighboursOf(Cell cell);

    public void StartGeneration()
    {
        grid2D = new Grid2D(rowAmount, columnAmount);
        CreateTiles();
        CreateWalls();
        GenerateMaze();
    }

    public virtual void ResetMaze(bool destroyImmediate = false)
    {
        grid2D.ForEach(cell =>
        {
            if (cell.MyGameObject != null)
            {
                if (destroyImmediate) DestroyImmediate(cell.MyGameObject);
                else
                {
                    Destroy(cell.MyGameObject);
                }
            }

            foreach (var wall in cell.Walls)
            {
                if (wall != null)
                {
                    if (destroyImmediate) DestroyImmediate(wall);
                    else
                    {
                        Destroy(wall);
                    }
                }
            }
        });

        grid2D.Cells = new List<Cell>();
    }

    public virtual int GetIndexFromGridPosition(int x, int y)
    {
        return x + columnAmount * y;
    }

    public virtual Vector2Int GetGridPositionFromIndex(int index)
    {
        return grid2D[index].GridPosition;
    }

    public abstract GameObject InstantiateWall(int j, Vector3 center);
    public abstract GameObject InstantiateTile(Vector3 center, Cell cell);
}