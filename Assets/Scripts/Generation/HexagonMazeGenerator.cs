using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox.Grid;
using Toolbox.MethodExtensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexagonMazeGenerator : MazeGenerator
{
    private void Start()
    {
        grid2D.ForEach(cell =>
        {
            GetTopLeftNeighbourIndex(cell);
            GetNeighboursOf(cell);
        });
    }

    public override void CreateTiles()
    {
        if (cellTile == null) return;
        float tileOffset = 0;

        onStartCreatingHexagons.Invoke();

        grid2D.ForEach(cell =>
        {
            if (cell.GridPosition.y % 2 == 1) tileOffset = 1;
            else tileOffset = 0;

            var posX = (cell.GridPosition.y * 1.5f * tileScale.x) + (spacing * cell.GridPosition.y);
            var posY = 0;
            var posZ = ((cell.GridPosition.x * 2 + tileOffset) * tileScale.z) + (spacing * cell.GridPosition.x);

            GameObject tile = InstantiateTile(new Vector3(posX, posY, posZ), cell);
            onUpdateCreatingHexagons.Invoke();
        });

        onFinishCreatingHexagons.Invoke();
    }

    public override GameObject InstantiateTile(Vector3 center, Cell cell)
    {
        GameObject tileObj = Instantiate(cellTile, center, Quaternion.identity);
        tileObj.transform.parent = tilesParent.transform;
        tileObj.name = "Hexagon: " + cell.Index;
        tileObj.transform.localScale = new Vector3(tileScale.x, tileScale.y, tileScale.z);

        cell.MyGameObject = tileObj;
        cell.Position = tileObj.transform.position;
        
        return tileObj;
    }

    public override void CreateWalls()
    {
        onStartCreatingWalls.Invoke();

        if (wallPerCell) CreateWallsPerCell();
        else CreateWallsAroundCells();

        onFinishCreatingWalls.Invoke();
    }

    public List<Vector3> GetTileVertices(Cell cell)
    {
        if (cell.MyGameObject == null) return null;
        var list = cell.MyGameObject.GetComponent<MeshFilter>().sharedMesh.vertices;
        List<Vector3> positions = new List<Vector3>();
        foreach (var vert in list)
        {
            positions.Add(vert + cell.MyGameObject.transform.position);
        }

        return positions;
    }


    public void CreateWallsPerCell()
    {
        grid2D.ForEach(cell =>
        {
            onUpdateCreatingWalls.Invoke();
            var positions = GetTileVertices(cell);

            for (int i = 0; i < 6; i++)
            {
                Vector3 start = positions.Get(i);
                Vector3 end = positions.Get(i + 1);
                if (i == 5) end = positions.Get(0);

                var center = new Vector3((start.x + end.x) / 2, 0 + .75f,
                    (start.z + end.z) / 2);
                var rotation = Quaternion.FromToRotation(Vector3.up, end - start).eulerAngles;

                GameObject wallObj = InstantiateWall(center, rotation);
                wallObj.name = "wall: " + cell.Index + " ||| " + i;
                cell.WallsObjects[i] = wallObj;
                cell.Walls[i] = true;
            }
        });
    }

    private void Awake()
    {
    }

    public void CreateWallsAroundCells()
    {
        grid2D.ForEach(cell =>
        {
            var positions = GetTileVertices(cell);
            for (int i = 0; i < 6; i++)
            {
                if (cell.WallsObjects[i] != null) continue;

                Vector3 start = positions.Get(i);
                Vector3 end = positions.Get(i + 1);

                if (i == 5) end = positions.Get(0);

                var center = new Vector3((start.x + end.x) / 2, 0, (start.z + end.z) / 2);
                var rotation = Quaternion.FromToRotation(Vector3.up, end - start).eulerAngles;

                GameObject wallObj = InstantiateWall(center, rotation);
                wallObj.name = "wall: " + cell.Index + " ||| " + i;
                cell.WallsObjects[i] = wallObj;
                cell.Walls[i] = true;

                //get neighbour towards wall 
                int neighbourNumber = i switch
                {
                    0 => GetTopRightNeighbourIndex(cell),
                    1 => GetRightNeighbourIndex(cell),
                    2 => GetBottomRightNeighbourIndex(cell),
                    3 => GetBottomLeftNeighbourIndex(cell),
                    4 => GetLeftNeighbourIndex(cell),
                    5 => GetTopLeftNeighbourIndex(cell),
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (grid2D.Cells.ContainsSlot(neighbourNumber))
                {
                    grid2D[neighbourNumber].WallsObjects.SetAt(i + 3, wallObj);
                    cell.Walls.SetAt(i + 3, true);
                }
            }
        });
    }

    public override GameObject InstantiateWall(Vector3 center, Vector3 rotation)
    {
        GameObject wallObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallObj.transform.position = center;
        wallObj.transform.parent = wallsParent.transform;
        wallObj.transform.localScale = new Vector3(tileScale.x + .16f, wallHeight, .1f);
        
        var transformPosition = wallObj.transform.position;
        transformPosition.y = wallHeight / 2;
        wallObj.transform.position = transformPosition;

        var transformRotation = wallObj.transform.rotation;
        transformRotation.eulerAngles = new Vector3(0, rotation.y, 0);
        wallObj.transform.localRotation = transformRotation;

        return wallObj;
    }

    public override void GenerateMaze()
    {
        // return;
        onStartGeneratingMaze.Invoke();
        
        bool finished = !OptionsLeft();
        while (!finished)
        {
            onUpdateGeneratingMaze.Invoke();
            grid2D[CurrentCellIndex].VisitedByGenerator = true;
            var next = GetRandomNotVisitedNeighbour(grid2D[CurrentCellIndex]);
            if (next != null)
            {
                RemoveWallsBetween(grid2D[CurrentCellIndex], next);
                grid2D[next.Index].VisitedByGenerator = true;
                Steps.Add(next);
                CurrentCellIndex = next.Index;
            }
            else
            {
                if (Steps.Count > 1)
                {
                    Steps.RemoveAt(Steps.Count - 1);
                    CurrentCellIndex = Steps[Steps.Count - 1].Index;
                }
            }
            
            finished = !OptionsLeft();
        }
        
        onFinishGeneratingMaze.Invoke();
    }

    public bool OptionsLeft()
    {
        return grid2D.Cells.Any(cell => cell.VisitedByGenerator == false);
    }

    public void RemoveWallsBetween(Cell cell1, Cell cell2)
    {
        int wallIndex1 = GetWallFromTo(cell1, cell2);
        int wallIndex2 = GetWallFromTo(cell2, cell1);
        
        grid2D[cell1.Index].Walls.SetAt(wallIndex1, false);
        grid2D[cell2.Index].Walls.SetAt(wallIndex2, false);

        if (!Application.isPlaying)
        {
            DestroyImmediate(grid2D[cell1.Index].WallsObjects.ToList().Get(wallIndex1));
            DestroyImmediate(grid2D[cell2.Index].WallsObjects.ToList().Get(wallIndex2));
            return;
        }

        Destroy(grid2D[cell1.Index].WallsObjects.ToList().Get(wallIndex1));
        Destroy(grid2D[cell2.Index].WallsObjects.ToList().Get(wallIndex2));
        

    }

    public int GetWallFromTo(Cell cell1, Cell cell2)
    {
        var cell1Neighbours = GetNeighboursOf(grid2D[cell2.Index]);
        var wallDirection = -1;
        for (int i = 0; i < cell1Neighbours.Count; i++)
        {
            if (cell1Neighbours[i] == null) continue;
            if (cell1Neighbours[i].Index != grid2D[cell1.Index].Index) continue;

            wallDirection = i - 1;
            break;
        }

        int wallIndex1 = wallDirection + 3;

        return wallIndex1;
    }

    public Cell GetRandomNotVisitedNeighbour(Cell aCell)
    {
        var neighbours = GetNeighboursOf(aCell);
        var notVisitedNeighbours = new List<Cell>();
        foreach (var cell in neighbours.Where(linqCell => linqCell is { VisitedByGenerator: false }))
        {
            notVisitedNeighbours.Add(cell);
        }

        if (notVisitedNeighbours.IsEmpty()) return null;

        int direction = Random.Range(0, notVisitedNeighbours.Count - 1);
        return notVisitedNeighbours[direction];
    }


    private void Update()
    {
        if (debugging)
        {
            DebugNeighbours();
        }
    }

    public void DebugNeighbours()
    {
        if (performanceMode) return;
        var neighbours = GetNeighboursOf(grid2D[debuggingIndex]);

        grid2D.ForEach((cell =>
        {
            if (cell.MyGameObject == null) return;
            cell.MyGameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
        }));

        if (neighbours[0]?.MyGameObject != null)
            neighbours[0].MyGameObject.GetComponent<Renderer>().material.color = new Color(.5f, 0, 0, 1);
        if (neighbours[1]?.MyGameObject != null)
            neighbours[1].MyGameObject.GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 1);
        if (neighbours[2]?.MyGameObject != null)
            neighbours[2].MyGameObject.GetComponent<Renderer>().material.color = new Color(0, .5f, 0, 1);
        if (neighbours[3]?.MyGameObject != null)
            neighbours[3].MyGameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
        if (neighbours[4]?.MyGameObject != null)
            neighbours[4].MyGameObject.GetComponent<Renderer>().material.color = new Color(0, 0, .5f, 1);
        if (neighbours[5]?.MyGameObject != null)
            neighbours[5].MyGameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 1);
    }


    #region NeighbourFunctions

    public override List<Cell> GetNeighboursOf(Cell cell)
    {
        int topLeftIndex = GetTopLeftNeighbourIndex(cell);
        int topRightIndex = GetTopRightNeighbourIndex(cell);
        int rightIndex = GetRightNeighbourIndex(cell);
        int bottomRightIndex = GetBottomRightNeighbourIndex(cell);
        int bottomLeftIndex = GetBottomLeftNeighbourIndex(cell);
        int leftIndex = GetLeftNeighbourIndex(cell);

        var list = new List<Cell>
        {
            grid2D.Cells.ContainsSlot(topLeftIndex) ? grid2D[topLeftIndex] : null,
            grid2D.Cells.ContainsSlot(topRightIndex) ? grid2D[topRightIndex] : null,
            grid2D.Cells.ContainsSlot(rightIndex) ? grid2D[rightIndex] : null,
            grid2D.Cells.ContainsSlot(bottomRightIndex) ? grid2D[bottomRightIndex] : null,
            grid2D.Cells.ContainsSlot(bottomLeftIndex) ? grid2D[bottomLeftIndex] : null,
            grid2D.Cells.ContainsSlot(leftIndex) ? grid2D[leftIndex] : null
        };

        return list;
    }

    public int GetTopLeftNeighbourIndex(Cell cell)
    {
        int topLeftIndex = -1;
        topLeftIndex = cell.GridPosition.y % 2 == 0
            ? GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y - 1)
            : GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1);

        if (!grid2D.IsBorder(cell, out var type)) return (topLeftIndex);
        if (type == BorderType.BorderLeft && cell.GridPosition.y % 2 == 0) topLeftIndex = -1;

        return (topLeftIndex);
    }

    public int GetTopRightNeighbourIndex(Cell cell)
    {
        int topRightIndex = -1;
        topRightIndex = cell.GridPosition.y % 2 == 0
            ? GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y - 1)
            : GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y - 1);

        if (!grid2D.IsBorder(cell, out var type)) return topRightIndex;
        if (type == BorderType.BorderRight && cell.GridPosition.y % 2 != 0) topRightIndex = -1;
        if (grid2D.IsCorner(cell, out var cornerType) && cornerType == CornerType.BottomRightCorner) topRightIndex = -1;


        return topRightIndex;
    }

    public int GetRightNeighbourIndex(Cell cell)
    {
        int rightIndex = -1;
        rightIndex = GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y);

        if (!grid2D.IsBorder(cell, out var borderType)) return rightIndex;
        grid2D.IsCorner(cell, out var cornerType);
        if (borderType == BorderType.BorderRight || cornerType == CornerType.BottomRightCorner ||
            cornerType == CornerType.TopRightCorner) rightIndex = -1;
        return rightIndex;
    }

    public int GetBottomRightNeighbourIndex(Cell cell)
    {
        int bottomRightIndex = -1;
        bottomRightIndex = cell.GridPosition.y % 2 == 0
            ? GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y + 1)
            : GetIndexFromGridPosition(cell.GridPosition.x + 1, cell.GridPosition.y + 1);

        if (!grid2D.IsBorder(cell, out var type)) return bottomRightIndex;
        if (type == BorderType.BorderRight && cell.GridPosition.y % 2 != 0) bottomRightIndex = -1;

        return (bottomRightIndex);
    }

    public int GetBottomLeftNeighbourIndex(Cell cell)
    {
        int bottomLeftIndex = -1;
        bottomLeftIndex = cell.GridPosition.y % 2 == 0
            ? GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y + 1)
            : GetIndexFromGridPosition(cell.GridPosition.x, cell.GridPosition.y + 1);

        if (!grid2D.IsBorder(cell, out var type)) return bottomLeftIndex;
        grid2D.IsCorner(cell, out var cornerType);
        if (type == BorderType.BorderLeft && cell.GridPosition.y % 2 == 0) bottomLeftIndex = -1;
        if (cornerType == CornerType.BottomLeftCorner) bottomLeftIndex = -1;

        return (bottomLeftIndex);
    }

    public int GetLeftNeighbourIndex(Cell cell)
    {
        int leftIndex = -1;
        leftIndex = GetIndexFromGridPosition(cell.GridPosition.x - 1, cell.GridPosition.y);

        if (!grid2D.IsBorder(cell, out var borderType)) return leftIndex;
        grid2D.IsCorner(cell, out var cornerType);
        if (borderType == BorderType.BorderLeft || cornerType == CornerType.BottomLeftCorner ||
            cornerType == CornerType.TopLeftCorner) leftIndex = -1;

        return leftIndex;
    }

    #endregion
}