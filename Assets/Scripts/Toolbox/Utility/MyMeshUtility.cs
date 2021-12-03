using System.Collections.Generic;
using System.Runtime.InteropServices;
using Toolbox.MethodExtensions;
using UnityEditor;
using UnityEngine;

namespace Toolbox.Utility
{

    public struct Chunk
    {
        public List<GameObject> gameObjects;
        public GameObject parent;
        public List<MeshFilter> filters;

        public Chunk(GameObject aParent)
        {
            this.parent = aParent;
            this.parent.name = "MeshChunk";
            gameObjects = new List<GameObject>();
            filters = new List<MeshFilter>();
        }

        public CombineInstance[] ToCombineInstance()
        {
            CombineInstance[] combine = new CombineInstance[filters.Count];
            for (int i = filters.Count / 2; i < filters.Count * 2; i++)
            {
                if (filters.Get(i) == null) continue;
                combine[combine.GetPossibleIndex(i - 1)].mesh = filters.Get(i).sharedMesh;
                combine[combine.GetPossibleIndex(i - 1)].transform = filters.Get(i).transform.localToWorldMatrix;
            }
            return combine;
        }
    }
    
    /// <summary>
    /// Combines all meshes in one big mesh note that there can only be one texture 
    /// </summary>
    public class MyMeshUtility
    {
        public static bool CombineMeshes(List<MeshFilter> filters, GameObject newGameObject, Material material = null)
        {
            if (filters.IsEmpty()) return false;

            if (material == null) material = filters[0].GetComponent<Renderer>().sharedMaterial;
            List<MeshFilter> meshFilters = new List<MeshFilter>();


            List<Chunk> chunks = new List<Chunk>(){new Chunk(new GameObject())};
            int verticesCount = 0;

            foreach (var filter in filters)
            {
                if (verticesCount + filter.sharedMesh.vertexCount >= (65535 - 1))
                {
                    //create new chunk
                    chunks.Add(new Chunk(new GameObject()));
                    chunks[chunks.Count - 1].filters.Add(filter);
                    verticesCount = 0;
                    continue;
                }
                //add to existing chunk
                chunks[chunks.Count - 1].filters.Add(filter);
                verticesCount += filter.sharedMesh.vertexCount;
                continue;
            }

            foreach (var chunk in chunks)
            {
                MeshFilter meshFilter = chunk.parent.GetOrAddComponent<MeshFilter>();
                MeshRenderer renderer = chunk.parent.GetOrAddComponent<MeshRenderer>();

                meshFilter.sharedMesh = new Mesh();
                meshFilter.sharedMesh.CombineMeshes(chunk.ToCombineInstance());
                meshFilter.sharedMesh.Optimize();
                
                renderer.sharedMaterial = material;

                chunk.parent.transform.parent = newGameObject.transform;
            }

            foreach (var filter in filters)
            {
                if (filter != null && filter.gameObject != null)
                {
                    if (Application.isPlaying) Object.Destroy(filter.gameObject);
                    else { Object.DestroyImmediate(filter.gameObject); }
                }
            }

            return true;
        }
    }
}