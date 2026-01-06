using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is GridPosition other)
            return x == other.x && y == other.y;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.y + b.y);
    }
}

[Serializable]
public class BuildingSize
{
    public int width;
    public int height;

    public BuildingSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}

public enum ConnectionType
{
    None,
    Input,
    Output
}

public enum BuildingType
{
    Materializer,
    Factory,
    Packager,
    Distribution,
    ConveyorBelt
}

