using System;
using System.Collections.Generic;
using Toolbox.MethodExtensions;
using UnityEngine;

public class HexagonMazeGenerator : MazeGenerator
{
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject tilesParent;
    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float spacing;
    public override void CreateTiles()
    {
        if (tile == null) return;
        float tileOffset = 0;   
        
        onStartCreatingHexagons.Invoke();

        for (int i = 0; i < rowAmount; i++)
        {
            for (int j = 0; j < columnAmount; j++)
            {
                if (i % 2 == 1) tileOffset = 1;
                else tileOffset = 0;

                GameObject tileObj = Instantiate(tile, new Vector3((i * 1.5f) + (spacing * i), 0, (j * 2 + tileOffset) + (spacing * j)), Quaternion.identity);
                tileObj.transform.parent = tilesParent.transform;

                HexagonCell hexagonCell = new HexagonCell();
                hexagonCell.MyGameObject = tileObj;
                Cells.Add(hexagonCell);

                onUpdateCreatingHexagons.Invoke();
            }
        }
        
        onFinishCreatingHexagons.Invoke();
    }

    public override void CreateWalls()
    {
        onStartCreatingWalls.Invoke();
        onUpdateCreatingWalls.Invoke();
        onFinishCreatingWalls.Invoke();
    }

    public override void GenerateMaze()
    {
        onStartGeneratingMaze.Invoke();
        onUpdateGeneratingMaze.Invoke();
        onFinishGeneratingMaze.Invoke();
    }
}
