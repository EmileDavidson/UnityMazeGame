using System.Collections.Generic;
using System.Linq;
using Toolbox.Grid;
using Toolbox.MethodExtensions;
using UnityEngine;
using UnityEngine.Events;

public abstract class MazeGenerator : MonoBehaviour
{
    [Header("Data")] 
    [SerializeReference, HideInInspector] protected Grid2D grid2D;
    [SerializeField] protected List<Cell> steps = new List<Cell>();
    [SerializeField, Min(0)] protected int currentCellIndex;

    [Header("Settings")] 
    [SerializeField, Min(1)] protected int rowAmount = 1;
    [SerializeField, Min(1)] protected int columnAmount = 1;
    [SerializeField] protected Vector3 tileScale = new Vector3(1,1,1);
    [SerializeField] protected float spacing;
    protected readonly bool wallPerCell = true; //can be [serializedField] and not readonly after function is fixed! 
    [SerializeField] protected bool performanceMode = true;
    [SerializeField, Tooltip("Used for performance mode")] protected Material tileMaterial;
    [SerializeField] protected GameObject tilesParent;
    [SerializeField] protected GameObject wallsParent;
    [SerializeField] protected GameObject tile;
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (grid2D == null || grid2D.Cells.IsEmpty()) grid2D = new Grid2D(rowAmount, columnAmount, wallsPerCell);
    }
#endif
    
    public abstract void CreateTiles();
    public abstract void CreateWalls();
    public abstract void GenerateMaze();
    public abstract List<Cell> GetNeighboursOf(Cell cell);

    public virtual void StartGeneration()
    {
        grid2D = new Grid2D(rowAmount, columnAmount, wallsPerCell);
        InitializeParents();
        CreateTiles();
        CreateWalls();
        GenerateMaze();
        CombineMeshes();
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
                else  Destroy(cell.MyGameObject); 
            }

            foreach (var wall in cell.Walls.Where(wall => wall != null))
            {
                if (destroyImmediate) DestroyImmediate(wall);
                else  Destroy(wall); 
            }
        });
        
        if (Application.isPlaying) { Destroy(tilesParent); }
        if (!Application.isPlaying) { DestroyImmediate(tilesParent); }

        steps = new List<Cell>();
        grid2D.Cells = new List<Cell>();
        currentCellIndex = 0;
    }
    
    public virtual void CombineMeshes()
    {
        if (!performanceMode) return;
        if (grid2D.Cells.IsEmpty()) return;
        
        List<MeshFilter> meshFilters = new List<MeshFilter>();

        grid2D.ForEach(cell =>
        {
            if (!cell.MyGameObject.HasAndGetComponent<MeshFilter>(out var comp)) return;
            meshFilters.Add(comp);
        });

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            if (Application.isPlaying) Destroy(meshFilters[i].gameObject);
            else { DestroyImmediate(meshFilters[i].gameObject); }
        }

        MeshFilter meshFilter = tilesParent.GetOrAddComponent<MeshFilter>();
        MeshRenderer renderer = tilesParent.GetOrAddComponent<MeshRenderer>();
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(combine);
        renderer.sharedMaterial = tileMaterial;
        tilesParent.transform.position = grid2D.Cells.First().Position;
        tilesParent.gameObject.SetActive(true);
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