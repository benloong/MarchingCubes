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
    

    public Octree Raycast(Ray ray, Predicate<Octree> pred)
    {
        return new Octree();
    }

    public Octree GetChild(int index)
    {
        int z = index % 2;
        int y = index / 2 % 2;
        int x = index / 4;

        return GetChild(x, y, z);
    }

    public Octree GetChild(int x, int y, int z)
    {
        return new Octree()
        {
            center = center + Vector3.one * -extend * 0.5f + new Vector3(extend * x, extend * y, extend * z),
            extend = extend * 0.5f
        };
    }

    public bool Raycast(Ray r, out float distance)
    {
        Vector3 min = this.min;
        Vector3 max = this.max;

        float tmin = float.NegativeInfinity, tmax = float.PositiveInfinity;

        Vector3 dir_inv = new Vector3();
        dir_inv.x = 1 / r.direction.x;
        dir_inv.y = 1 / r.direction.y;
        dir_inv.z = 1 / r.direction.z;

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
