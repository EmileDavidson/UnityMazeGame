using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[Serializable]
public class Cell
{
    [SerializeReference] private List<GameObject> walls = new List<GameObject>();
    [SerializeReference] private GameObject myGameObject;
    [SerializeReference] private bool visitedByGenerator = false;

    public GameObject MyGameObject
    {
        get => myGameObject;
        set => myGameObject = value;
    }
}