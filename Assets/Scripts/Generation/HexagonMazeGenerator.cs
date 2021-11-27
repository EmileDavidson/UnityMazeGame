using System.Collections.Generic;
using System.Linq;
using Toolbox.Grid;
using Toolbox.MethodExtensions;
using UnityEngine;

public class HexagonMazeGenerator : MazeGenerator
{
    [Header("Prefabs and GameObjects")] [SerializeField]
    private GameObject tile;
    



    public override void CreateTiles()
    {
        if (tile == null) return;
        float tileOffset = 0;

        onStartCreatingHexagons.Invoke();

        grid2D.ForEach((cell =>
        {
            if (cell.GridPosition.y % 2 == 1) tileOffset = 1;
            else tileOffset = 0;

            var posX = (cell.GridPosition.y * 1.5f * tileScaleX) + (spacing * cell.GridPosition.y);
            var posY = 0;
            var posZ = ((cell.GridPosition.x * 2 + tileOffset) * tileScaleZ) + (spacing * cell.GridPosition.x);

            GameObject tileObj = InstantiateTile(new Vector3(posX, posY, posZ), cell);
            onUpdateCreatingHexagons.Invoke();
        }));

        onFinishCreatingHexagons.Invoke();
    }

    public override GameObject InstantiateTile(Vector3 center, Cell cell)
    {
        GameObject tileObj = Instantiate(tile, center, Quaternion.identity);
        tileObj.transform.parent = tilesParent.transform;
        tileObj.name = "Hexagon: " + cell.Index;
        tileObj.transform.localScale = new Vector3(tileScaleX, tileScaleY, tileScaleZ);

        cell.MyGameObject = tileObj;
        cell.Position = tileObj.transform.position;
        return tileObj;
    }

    public override void CreateWalls()
    {
        onStartCreatingWalls.Invoke();

        grid2D.ForEach(cell =>
        {
            var list = cell.MyGameObject.GetComponent<MeshFilter>().sharedMesh.vertices;
            List<Vector3> positions = new List<Vector3>();
            foreach (var vert in list)
            {
                positions.Add(vert + cell.MyGameObject.transform.position);
            }

            if (wallPerCell) CreateWallsPerCell(cell, positions);
            else CreateWallsAroundCells(cell);
        });

        onFinishCreatingWalls.Invoke();
    }

    public void CreateWallsPerCell(Cell cell, List<Vector3> positions)
    {
        if (wallPerCell)
        {
            for (int j = 0; j < 6; j++)
            {
                Vector3 start = positions.Get(j);
                Vector3 end = positions.Get(j + 1);
                if (j == 5) end = positions.Get(0);
                Vector3 center = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2 * tileScaleY,
                    (start.z + end.z) / 2);

                GameObject wallObj = InstantiateWall(j, center);
                grid2D[cell.Index].Walls.Add(wallObj);
            }
        }
    }

    public void CreateWallsAroundCells(Cell cell)
    {
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

    //todo: return list of grid2D in order of topleft, ropright, right, bottomright, bottomleft, left
    public override List<Cell> GetNeighboursOf(Cell cell)
    {
        int topLeftIndex = GetTopLeftNeighbourIndex(cell);
        int topRightIndex = GetTopRightNeighbourIndex(cell);
        int rightIndex = GetRightNeighbourIndex(cell);
        int bottomRightIndex = GetBottomRightNeighbourIndex(cell);
        int bottomLeftIndex = GetBottomLeftNeighbourIndex(cell);
        int leftIndex = GetLeftNeighbourIndex(cell);

        return new List<Cell>()
        {
            grid2D[topLeftIndex], grid2D[topRightIndex], grid2D[rightIndex], grid2D[bottomRightIndex],
            grid2D[bottomLeftIndex], grid2D[leftIndex]
        };
    }

    public int TEST = 0;

    private void Update()
    {
        grid2D.ForEach(cell =>
        {
            if (cell.MyGameObject.HasAndGetComponent<MeshRenderer>(out var comp))
            {
                comp.material.color = new Color(0, 0, 0, 1);
            }
        });

        if (!grid2D.Cells.ContainsSlot(TEST)) return;

        Cell cell = grid2D[TEST];
        if (cell.MyGameObject == null) return;
        cell.MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);

        int topLeftIndex = GetTopLeftNeighbourIndex(cell);
        int topRightIndex = GetTopRightNeighbourIndex(cell);
        int rightIndex = GetRightNeighbourIndex(cell);
        int bottomRightIndex = GetBottomRightNeighbourIndex(cell);
        int bottomLeftIndex = GetBottomLeftNeighbourIndex(cell);
        int leftIndex = GetLeftNeighbourIndex(cell);

        if (grid2D.Cells.ContainsSlot(topLeftIndex))
            grid2D[topLeftIndex].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(.5f, 0, 0, 1);
        if (grid2D.Cells.ContainsSlot(topRightIndex))
            grid2D[topRightIndex].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 1);
        if (grid2D.Cells.ContainsSlot(rightIndex))
            grid2D[rightIndex].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, .5f, 0, 1);
        if (grid2D.Cells.ContainsSlot(bottomRightIndex))
            grid2D[bottomRightIndex].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, 1);
        if (grid2D.Cells.ContainsSlot(bottomLeftIndex))
            grid2D[bottomLeftIndex].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, .5f, 1);
        if (grid2D.Cells.ContainsSlot(leftIndex))
            grid2D[leftIndex].MyGameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, 1);
    }


    public int GetTopLeftNeighbourIndex(Cell cell)
    {
        int topLeftIndex = -1;
        if (cell.GridPosition.y % 2 == 0)
            topLeftIndex = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y - 1);
        if (cell.GridPosition.y % 2 != 0)
            topLeftIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);
        if ((GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y) % columnAmount == 0 &&
             cell.GridPosition.y % 2 == 0)) topLeftIndex = -1;
        return (topLeftIndex);
    }

    public int GetTopRightNeighbourIndex(Cell cell)
    {
        int topRightIndex = -1;
        if (cell.GridPosition.y % 2 == 0)
            topRightIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);
        if (cell.GridPosition.y % 2 != 0)
            topRightIndex = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y - 1);
        if (cell.GridPosition.y % 2 != 0 && (cell.GridPosition.x + 1) % rowAmount == 0) topRightIndex = -1;

        return topRightIndex;
    }

    public int GetRightNeighbourIndex(Cell cell)
    {
        int rightIndex = -1;
        if ((cell.GridPosition.x + 1) % rowAmount != 0)
            rightIndex = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y);
        return rightIndex;
    }

    public int GetBottomRightNeighbourIndex(Cell cell)
    {
        int bottomRightIndex = -1;
        if (cell.GridPosition.y % 2 == 0)
            bottomRightIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y + 1);
        if (cell.GridPosition.y % 2 != 0)
            bottomRightIndex = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y + 1);
        if (cell.GridPosition.y % 2 != 0 && (cell.GridPosition.x + 1) % rowAmount == 0) bottomRightIndex = -1;

        return (bottomRightIndex);
    }

    public int GetBottomLeftNeighbourIndex(Cell cell)
    {
        int bottomLeftIndex = -1;
        if (cell.GridPosition.y % 2 == 0)
            bottomLeftIndex = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y + 1);
        if (cell.GridPosition.y % 2 != 0)
            bottomLeftIndex = GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y + 1);
        if (cell.GridPosition.y % 2 == 0 && cell.GridPosition.x % rowAmount == 0) bottomLeftIndex = -1;

        return (bottomLeftIndex);
    }

    public int GetLeftNeighbourIndex(Cell cell)
    {
        int leftIndex = -1;
        if (cell.GridPosition.x != 0)
            leftIndex = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y);
        return leftIndex;
    }
}