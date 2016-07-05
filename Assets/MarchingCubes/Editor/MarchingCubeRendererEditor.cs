using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MarchingCubeRenderer))]
public class MarchingCubeRendererEditor : Editor
{
    MarchingCubeRenderer voxelRenderer;

    static int s_Hash = "MarchingCube".GetHashCode();

    private GUIStyle edgeStyle = new GUIStyle();

    private GUIStyle vertexStyle = new GUIStyle();

    int width;
    int height;
    int depth;

    void OnEnable()
    {
        edgeStyle.normal.textColor = Color.green;
        vertexStyle.normal.textColor = Color.red;

        voxelRenderer = target as MarchingCubeRenderer;
        width = voxelRenderer.voxel.resolution;
        height = width;
        depth = width;
    }

    public override void OnInspectorGUI()
    {
        int newValue = EditorGUILayout.IntSlider("Resolution:", voxelRenderer.voxel.resolution, 1, 15);
        if (newValue != voxelRenderer.voxel.resolution)
        {
            voxelRenderer.voxel.resolution = newValue;
            voxelRenderer.GenerateMesh();
        }
    }

    void OnSceneGUI()
    {
        voxelRenderer = target as MarchingCubeRenderer;
        voxelRenderer = target as MarchingCubeRenderer;
        width = voxelRenderer.voxel.width;
        height = voxelRenderer.voxel.height;
        depth = voxelRenderer.voxel.depth;

        Handles.matrix = voxelRenderer.transform.localToWorldMatrix;

        DrawVertices();
        DrawEdges();

        Handles.matrix = Matrix4x4.identity;
    }

    void DrawVertices()
    {
        Color c = Handles.color;

        var voxelData = voxelRenderer.voxel;

        for (int x = 0; x < voxelData.width; x++)
        {
            for (int y = 0; y < voxelData.height; y++)
            {
                for (int z = 0; z < voxelData.depth; z++)
                {
                    DrawVertexInternel(x, y, z);
                }
            }
        }
        Handles.color = c;
    }

    void DrawVertexInternel(int x, int y, int z)
    {
        Color color = new Color(0.2f, 0.8f, 0, 0.3f);
        if (voxelRenderer.voxel[x, y, z] > 0)
        {
            color = new Color(0.1f, 1f, 0, 1f);
        }
        Handles.color = color;

        Vector3 pos = new Vector3(x, y, z);
        if (Handles.Button(pos, Quaternion.identity, 0.05f, 0.05f, Handles.DotCap))
        {
            voxelRenderer.voxel[x, y, z] ^= 1;
            voxelRenderer.GenerateMesh();
        }
    }

    void DrawEdges()
    {
        Color c = Handles.color;
        Handles.color = new Color(0, 1, 0, 0.5f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Handles.DrawLine(new Vector3(x, y, 0), new Vector3(x, y, depth - 1));
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                Handles.DrawLine(new Vector3(0, y, z), new Vector3(width - 1, y, z));
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Handles.DrawLine(new Vector3(x, 0, z), new Vector3(x, height - 1, z));
            }
        }
        Handles.color = c;
    }     
}
