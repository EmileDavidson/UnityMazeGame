using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.Grid
{
    [Serializable]
    public class Grid2D
    {
        [SerializeReference] private List<Cell> cells = new List<Cell>();
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


    }
}