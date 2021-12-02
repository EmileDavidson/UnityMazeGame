using System.Collections.Generic;
using Toolbox.Grid;
using Toolbox.MethodExtensions;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(MazeGenerator))]
public class MazeItemSpawner : MonoBehaviour
{
    [Header("Main Settings")] [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField] private bool spawnOnStart = false;
    [SerializeField] private bool spawnAfterGeneration = true;

    [Header("End point spawning")] [SerializeField]
    private bool spawnOnEachDeadEnd = true;

    [SerializeField] private int maxEndPointSpawnAmount = int.MaxValue;

    [Header("Random Spawning")] [SerializeField]
    private int maxRandomPointSpawnAmount = int.MaxValue;

    [SerializeField] [Min(0)] private int spawnChange = 10;

    [Header("Data")] [SerializeField] private List<GameObject> spawnables = new List<GameObject>();
    private readonly List<GameObject> _spawnedItems = new List<GameObject>();
    private readonly List<int> spawnedIndex = new List<int>();
    [SerializeField] private GameObject itemParent;


    private void Awake()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        if (spawnOnStart) SpawnItems();
        if (itemParent == null) CreateItemParent();
    }

    private void OnEnable()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        if (mazeGenerator == null)
        {
            Debug.LogWarning("This should not be possible but if it happens we are missing the MazeGenerator script!");
            return;
        }

        if (spawnAfterGeneration) mazeGenerator.onFinishGeneratingMaze.AddListener(SpawnItems);
        mazeGenerator.onResetMaze.AddListener(Reset);
    }

    private void CreateItemParent()
    {
        itemParent = new GameObject
        {
            transform = { parent = this.transform },
            name = "ItemParent"
        };
    }

    private void Reset()
    {
        if (!Application.isPlaying) DestroyImmediate(itemParent);
        else Destroy(itemParent);
        CreateItemParent();

        _spawnedItems.Clear();
        spawnedIndex.Clear();
    }


    private void SpawnItems()
    {
        Reset();
        if (itemParent == null) CreateItemParent();
        if (mazeGenerator == null || spawnables.IsEmpty()) return;
        if (spawnOnEachDeadEnd) SpawnOnEachDeadEnd();
        SpawnRandom();
    }

    private void SpawnRandom()
    {
        int totalSpawnAmount = 0;
        List<GameObject> objects = new List<GameObject>();

        mazeGenerator.Grid2D.ForEach(cell =>
        {
            if (spawnedIndex.Contains(cell.Index)) return;
            int random = Random.Range(0, 101);
            if (random >= spawnChange) return;

            GameObject spawnedItem = Instantiate(spawnables[Random.Range(0, spawnables.Count - 1)],
                itemParent.transform, true);
            spawnedItem.transform.position = cell.Position.AddY(.5f);
            objects.Add(spawnedItem);
            spawnedIndex.Add(cell.Index);
            totalSpawnAmount++;
        });

        if (totalSpawnAmount <= maxRandomPointSpawnAmount)
        {
            _spawnedItems.AddList(objects);
            return;
        }

        int removeAmount = totalSpawnAmount - maxRandomPointSpawnAmount;
        objects = RemoveRandom(removeAmount, objects);

        _spawnedItems.AddList(objects);
    }

    private void SpawnOnEachDeadEnd()
    {
        int totalSpawnAmount = 0;
        List<GameObject> objects = new List<GameObject>();
        mazeGenerator.Grid2D.ForEach(cell =>
        {
            int openNeighbourAmount = CalculateOpenNeighbourAmount(cell);
            if (openNeighbourAmount != 1) return;
            GameObject spawnedItem = Instantiate(spawnables[Random.Range(0, spawnables.Count - 1)],
                itemParent.transform, true);
            spawnedItem.transform.position = cell.Position.AddY(.5f);
            objects.Add(spawnedItem);
            spawnedIndex.Add(cell.Index);
            totalSpawnAmount++;
        });

        if (totalSpawnAmount <= maxEndPointSpawnAmount)
        {
            _spawnedItems.AddList(objects);
            return;
        }

        int removeAmount = totalSpawnAmount - maxEndPointSpawnAmount;

        objects = RemoveRandom(removeAmount, objects);

        _spawnedItems.AddList(objects);
    }

    public List<GameObject> RemoveRandom(int removeAmount, List<GameObject> objects)
    {
        for (int i = 0; i < removeAmount; i++)
        {
            if (objects.IsEmpty()) continue;
            int random = Random.Range(0, objects.Count - 1);
            if (!Application.isPlaying) DestroyImmediate(objects[random]);
            else Destroy(objects[random]);
            objects.RemoveAt(random);
            spawnedIndex.Remove(random);
        }

        return objects;
    }

    public int CalculateOpenNeighbourAmount(Cell cell)
    {
        var neighbours = mazeGenerator.GetNeighboursOf(cell);
        int openNeighbours = 0;
        foreach (var neighbourCell in neighbours)
        {
            if (neighbourCell == null) continue;
            int wallIndex = mazeGenerator.GetWallIndexFromTo(cell, neighbourCell);
            if (cell.Walls.ContainsSlot(wallIndex) && cell.Walls[wallIndex]) continue;
            openNeighbours++;
        }

        return openNeighbours;
    }
}