using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HexagonCell : Cell
{
    public HexagonCell(MazeGenerator mazeGenerator, Vector2Int gridPosition) : base(mazeGenerator, gridPosition)
    {
        this.GridPosition = gridPosition;
        this.MazeGenerator = mazeGenerator;
    }
}