using UnityEngine;
using System;
using System.Collections;

[Serializable]
public struct Octree
{
    public Vector3 center;
    public float extend;

    public Vector3 min { get { return center - new Vector3(extend, extend, extend); } }
    public Vector3 max { get { return center + new Vector3(extend, extend, extend); } }

    public float size { get { return extend * 2; } }

    public Octree GetChild(int index)
    {
        Vector3 center = GetChildCenter(index);
        return new Octree() { center = center, extend = extend * 0.5f };
    }

    public Octree Raycast(Ray ray, Predicate<Octree> pred)
    {
        return new Octree();
    }

    private Vector3 GetChildCenter(int index)
    {
        int z = index % 2;
        int y = index / 2 % 2;
        int x = index / 4;
         
        return center + Vector3.one * -extend * 0.5f  + new Vector3(extend * x, extend * y, extend * z);
    }


    public bool Raycast(Ray r, out float distance)
    {
        Vector3 min = this.min;
        Vector3 max = this.max;

        float tmin = float.NegativeInfinity, tmax = float.PositiveInfinity;

        Vector3 dir_inv = new Vector3();
        dir_inv.x = r.direction.x == 0 ? float.PositiveInfinity : 1 / r.direction.x;
        dir_inv.y = r.direction.y == 0 ? float.PositiveInfinity : 1 / r.direction.y;
        dir_inv.z = r.direction.z == 0 ? float.PositiveInfinity : 1 / r.direction.z;

        distance = 0;

        for (int i = 0; i < 3; ++i)
        {
            float t1 = (min[i] - r.origin[i]) * dir_inv[i];
            float t2 = (max[i] - r.origin[i]) * dir_inv[i];

            tmin = Mathf.Max(tmin, Mathf.Min(t1, t2));
            tmax = Mathf.Min(tmax, Mathf.Max(t1, t2));

            distance = tmin * tmin;
        }
        distance = Mathf.Sqrt(distance);
        return tmax > Mathf.Max(tmin, 0.0f);
    }

    public void Traverse(System.Action<Octree> action, int maxDepth, int depth = 0)
    {
        if (action != null)
        {
            action(this);
        }

        if (depth < maxDepth)
        {
            for (int i = 0; i < 8; i++)
            {
                this.GetChild(i).Traverse(action, maxDepth, depth + 1);
            }
        }
    }
}
