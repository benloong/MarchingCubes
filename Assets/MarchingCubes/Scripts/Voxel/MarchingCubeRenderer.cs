using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class MarchingCubeRenderer : MonoBehaviour
{
    [SerializeField]
    public VoxelData voxel = new VoxelData();

    public Material material;

    [SerializeField]
    byte _isoValue = 100;

    public int isoValue
    {
        get
        {
            return _isoValue;
        }

        set
        {
            if (_isoValue != value)
            {
                _isoValue = (byte)value;
                GenerateMesh();
            }
        }
    }
     

    List<Vector3> vertices = new List<Vector3>();

    void Awake()
    { 
    }

    void Start()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        int w = voxel.data.GetLength(0);
        int h = voxel.data.GetLength(1);
        int d = voxel.data.GetLength(2);

        vertices.Clear();
        for (int x = -1; x < w; x++)
        {
            for (int y = -1; y < h; y++)
            {
                for (int z = -1; z < d; z++)
                {
                    Marching(x, y, z);
                }
            }
        }

        const int maxVertexCount = 63000;
        int meshCount = vertices.Count / maxVertexCount + 1;
        for (int i = 0; i < meshCount; i++)
        {
            var meshFilter = GetMeshFilter(i);
            meshFilter.gameObject.SetActive(true);
            meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh.vertices = vertices.Skip(maxVertexCount * i).Take(maxVertexCount).ToArray();
            meshFilter.sharedMesh.triangles = vertices.Skip(maxVertexCount * i).Take(maxVertexCount).Select((v, index) => index).ToArray();
            meshFilter.sharedMesh.RecalculateNormals();
        }

        int childCount = transform.childCount;

        
        for (int i = childCount; i > meshCount; i--)
        {
            var child = transform.GetChild(i - 1);
            child.SetParent(null, false);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    void Marching(int x, int y, int z)
    {
        MarchingCube.Polygonise(x, y, z, Lookup, vertices, new Vector3(x, y, z), _isoValue);
    }

    byte Lookup(int x, int y, int z)
    {
        return voxel[x, y, z];
    }

    void DrawVertex(int x, int y, int z)
    {
        Color old = Gizmos.color;
        var c = Color.magenta;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawSphere(new Vector3(x, y, z), 0.05f);
        Gizmos.color = old;
    }

    void DrawVertexEmpty(int x, int y, int z)
    {
        Color old = Gizmos.color;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(new Vector3(x, y, z), 0.06f);
        Gizmos.color = old;
    }

    public void OnDrawGizmos()
    {
        //DrawAllVertex();
    }

    MeshFilter GetMeshFilter(int index)
    {
        if (index >= transform.childCount)
        {
            var child = new GameObject(index.ToString());
            var rend = child.AddComponent<MeshRenderer>();
            rend.sharedMaterial = material;

            var filter = child.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.MarkDynamic();
            mesh.name = index.ToString();
            mesh.Clear();
            filter.sharedMesh = mesh;

            child.hideFlags = HideFlags.HideInHierarchy;
            child.transform.SetParent(transform, false);
        }

        return transform.GetChild(index).GetComponent<MeshFilter>();
    }

    void DrawAllVertex()
    {
        int w = voxel.data.GetLength(0);
        int h = voxel.data.GetLength(1);
        int d = voxel.data.GetLength(2);
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                for (int z = 0; z < d; z++)
                {
                    bool sample = voxel[x, y, z] > 0;
                    if (sample)
                    {
                        DrawVertex(x, y, z);
                    }
                    else
                    {
                        DrawVertexEmpty(x, y, z);
                    }
                }
            }
        }
    }
}
