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

                InstantiateTile(new Vector3(posX, posY, posZ), gridX, gridY);
                onUpdateCreatingHexagons.Invoke();
            }
        }

        onFinishCreatingHexagons.Invoke();
    }
    
    public override GameObject InstantiateTile(Vector3 center, int gridX, int gridY)
    {
        GameObject tileObj = Instantiate(tile, center, Quaternion.identity);
        tileObj.transform.parent = tilesParent.transform;
        tileObj.name = "Hexagon: " + grid.Count;

        HexagonCell hexagonCell = new HexagonCell(this, new Vector2Int(gridX, gridY));

        hexagonCell.MyGameObject = tileObj;
        tileObj.transform.localScale = new Vector3(tileScaleX, tileScaleY, tileScaleZ);
        grid.Add(hexagonCell);
        return tileObj;
    }

    public override void IsBorderCell()
    {
        //todo: calculate if the cell is on border of maze
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

            if (wallPerCell)
            {
                for (int j = 0; j < 6; j++)
                {
                    Vector3 start = position.Get(j);
                    Vector3 end = position.Get(j + 1);
                    if (j == 5) end = position.Get(0);
                    Vector3 center = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2 * tileScaleY, (start.z + end.z) / 2);

                    GameObject wallObj = InstantiateWall(j, center);
                    grid[i].Walls.Add(wallObj);
                }
            }
            else
            {
                //todo: calculate walls for each cell and also for its neighbours so there is only one wall per connection 
            }
        }

        onFinishCreatingWalls.Invoke();
    }
    
    public override GameObject InstantiateWall(int j, Vector3 center)
    {
        GameObject wallObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallObj.transform.position = center;
        wallObj.transform.name = "wall";
        wallObj.transform.parent = wallsParent.transform;
        wallObj.transform.localScale = new Vector3(.3f, .1f, .1f);

        float rotationY = 0;
        if (j == 0 || j == 3) rotationY = -60;
        if (j == 1 || j == 4) rotationY = 0;
        if (j == 2 || j == 5) rotationY = 60;
        
        var transformRotation = wallObj.transform.rotation;
        transformRotation.eulerAngles = new Vector3(0, rotationY, 0);
        wallObj.transform.localRotation = transformRotation;
        
        return wallObj;
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
        int topLeft = GetIndexFromGridPosition(cell.GridPosition.x , cell.GridPosition.y + 1);
        int topRight = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y);
        int right = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y);
        int bottomRight = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y);
        int bottomLeft = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y);
        int left = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y);
        
        return null;
    }
    
    // private void Awake()
    // {
    //     Cell cell = grid[10];
    //     cell.MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(100, 100, 100, 1);
    //     int topLeft = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);
    //     int topRight = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y - 1);
    //     int right = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y);
    //     int bottomRight = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y + 1);
    //     int bottomLeft = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y + 1);
    //     int left = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y);
    //     
    //     Debug.Log(cell.GridPosition);
    //     if (right <= 0 || right / columnAmount > rowAmount) right = grid.Count - 1;
    //     if (left == 0 || left > rowAmount) left = grid.Count - 1;
    //
    //     Debug.Log(topLeft);
    //     Debug.Log(topRight);
    //     Debug.Log(right);
    //     Debug.Log(bottomRight);
    //     Debug.Log(bottomLeft);
    //     Debug.Log(left);
    //
    //     var debugging = true;
    //     if (debugging)
    //     {
    //        if(grid[topLeft] != null) grid[topLeft].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    //        if(grid[topRight] != null) grid[topRight].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    //        if(grid[right] != null) grid[right].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    //        if(grid[bottomLeft] != null) grid[bottomLeft].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    //        if(grid[bottomRight] != null) grid[bottomRight].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    //        if (grid[left] != null) grid[left].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0, 0);
    //     }
    // }


    public int TEST = 0;
    private void Awake()
    {
        Cell cell = grid[TEST];
        cell.MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(.3f, .3f, .3f, 1);

        int index = GetBottomRightNeighbourIndex(cell);
        Debug.Log(index);
        if(grid.ContainsSlot(index)) grid[index].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(1,1,1);
    }

    public int GetTopLeftNeighbourIndex(Cell cell)
    {
        int topLeftIndex = -1;
        if(cell.GridPosition.y % 2 == 0) topLeftIndex = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y - 1);
        if(cell.GridPosition.y % 2 != 0) topLeftIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);
        if ((GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y) % columnAmount == 0 && cell.GridPosition.y % 2 == 0)) topLeftIndex = -1;
        return (topLeftIndex);
    }
    public int GetTopRightNeighbourIndex(Cell cell)
    {
        int topRightIndex = -1;
        if(cell.GridPosition.y % 2 == 0) topRightIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);
        if(cell.GridPosition.y % 2 != 0) topRightIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);
        if ((GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y) % columnAmount == 0 && cell.GridPosition.y % 2 == 0)) topRightIndex = -1;
        return topRightIndex;
    }

    public int GetRightNeighbour(Cell cell)
    {
        int rightIndex = -1;
        if((cell.GridPosition.x + 1) % rowAmount != 0) rightIndex = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y);
        return rightIndex;
    }

    public int GetBottomRightNeighbourIndex(Cell cell)
    {
        int bottomRightIndex = -1;
        if(cell.GridPosition.y % 2 == 0) bottomRightIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y + 1);
        if(cell.GridPosition.y % 2 != 0) bottomRightIndex = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y + 1);
        return (bottomRightIndex);
    }
    
    
    //todo: NOT WORKING YET!
    public int GetBottomLeftNeighbourIndex(Cell cell)
    {
        int bottomRightIndex = -1;
        if(cell.GridPosition.y % 2 == 0) bottomRightIndex = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y + 1);
        if(cell.GridPosition.y % 2 != 0) bottomRightIndex = GetIndexFromGridPosition(cell.GridPosition.x , cell.GridPosition.y + 1);
        return (bottomRightIndex);
    }
}

