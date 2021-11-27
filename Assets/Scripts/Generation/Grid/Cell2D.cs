using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generation.Grid
{
    [Serializable]
    public class Cell
    {
        [SerializeReference] private List<GameObject> walls = new List<GameObject>();
        [SerializeReference] private GameObject myGameObject;
        [SerializeReference] private bool visitedByGenerator = false;
        [SerializeReference] public Vector2Int gridPosition;
        [SerializeReference] private int index;

        public Cell(Vector2Int gridPosition, int index)
        {
            GridPosition = gridPosition;
            Index = index;
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

        public List<GameObject> Walls
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