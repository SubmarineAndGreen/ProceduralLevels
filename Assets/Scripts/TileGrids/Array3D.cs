using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Array3D<T>
{
    private T[,,] _values;

    public Array3D(Vector3Int dimensions)
    {
        _values = new T[dimensions.x, dimensions.y, dimensions.z];
    }

    public Array3D(T[,,] values)
    {
        _values = values;
    }

    public Array3D(Vector3Int dimensions, T[] flatArray)
    {
        _values = new T[dimensions.x, dimensions.y, dimensions.z];
        int xMax = _values.GetLength(0), yMax = _values.GetLength(1);
        this.forEach((x, y, z) => {
            int index = x + y * xMax + z * yMax * xMax;
            _values[x,y,z] = flatArray[index];
        });
    }

    // public Array3D(Array3D<T> source)
    // {
    //     _values = source._values;
    // }

    public Vector3Int dimensions()
    {
        return new Vector3Int(_values.GetLength(0), _values.GetLength(1), _values.GetLength(2));
    }

    public T at(Vector3Int position)
    {
        return _values[position.x, position.y, position.z];
    }

    public void set(Vector3Int position, T newValue)
    {
        _values[position.x, position.y, position.z] = newValue;
    }

    public T at(int x, int y, int z)
    {
        return _values[x, y, z];
    }

    public T[,,] getValueArray() {
        return _values;
    }

    public T[] flatArray() {
        T[] result = new T[_values.GetLength(0) * _values.GetLength(1) * _values.GetLength(2)];

        int xMax = _values.GetLength(0), yMax = _values.GetLength(1);
        this.forEach((x, y, z, value) => {
            int index = x + y * xMax + z * yMax * xMax;
            result[index] = value;
        });

        return result;
    }

    public void setEach(ValueOperation operation)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    _values[x, y, z] = operation(new Vector3Int(x, y, z), _values[x, y, z]);
                }
            }
        }
    }

    public void setEach(ValueOperation2 operation)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    _values[x, y, z] = operation(x, y, z, _values[x, y, z]);
                }
            }
        }
    }

    public void setEach(ValueOperation3 operation)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    _values[x, y, z] = operation(_values[x, y, z]);
                }
            }
        }
    }

    public void forEach(IndexAction action)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    action(x, y, z);
                }
            }
        }
    }

    public void forEach(IndexAction2 action)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    action(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public void forEach(ValueAction action)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    action(new Vector3Int(x, y, z), _values[x, y, z]);
                }
            }
        }
    }

    public void forEach(ValueAction2 action)
    {
        for (int x = 0; x < _values.GetLength(0); x++)
        {
            for (int y = 0; y < _values.GetLength(1); y++)
            {
                for (int z = 0; z < _values.GetLength(2); z++)
                {
                    action(x, y, z, _values[x, y, z]);
                }
            }
        }
    }

    public delegate T ValueOperation(Vector3Int position, T value);
    public delegate T ValueOperation2(int x, int y, int z, T value);
    public delegate T ValueOperation3(T value);
    public delegate void IndexAction(int x, int y, int z);
    public delegate void IndexAction2(Vector3Int position);
    public delegate void ValueAction(Vector3Int position, T value);
    public delegate void ValueAction2(int x, int y, int z, T value);
}
