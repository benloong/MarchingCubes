using UnityEngine;
using System.Collections;
using System.Linq;

public class OctreeDebug : MonoBehaviour {
    public Octree octree;
    
    [Range(0, 4)]
    public int maxDepth;

    public Color color;

    [Range(0, 4)]
    public int cube;

    void Start()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Debug.Log(new Vector3(x, y, z) + new Vector3(-1, -1, -1)* 0.5f);
                }
            }
        }
    }
     
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        float d;
        if(octree.Raycast(ray, out d))
        {
            Debug.DrawRay(ray.origin, ray.direction * d, Color.cyan);
        }
    }

    void OnDrawGizmos()
    {
        var old = Gizmos.color;
        Gizmos.color = color;
        DrawOctree(octree, 0);

        Gizmos.color = old;
    }

    void DrawOctree(Octree octree, int depth)
    {
        if (depth < maxDepth)
        {
            for (int i = 0; i < 8; i++)
            {
                DrawOctree(octree.GetChild(i), depth + 1);
            }
        }
        else
        {
            Gizmos.DrawWireCube(octree.center, Vector3.one * octree.size);
        }
    }
}
