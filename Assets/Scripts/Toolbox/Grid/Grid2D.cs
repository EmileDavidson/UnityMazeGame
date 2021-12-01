using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.Grid
{
    [Serializable]
    public class Grid2D
    {
        [SerializeReference] private List<Cell> cells = new List<Cell>();
        public int RowAmount { get; private set; } = 0;
        public int ColumnAmount { get; private set; } = 0;
        public List<Cell> Cells
        {
            get => cells;
            set => cells = value;
        } 

        public Cell this[int i]
        {
            get => cells[i];
            set => cells[i] = value;
        }

        public Grid2D(int rowAmount, int columnAmount, int wallsPerCell)
        {
            this.RowAmount = rowAmount;
            this.ColumnAmount = columnAmount;
            for (int gridY = 0; gridY < rowAmount; gridY++)
            {
                for (int gridX = 0; gridX < columnAmount; gridX++)
                {
                    int index = gridX + columnAmount * gridY;
                    cells.Add(new Cell(new Vector2Int(gridX, gridY), index, wallsPerCell));
                }
            }
        }

        public void ForEach(Action<Cell> action)
        {
            foreach (var cell in cells)
            {
                action.Invoke(cell);
            }
        }
        
            public bool IsBorder(Cell cell, out BorderType type)
            {
                type = BorderType.None;
        
                if (cell.GridPosition.x == 0)
                {
                    type = BorderType.BorderLeft;
                    return true;
                }
        
                if (cell.GridPosition.y == 0)
                {
                    type = BorderType.BorderTop;
                    return true;
                }
        
                if (cell.GridPosition.y == RowAmount - 1)
                {
                    type = BorderType.BorderBottom;
                    return true;
                }
        
                if (cell.GridPosition.x == ColumnAmount - 1)
                {
                    type = BorderType.BorderRight;
                    return true;
                }
        
                return false;
            }
        
            public bool IsCorner(Cell cell, out CornerType type)
            {
                type = CornerType.None;
                
                if (cell.GridPosition.x == 0 && cell.GridPosition.y == 0)
                {
                    type = CornerType.TopLeftCorner;
                    return true;
                }
        
                if (cell.GridPosition.x == ColumnAmount - 1 && cell.GridPosition.y == 0)
                {
                    type = CornerType.TopRightCorner;
                    return true;
                }
        
                if (cell.GridPosition.x == ColumnAmount - 1 && cell.GridPosition.y == RowAmount - 1)
                {
                    type = CornerType.BottomRightCorner;
                    return true;
                }
        
                if (cell.GridPosition.x == 0 && cell.GridPosition.y == RowAmount - 1)
                {
                    type = CornerType.BottomLeftCorner;
                    return true;
                }
        
                return false;
            }


    }
}