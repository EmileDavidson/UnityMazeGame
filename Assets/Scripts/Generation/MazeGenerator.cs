
using System.Collections.Generic;
using System.Linq;
using Toolbox.Grid;
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
    [SerializeField] protected bool performanceMode = true;
    [SerializeField, Tooltip("Used for performance mode")] protected Material tileMaterial;
    [SerializeField] protected GameObject tilesParent;
    [SerializeField] protected GameObject wallsParent;
    
    public UnityEvent onStartCreatingHexagons = new UnityEvent();
    public UnityEvent onUpdateCreatingHexagons = new UnityEvent();
    public UnityEvent onFinishCreatingHexagons = new UnityEvent();

    public UnityEvent onStartCreatingWalls = new UnityEvent();
    public UnityEvent onUpdateCreatingWalls = new UnityEvent();
    public UnityEvent onFinishCreatingWalls = new UnityEvent();

    public UnityEvent onStartGeneratingMaze = new UnityEvent();
    public UnityEvent onUpdateGeneratingMaze = new UnityEvent();
    public UnityEvent onFinishGeneratingMaze = new UnityEvent();

    [SerializeReference] public UnityEvent resetMaze = new UnityEvent();

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
        resetMaze.Invoke();
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
        
        if (Application.isPlaying) { Destroy(tilesParent); }
        if (!Application.isPlaying) { DestroyImmediate(tilesParent); }
        InitializeParents();

        grid2D.Cells = new List<Cell>();
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

    public abstract GameObject InstantiateWall(int j, Vector3 center);
    public abstract GameObject InstantiateTile(Vector3 center, Cell cell);
}