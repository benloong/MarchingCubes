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

    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();

    void Awake()
    {
        mesh = new Mesh();
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
        mesh.vertices = vertices.ToArray();
        mesh.triangles = vertices.Select((x, i) => i).ToArray();
        mesh.RecalculateNormals();
    }

    void Marching(int x, int y, int z)
    {
        int cubeIndex = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    int vertex = i * 4 + j * 2 + k;
                    vertex = MarchingCubeLookupTable.vertexMapping[vertex];
                    if (voxel[x + i, y + j, z + k] > 0)
                    {
                        cubeIndex |= 1 << vertex;
                    }
                }
            }
        }

        MarchingCube.Polygonise((byte)cubeIndex, vertices, new Vector3(x, y, z));
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
