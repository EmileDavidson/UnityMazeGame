using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.Grid
{
    [Serializable]
    public class Cell
    {
        [SerializeReference] private GameObject[] walls;
        [SerializeReference] private GameObject myGameObject;
        [SerializeField] private Vector3 position;
        [SerializeReference] private bool visitedByGenerator = false;
        [SerializeReference] public Vector2Int gridPosition;
        [SerializeReference] private int index;

        public Cell(Vector2Int gridPosition, int index, int wallAmount)
        {
            GridPosition = gridPosition;
            Index = index;
            walls = new GameObject[wallAmount];
        }

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public GameObject MyGameObject
        {
            get => myGameObject;
            set => myGameObject = value;
        }

        public bool VisitedByGenerator
        {
            get => visitedByGenerator;
            set => visitedByGenerator = value;
        }

        public GameObject[] Walls
        {
            get => walls;
            set => walls = value;
        }

        public Vector2Int GridPosition
        {
            get => gridPosition;
            set => gridPosition = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }
    }
}