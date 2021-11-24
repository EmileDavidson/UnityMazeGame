using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HexagonGenerator : EditorWindow
{
    [MenuItem("GameObject/3D Object/Hexagon", false, 10)]
    public static void GenerateHexagon()
    {
        HexMesh hexMesh = new HexMesh();
        hexMesh.CreateHexagon();
    }
}
