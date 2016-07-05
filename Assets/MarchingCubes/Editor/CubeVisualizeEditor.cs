using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CubeVisualize))]
public class CubeVisualizeEditor : Editor
{
    [MenuItem("GameObject/CreateMarchingCubeTable")]
    public static void GenerateAllPossible()
    {
        if (Selection.activeGameObject == null)
        {
            return;
        }
        GameObject parent = new GameObject("Group");
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/MarchingCubes/NoCull.mat");
        for (int i = 0; i < 256; i++)
        {
            GameObject newGo = Instantiate(Selection.activeGameObject);
            newGo.name = i.ToString();
            newGo.transform.SetParent(parent.transform, false);
            newGo.transform.localPosition = new Vector3(i / 16, i % 16, 0);
            var marchingCube = newGo.AddComponent<CubeVisualize>();
            marchingCube.cube = i;
            var mesh = newGo.GetComponent<MeshRenderer>();
            mesh.sharedMaterial = mat;
        }
    }

    static int s_Hash = "MarchingCube".GetHashCode();

    private GUIStyle edgeStyle = new GUIStyle();

    private GUIStyle vertexStyle = new GUIStyle();

    CubeVisualize mono;

    void OnSceneGUI()
    {
        mono = target as CubeVisualize;
        Handles.matrix = mono.transform.localToWorldMatrix;
        DrawMarchingCube(mono.cube);
        Handles.matrix = Matrix4x4.identity;
    }

    void OnEnable()
    {
        edgeStyle.normal.textColor = Color.green;
        vertexStyle.normal.textColor = Color.red;

        mono = target as CubeVisualize;
    }
    
    void DrawMarchingCube(int cube)
    {
        DrawVertex(cube);
        DrawEdge(cube);
    } 

    void DrawVertex(int cube)
    {
        Color c = Handles.color;
        Handles.color = new Color(0.9f, 0, 0, 0.3f);
        for (int i = 0; i < 8; i++)
        {
            Color color = new Color(0.9f, 0.8f, 0, 0.3f);
            if ((cube & (1 << i)) != 0)
            {
                color = new Color(0.1f, 1f, 0, 0.9f);
            }
            Handles.color = color;
            Vector3 pos = MarchingCubeLookupTable.pointTable[i] + mono.offset;
            DrawVertexInternel(pos, i);
            
            Handles.Label(pos, i.ToString(), vertexStyle);
        }

        Handles.color = c;
    }

    void DrawVertexInternel(Vector3 pos, int index)
    {
        if (Handles.Button(pos, Quaternion.identity, 0.05f, 0.05f, Handles.DotCap))
        {
            mono.cube ^= 1 << index;
        }
    }

    void DrawEdge(int cube)
    {
        var edgeTable = MarchingCubeLookupTable.edgeTable;
        var points = MarchingCubeLookupTable.pointTable;

        for (int i = 0; i < 12; i++)
        {
            _DrawEdge(points[edgeTable[i, 0]], points[edgeTable[i, 1]], i);
        }
    }

    void _DrawEdge(Vector3 p1, Vector3 p2, int pIndex)
    {
        p1 += mono.offset;
        p2 += mono.offset;
        Color c = Handles.color;
        Handles.color = new Color(0, 1, 0, 0.5f);
        Handles.DrawLine(p1, p2);

        Handles.Label((p1 + p2) * 0.5f, pIndex.ToString(), edgeStyle);
        Handles.color = c;
    }
}