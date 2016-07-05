using UnityEngine;
using System.Collections;

[System.Serializable]
public class VoxelData
{
    [Range(1, 16), SerializeField]
    int _resolution;

    public int resolution
    {
        get
        {
            return _resolution;
        }
        set
        {
            if (value > _resolution)
            {
                _data = null;
            }

            value = Mathf.Clamp(value, 1, 16);
            _resolution = value;
        }
    }

    public byte[,,] _data;
    public byte[,,] data
    {
        get
        {
            if (_data == null)
            {
                _data = new byte[resolution, resolution, resolution];
                for (int x = 0; x < resolution; x++)
                {
                    for (int y = 0; y < resolution; y++)
                    {
                        for (int z = 0; z < resolution; z++)
                        {
                            _data[x, y, z] = 1;
                        }
                    }
                }
            }
            return _data;
        }
    }

    public int width { get { return resolution; } }
    public int height { get { return resolution; } }
    public int depth { get { return resolution; } }

    public byte this[int x,int y,int z]
    {
        get
        {
            if (x < 0 
                || y < 0 
                || z < 0
                || x >= width 
                || y >= height 
                || z >= depth)
            {
                return 0;
            }

            return data[x, y, z];
        }

        set
        {
            if (x < 0 
                || y < 0 
                || z < 0 
                || x >= width 
                || y >= height 
                || z >= depth)
            {
                return;
            }

            data[x, y, z] = value;
            if (dataChanged != null)
            {
                dataChanged();
            }
        }
    }

    public event System.Action dataChanged;
}
