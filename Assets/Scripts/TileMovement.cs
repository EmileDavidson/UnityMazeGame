using Toolbox.Grid;
using UnityEngine;
using System.Collections.Generic;
using Toolbox.MethodExtensions;
using Random = UnityEngine.Random;

public class TileMovement : MonoBehaviour
{
    private Vector3 _screenClickPos;

    private MovementDirections _currentDirection;
    private Cell _currentCell;
    private Cell _nextCell;

    private HexagonMazeGenerator _generator;
    private List<Cell> _mapCells = new List<Cell>();

    private void Awake()
    {
        _generator = FindObjectOfType<HexagonMazeGenerator>();
        _mapCells = _generator.Grid2D.Cells;
        
        _currentCell = _mapCells[Random.Range(0, _mapCells.Count)];
        transform.position = _currentCell.Position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    void OnClick()
    {
        float circleAngle;

        Vector3 normalizedClick = new Vector3();
        
        _screenClickPos = Input.mousePosition;
        normalizedClick.x = (_screenClickPos.x / Screen.width - 0.5f) * 2;
        normalizedClick.y = (_screenClickPos.y / Screen.height - 0.5f) * 2;

        circleAngle = (Mathf.Atan2(normalizedClick.y, normalizedClick.x) * 180 / Mathf.PI) + 180;
        _currentDirection = GetDirection(circleAngle);

        _nextCell = getCellFromDirection(_currentDirection);
        
        Debug.Log(_currentCell.Index + ", " + _nextCell.Index);
        
        int wallIndex = _generator.GetWallIndexFromTo(_currentCell, _nextCell);
        bool hasWall = _currentCell.Walls.Get(wallIndex);

        if (!hasWall) MoveToTile(_nextCell);
    }

    void MoveToTile(Cell tile)
    {
        transform.position = tile.Position;
        _currentCell = tile;
    }

    Cell getCellFromDirection(MovementDirections dir)
    {
        int tileIndex = 0;
        switch (dir)
        {
            case MovementDirections.TopRight:
                tileIndex = _generator.GetTopRightNeighbourIndex(_currentCell);
                break;
            case MovementDirections.Right:
                tileIndex = _generator.GetRightNeighbourIndex(_currentCell);
                break;
            case MovementDirections.BottomRight:
                tileIndex = _generator.GetBottomRightNeighbourIndex(_currentCell);
                break;
            case MovementDirections.BottomLeft:
                tileIndex = _generator.GetBottomLeftNeighbourIndex(_currentCell);
                break;
            case MovementDirections.Left:
                tileIndex = _generator.GetLeftNeighbourIndex(_currentCell);
                break;
            case MovementDirections.TopLeft:
                tileIndex = _generator.GetTopLeftNeighbourIndex(_currentCell);
                break;
        }
        return _mapCells[tileIndex];
    }

    MovementDirections GetDirection(float angle)
    {
        if (angle < 270 && angle > 210)
        {
            return MovementDirections.TopRight;
        }
        if (angle < 210 && angle > 150)
        {
            return MovementDirections.Right;
        }
        if (angle < 150 && angle > 90)
        {
            return MovementDirections.BottomRight;
        }
        if (angle < 90 && angle > 30)
        {
            return MovementDirections.BottomLeft;
        }
        if (angle < 30 || angle > 330)
        {
            return MovementDirections.Left;
        }
        if (angle < 330 && angle > 270)
        {
            return MovementDirections.TopLeft;
        }
        return MovementDirections.None;
    }
}
