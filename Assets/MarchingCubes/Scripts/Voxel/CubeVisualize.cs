using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class CubeVisualize : MonoBehaviour {
    [Range(0,255), SerializeField]
    private int _cube;
    public int cube
    {
        set
        {
            if (value != _cube)
            {
                _cube = value;
                GenerateMesh();
            }
        }
        get
        {
            return _cube;
        }
    }

    int lastCube = -1;
    MeshFilter meshFilter;
    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();

    public Vector3 offset;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.MarkDynamic();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        if (lastCube != cube)
        {
            GenerateMesh();
            lastCube = cube;
        }
    }

    void GenerateMesh()
    {
        vertices.Clear();
        MarchingCube.Polygonise((byte)cube, vertices, offset);
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = vertices.Select((x, i) => i).ToArray();
        mesh.RecalculateNormals();

    }

    void OnDrawGizmos()
    {
        //Gizmos.matrix = transform.localToWorldMatrix;
        //for (int i = 0; i < 8; i++)
        //{
        //    int mask = 1 << i;
        //    if ((cube & mask) != 0)
        //    {
        //        DrawVertex(i);
        //    }
        //    else
        //    {
        //        DrawVertexEmpty(i);
        //    }
        //}
        //Gizmos.matrix = Matrix4x4.identity;
    }

    void OnDrawGizmosSelected()
    {
        //if (mesh.vertexCount > 0)
        //{
        //    Gizmos.matrix = transform.localToWorldMatrix;
        //    Gizmos.color = Color.cyan;
        //    Gizmos.DrawMesh(mesh);
        //    Gizmos.matrix = Matrix4x4.identity;
        //}
    }

    void DrawVertex(int i)
    {
        int x = i / 2 / 2;
        int y = i / 2 % 2;
        int z = i % 2;
        Color old = Gizmos.color;
        var c = Color.magenta;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawSphere(new Vector3(-0.5f, -0.5f, -0.5f) + new Vector3(x, y, z), 0.05f);
        Gizmos.color = old;
    }

    void DrawVertexEmpty(int i)
    {
        int x = i / 2 / 2;
        int y = i / 2 % 2;
        int z = i % 2;
        Color old = Gizmos.color;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(new Vector3(-0.5f, -0.5f, -0.5f) + new Vector3(x, y, z), 0.06f);
        Gizmos.color = old;
    }

    void OnEnable()
    {
        GenerateMesh();
    }
}
