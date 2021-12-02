using System.Collections.Generic;
using System.Linq;
using Toolbox.Grid;
using Toolbox.MethodExtensions;
using Toolbox.Utility;
using UnityEngine;
using UnityEngine.Events;

public abstract class MazeGenerator : MonoBehaviour
{
    [Header("Debugging")] [SerializeField] protected bool debugging = true;
    [SerializeField] protected int debuggingIndex = 0;

    [Header("Data")] 
    [SerializeReference, HideInInspector] protected Grid2D grid2D;
    public Grid2D Grid2D => grid2D;
    [HideInInspector] protected List<Cell> Steps = new List<Cell>();
    [Min(0)] protected int CurrentCellIndex;

    [Header("Settings")]
    [SerializeField, Min(1)] protected int rowAmount = 1;
    [SerializeField, Min(1)] protected int columnAmount = 1;
    [SerializeField] protected Vector3 tileScale = new Vector3(1, 1, 1);
    [SerializeField] protected float spacing;

    [Header("Performance Settings")] 
    [SerializeField]protected bool wallPerCell = true;
    [SerializeField] protected bool performanceMode = true;

    [Header("GameObject and GameObjects Settings")] [SerializeField, Tooltip("Used for performance mode")]
    protected Material tileMaterial;
    [SerializeField, Tooltip("Used for performance mode")]
    protected Material wallMaterial;

    [SerializeField] protected float wallHeight = 3;

    [SerializeField] protected GameObject tilesParent;
    [SerializeField] protected GameObject wallsParent;
    [SerializeField] protected GameObject cellTile;
    [SerializeField] protected int wallsPerCell = 6;

    public UnityEvent onStartCreatingHexagons = new UnityEvent();
    public UnityEvent onUpdateCreatingHexagons = new UnityEvent();
    public UnityEvent onFinishCreatingHexagons = new UnityEvent();

    public UnityEvent onStartCreatingWalls = new UnityEvent();
    public UnityEvent onUpdateCreatingWalls = new UnityEvent();
    public UnityEvent onFinishCreatingWalls = new UnityEvent();

    public UnityEvent onStartGeneratingMaze = new UnityEvent();
    public UnityEvent onUpdateGeneratingMaze = new UnityEvent();
    public UnityEvent onFinishGeneratingMaze = new UnityEvent();

    public UnityEvent onResetMaze = new UnityEvent();


    public abstract void CreateTiles();
    public abstract void CreateWalls();
    public abstract void GenerateMaze();
    public abstract List<Cell> GetNeighboursOf(Cell cell);

    public virtual void StartGeneration()
    {
        grid2D = new Grid2D(rowAmount, columnAmount, wallsPerCell);
        if (cellTile == null)
        {
            Debug.LogError("You have to select a tile for the maze");
            return;
        }

        InitializeParents();
        CreateTiles();
        CreateWalls();
        GenerateMaze();

        if (performanceMode)
        {
            CombineTileMeshes();
            CombineWallMeshes();
        }
    }

    public void InitializeParents()
    {
        if (tilesParent == null || tilesParent == default)
        {
            tilesParent = new GameObject
            {
                transform = { parent = this.transform },
                name = "TilesParent"
            };
        }

        if (wallsParent == null || tilesParent == default)
        {
            wallsParent = new GameObject
            {
                transform = { parent = this.transform },
                name = "WallsParent"
            };
        }
    }

    public virtual void ResetMaze(bool destroyImmediate = false)
    {
        onResetMaze.Invoke();
        grid2D.ForEach(cell =>
        {
            if (cell.MyGameObject != null)
            {
                if (destroyImmediate) DestroyImmediate(cell.MyGameObject);
                else Destroy(cell.MyGameObject);
            }

            foreach (var wall in cell.WallsObjects.Where(wall => wall != null))
            {
                if (destroyImmediate) DestroyImmediate(wall);
                else Destroy(wall);
            }
        });

        if (Application.isPlaying)
        {
            Destroy(tilesParent);
        }

        if (!Application.isPlaying)
        {
            DestroyImmediate(tilesParent);
        }

        if (Application.isPlaying)
        {
            Destroy(wallsParent);
        }

        if (!Application.isPlaying)
        {
            DestroyImmediate(wallsParent);
        }

        Steps = new List<Cell>();
        grid2D.Cells = new List<Cell>();
        CurrentCellIndex = 0;
    }

    public virtual void CombineTileMeshes()
    {
        if (!performanceMode) return;
        List<MeshFilter> filters = (
            from cell in grid2D.Cells.Where(linqCell =>
                linqCell != null && linqCell.MyGameObject != null && linqCell.MyGameObject.HasComponent<MeshFilter>())
            select cell.MyGameObject.GetComponent<MeshFilter>()
        ).ToList();

        MyMeshUtility.CombineMeshes(filters, tilesParent, tileMaterial);
        tilesParent.transform.position = grid2D[0].Position;
    }

    public void CombineWallMeshes()
    {
        if (!performanceMode) return;
        List<MeshFilter> filters = (
            from cell in grid2D.Cells.Where(linqCell => linqCell is { WallsObjects: { } })
            from wall in cell.WallsObjects
            where wall != null && wall.HasComponent<MeshFilter>()
            select wall.GetComponent<MeshFilter>()).ToList();

        MyMeshUtility.CombineMeshes(filters, wallsParent, wallMaterial);
        tilesParent.transform.position = grid2D[0].Position;
    }

    public virtual int GetIndexFromGridPosition(int x, int y)
    {
        return x + columnAmount * y;
    }

    public virtual Vector2Int GetGridPositionFromIndex(int index)
    {
        return grid2D[index].GridPosition;
    }

    public abstract GameObject InstantiateWall(Vector3 center, Vector3 rotation);
    public abstract GameObject InstantiateTile(Vector3 center, Cell cell);
}