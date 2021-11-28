using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Toolbox.Grid;
using Toolbox.MethodExtensions;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexagonMazeGenerator : MazeGenerator
{
    public override void CreateTiles()
    {
        if (tile == null) return;
        float tileOffset = 0;

        onStartCreatingHexagons.Invoke();

        grid2D.ForEach((cell =>
        {
            if (cell.GridPosition.y % 2 == 1) tileOffset = 1;
            else tileOffset = 0;

            var posX = (cell.GridPosition.y * 1.5f * tileScale.x) + (spacing * cell.GridPosition.y);
            var posY = 0;
            var posZ = ((cell.GridPosition.x * 2 + tileOffset) * tileScale.z) + (spacing * cell.GridPosition.x);

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
            var positions = GetTileVertices(cell);
            
            for (int j = 0; j < 6; j++)
            {
                Vector3 start = positions.Get(j);
                Vector3 end = positions.Get(j + 1);
                if (j == 5) end = positions.Get(0);

                var center = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2 * tileScale.y, (start.z + end.z) / 2);
                var rotation = Quaternion.FromToRotation(Vector3.up, end - start).eulerAngles;

                GameObject wallObj = InstantiateWall(center, rotation);
                wallObj.name = "wall: " + j;
                cell.Walls[j] = wallObj;
            }
        });
    }
    
    //TODO: NOT WORKING YET BUT HAD TO START ON MAZE GENERATION.
    public void CreateWallsAroundCells()
    {
        grid2D.ForEach(cell =>
        {
            var positions = GetTileVertices(cell);
            var neighbours = GetNeighboursOf(cell);
            
            for (int j = 0; j < 6; j++)
            {
                Vector3 start = positions.Get(j);
                Vector3 end = positions.Get(j + 1);
                if (j == 5) end = positions.Get(0);
                
                if(neighbours[j] != null && neighbours[j].Walls[j] != null) continue;

                var center = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2 * tileScale.y, (start.z + end.z) / 2);
                var rotation = Quaternion.FromToRotation(Vector3.up, end - start).eulerAngles;

                GameObject wallObj = InstantiateWall(center, rotation);
                wallObj.name = "wall: " + cell.Index + j;
                cell.Walls[j] = wallObj;
                
                var neighbourIndex = j switch
                {
                    0 => GetTopLeftNeighbourIndex(cell),
                    1 => GetTopRightNeighbourIndex(cell),
                    2 => GetLeftNeighbourIndex(cell),
                    3 => GetBottomRightNeighbourIndex(cell),
                    4 => GetBottomLeftNeighbourIndex(cell),
                    5 => GetLeftNeighbourIndex(cell),
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (grid2D.Cells.ContainsSlot(neighbourIndex))
                {
                    grid2D[neighbourIndex].Walls[j] ??= wallObj;
                }
            }
        });
    }
    public override GameObject InstantiateWall(Vector3 center, Vector3 rotation)
    {
        GameObject wallObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallObj.transform.position = center;
        wallObj.transform.parent = wallsParent.transform;
        wallObj.transform.localScale = new Vector3(tileScale.x + .16f, .1f, .1f);

        var transformRotation = wallObj.transform.rotation;
        transformRotation.eulerAngles = new Vector3(0, rotation.y, 0);
        wallObj.transform.localRotation = transformRotation;

        return wallObj;
    }

    public bool IsBorder(Cell cell)
    {
        return (cell.GridPosition.x == 0 ||
                cell.GridPosition.y == 0 || 
                cell.GridPosition.x == rowAmount - 1 ||
                cell.GridPosition.y == columnAmount - 1);
    }

    public override void GenerateMaze()
    {
        // return;
        onStartGeneratingMaze.Invoke();
        
        bool finished = !OptionsLeft();
        while (!finished)
        {
            onUpdateGeneratingMaze.Invoke();
            grid2D[currentCellIndex].VisitedByGenerator = true;
            var next = GetRandomNotVisitedNeighbour(grid2D[currentCellIndex]);
            if (next != null)
            {
                RemoveWallsBetween(grid2D[currentCellIndex], next);
                grid2D[next.Index].VisitedByGenerator = true;
                steps.Add(next);
                currentCellIndex = next.Index;
            }
            else
            {
                if (steps.Count > 1)
                {
                    steps.RemoveAt(steps.Count - 1);
                    currentCellIndex = steps[steps.Count - 1].Index;
                }
            }
            
            finished = !OptionsLeft();
        }
        
        onFinishGeneratingMaze.Invoke();
    }
    
    public int GetWallIndexFromDirection(int direction)
    {
        print(direction);
        return direction switch
        {
            0 => 3,
            1 => 4,
            2 => 5,
            3 => 0,
            4 => 1,
            5 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public bool OptionsLeft()
    {
        foreach (var cell in grid2D.Cells)
        {
            if (cell.VisitedByGenerator == false) return true;
        }
        return false;
    }

    public void RemoveWallsBetween(Cell cell1, Cell cell2)
    {
        int wallIndex1 = GetWallFromTo(cell1, cell2);
        int wallIndex2 = GetWallFromTo(cell2, cell1);

        if (!Application.isPlaying)
        {
            DestroyImmediate(grid2D[cell1.Index].Walls.ToList().Get(wallIndex1));    
            DestroyImmediate(grid2D[cell2.Index].Walls.ToList().Get(wallIndex2));
            return;
        }
        
        Destroy(grid2D[cell1.Index].Walls.ToList().Get(wallIndex1));
        Destroy(grid2D[cell2.Index].Walls.ToList().Get(wallIndex2));
    }

    public int GetWallFromTo(Cell cell1, Cell cell2)
    {
        var cell1Neighbours = GetNeighboursOf(grid2D[cell2.Index]);
        var wallDirection = -1;
        for (int i = 0; i < cell1Neighbours.Count; i++)
        {
            if(cell1Neighbours[i] == null) continue;
            if(cell1Neighbours[i].Index != grid2D[cell1.Index].Index) continue;

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
        var direction = Random.Range(0, notVisitedNeighbours.Count);
        return notVisitedNeighbours[direction];
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

    #endregion
}