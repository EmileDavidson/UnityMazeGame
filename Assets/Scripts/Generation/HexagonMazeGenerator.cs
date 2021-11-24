using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.MethodExtensions;
using UnityEngine;

public class HexagonMazeGenerator : MazeGenerator
{
    [Header("Prefabs and GameObjects")] [SerializeField]
    private GameObject tile;

    [SerializeField] private GameObject tilesParent;
    [SerializeField] private GameObject wallsParent;

    public override void CreateTiles()
    {
        if (tile == null) return;
        float tileOffset = 0;

        onStartCreatingHexagons.Invoke();

        for (int gridY = 0; gridY < rowAmount; gridY++)
        {
            for (int gridX = 0; gridX < columnAmount; gridX++)
            {
                if (gridY % 2 == 1) tileOffset = 1;
                else tileOffset = 0;

                var posX = (gridY * 1.5f * tileScaleX) + (spacing * gridY);
                var posY = 0;
                var posZ = ((gridX * 2 + tileOffset) * tileScaleZ) + (spacing * gridX);

                GameObject tileObj = Instantiate(tile, new Vector3(posX, posY, posZ), Quaternion.identity);
                tileObj.transform.parent = tilesParent.transform;
                tileObj.name = "Hexagon: " + grid.Count;

                HexagonCell hexagonCell = new HexagonCell(this, new Vector2Int(gridX, gridY));

                hexagonCell.MyGameObject = tileObj;
                tileObj.transform.localScale = new Vector3(tileScaleX, tileScaleY, tileScaleZ);
                grid.Add(hexagonCell);

                onUpdateCreatingHexagons.Invoke();
            }
        }

        onFinishCreatingHexagons.Invoke();
    }

    public override void CreateWalls()
    {
        onStartCreatingWalls.Invoke();
        for (int i = 0; i < grid.Count; i++)
        {
            //first 6 are top points
            var list = grid[i].MyGameObject.GetComponent<MeshFilter>().sharedMesh.vertices;
            List<Vector3> position = new List<Vector3>();
            foreach (var vert in list)
            {
                position.Add(vert + grid[i].MyGameObject.transform.position);
            }

            for (int j = 0; j < 6; j++)
            {
                Vector3 start = position.Get(j);
                Vector3 end = position.Get(j + 1);
                if (j == 5) end = position.Get(0);
                Vector3 center = (start + end) / 2;
                GameObject wallObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallObj.transform.position = center;
                wallObj.transform.name = "wall";
                wallObj.transform.parent = wallsParent.transform;
                wallObj.transform.localScale = new Vector3(.3f, .1f, .1f);

                var transformRotation = wallObj.transform.rotation;
                float rotationY = 0;
                if (j == 0 || j == 3) rotationY = -60;
                if (j == 1 || j == 4) rotationY = 0;
                if (j == 2 || j == 5) rotationY = 60;
                transformRotation.eulerAngles = new Vector3(0, rotationY, 0);
                wallObj.transform.localRotation = transformRotation;

                grid[i].Walls.Add(wallObj);
            }
        }

        onFinishCreatingWalls.Invoke();
    }

    public override void GenerateMaze()
    {
        onStartGeneratingMaze.Invoke();
        onUpdateGeneratingMaze.Invoke();
        onFinishGeneratingMaze.Invoke();
    }

    //todo: return list of grid in order of topleft, ropright, right, bottomright, bottomleft, left
    public override List<Cell> GetNeighboursOf(Cell cell)
    {
        return null;
    }

    private void Awake()
    {
    }
}