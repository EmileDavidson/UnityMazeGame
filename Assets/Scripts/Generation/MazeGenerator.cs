using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public abstract class MazeGenerator : MonoBehaviour
{
    [Header("Data")]
    protected List<Cell> Cells = new List<Cell>();
    
    [Header("Settings")]
    [SerializeField] protected int rowAmount;
    [SerializeField] protected int columnAmount;
    [SerializeField] protected float tileWidth;
    [SerializeField] protected float tileHeight;
    
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

    public void StartGeneration()
    {
        CreateTiles();
        CreateWalls();
        GenerateMaze();
    }

    public virtual void ResetMaze(bool destroyImmediate = false)
    {
        foreach (var cell in Cells)
        {
            if(destroyImmediate) DestroyImmediate(cell.MyGameObject);
            else{ Destroy(cell.MyGameObject); }
        }
        Cells = new List<Cell>();
    }
}


