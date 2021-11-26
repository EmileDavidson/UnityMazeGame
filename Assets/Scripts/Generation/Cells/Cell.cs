using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[Serializable]
public class Cell
{
    [SerializeReference] private List<GameObject> _walls = new List<GameObject>();
    [SerializeReference] private GameObject _myGameObject;
    [SerializeReference] private bool _visitedByGenerator = false;
    [SerializeReference] private Vector2Int _gridPosition;

    public Cell(MazeGenerator mazeGenerator, Vector2Int gridPosition)
    {
        this._gridPosition = gridPosition;
    }

    public GameObject MyGameObject
    {
        get => _myGameObject;
        set => _myGameObject = value;
    }

    public bool VisitedByGenerator
    {
        get => _visitedByGenerator;
        set => _visitedByGenerator = value;
    }

    public List<GameObject> Walls
    {
        get => _walls;
        set => _walls = value;
    }

    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set => _gridPosition = value;
    }
}