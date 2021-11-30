using System.Collections;
using System.Collections.Generic;
using Toolbox.Utility;
using UnityEngine;

public class TestingScript2 : MonoBehaviour
{
    public List<GameObject> _list = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        _list.ForEach((obj =>
        {
            print(obj.GetComponent<MeshFilter>().mesh.vertexCount);
            print(obj.GetComponent<MeshFilter>().sharedMesh.vertexCount);
        }));

        MyMeshUtility.CombineMeshes(_list, this.gameObject);
        
        print("===== CHUNCK OBJ WITH NEW MESH =====");
        print(this.gameObject.GetComponentInChildren<MeshFilter>().mesh.vertexCount);
        print(this.gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.vertexCount);
    }
}
