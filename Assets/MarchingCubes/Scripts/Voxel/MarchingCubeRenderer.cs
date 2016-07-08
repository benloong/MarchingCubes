using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class MarchingCubeRenderer : MonoBehaviour
{
    [SerializeField]
    public VoxelData voxel = new VoxelData();

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

    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Marching";
        mesh.MarkDynamic();
        mesh.Clear();
        
        GetComponent<MeshFilter>().sharedMesh = mesh;
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

        mesh.Clear();
        if (vertices.Count > 65000)
        {
            mesh.subMeshCount = vertices.Count / 65000;
            mesh.SetVertices(vertices);
        }
        else
        {
            mesh.SetVertices(vertices);
            mesh.triangles = vertices.Select((x, i) => i).ToArray();
        }
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
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
