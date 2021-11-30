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
            for (int i = 0; i < filters.Count; i++)
            {
                if (filters[i] == null) continue;
                combine[i].mesh = filters[i].sharedMesh;
                combine[i].transform = filters[i].transform.localToWorldMatrix;
            }
            return combine;
        }
    }
    
    /// <summary>
    /// Combines all meshes in one big mesh note that there can only be one texture 
    /// </summary>
    public class MyMeshUtility
    {
        public static bool CombineMeshes(List<GameObject> gameObjects, GameObject newGameObject, Material material = null)
        {
            if (gameObjects.IsEmpty()) return false;

            if (material == null) material = gameObjects[0].GetComponent<Renderer>().sharedMaterial;
            List<MeshFilter> meshFilters = new List<MeshFilter>();


            List<Chunk> chunks = new List<Chunk>(){new Chunk(new GameObject())};
            int verticesCount = 0;

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.HasAndGetComponent<MeshFilter>(out var comp)) continue;
                if (verticesCount + comp.sharedMesh.vertexCount >= (65535 - 1) / 2)
                {
                    //create new chunk
                    chunks.Add(new Chunk(new GameObject()));
                    chunks[chunks.Count - 1].gameObjects.Add(gameObject);
                    chunks[chunks.Count - 1].filters.Add(comp);
                    verticesCount = 0;
                    continue;
                }
                //add to existing chunk
                chunks[chunks.Count - 1].gameObjects.Add(gameObject);
                chunks[chunks.Count - 1].filters.Add(comp);
                verticesCount += comp.sharedMesh.vertexCount;
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

            foreach (var obj in gameObjects)
            {
                if (Application.isPlaying) Object.Destroy(obj);
                else { Object.DestroyImmediate(obj); }
            }

            return true;
        }
    }
}