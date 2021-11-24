using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HexMesh : MonoBehaviour
{
    [SerializeField] private float width = 1;
    [SerializeField] private float height = 1;

    public void CreateHexagon()
    {
        GameObject hexagon = new GameObject();
        hexagon.name = "Hexagon";
        
        MeshRenderer meshRenderer = hexagon.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        
        MeshFilter meshFilter = hexagon.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        mesh = CreateHexagonMesh(mesh);
        mesh.name = "hexagonMesh";
        
        MeshCollider meshCollider = hexagon.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;

        meshFilter.mesh = mesh;
    }

    private Mesh CreateHexagonMesh(Mesh mesh)
    {
        Vector3[] vertices =
        {
            //top plane
            new Vector3(-width, height/2, 0),
            new Vector3(-width/2, height/2, width),
            new Vector3(width/2, height/2, width),
            new Vector3(width, height/2, 0),
            new Vector3(width/2, height/2, -width),
            new Vector3(-width/2, height/2, -width),
            //bottom plane
            new Vector3(-width, -height/2, 0),
            new Vector3(-width/2, -height/2, width),
            new Vector3(width/2, -height/2, width),
            new Vector3(width, -height/2, 0),
            new Vector3(width/2, -height/2, -width),
            new Vector3(-width/2, -height/2, -width)
        };
        mesh.vertices = vertices;
        
        int[] tris =
        {
            // top plane
            // 0, 1, 2,
            // 2, 3, 4,
            // 4, 5, 0,
            // 0, 2, 4,
            0, 1, 5,
            4, 5, 1,
            1, 2, 4,
            3, 4, 2,
            // bottom plane
            11, 7, 6,
            7, 11, 10,
            10, 8, 7,
            8, 10, 9,
            // side 0
            7, 1, 0,
            0, 6, 7,
            // side 1
            8, 2, 1,
            1, 7, 8,
            // side 2
            9, 3, 2,
            2, 8, 9,
            // side 3
            10, 4, 3,
            3, 9, 10,
            // side 4
            11, 5, 4,
            4, 10, 11,
            // side 5
            6, 0, 5,
            5, 11, 6
        };
        mesh.triangles = tris;

        Vector3[] normals =
        {
            //top plane
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            //bottom plane
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up
        };
        mesh.normals = normals;

        Vector2[] uv =
        {
            new Vector2(0, 0.5f),
            new Vector2(0.33f, 1),
            new Vector2(0.66f, 1),
            new Vector2(1, 0.5f),
            new Vector2(0.66f, 0),
            new Vector2(0.33f, 0),
            //
            new Vector2(1, 0.5f),
            new Vector2(0.66f, 0),
            new Vector2(0.33f, 0),
            new Vector2(0, 0.5f),
            new Vector2(0.33f, 1),
            new Vector2(0.66f, 1),
        };
        mesh.uv = uv;

        return mesh;
    }
}
