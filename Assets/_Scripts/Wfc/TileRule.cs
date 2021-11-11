using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileRule : IEquatable<TileRule>
{
    public int valueA;
    public int valueB;
    public Directions3D directionAtoB;

    public TileRule(int valueA, int valueB, Directions3D directionAtoB)
    {
        this.valueA = valueA;
        this.valueB = valueB;
        this.directionAtoB = directionAtoB;
    }


    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Equals(obj as TileRule);

    }

    public bool Equals(TileRule other)
    {
        return this.valueA == other.valueA && this.valueB == other.valueB && this.directionAtoB == other.directionAtoB;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return this.valueA ^ this.valueB ^ (int)this.directionAtoB;
    }

    public static bool operator ==(TileRule optionsA, TileRule optionsB)
    {
        return optionsA.Equals(optionsB);
    }

    public static bool operator !=(TileRule optionsA, TileRule optionsB)
    {
        return !optionsA.Equals(optionsB);
    }
}

