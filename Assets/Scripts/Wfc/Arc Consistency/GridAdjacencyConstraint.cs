using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridAdjacencyConstraint : IEquatable<GridAdjacencyConstraint>
{
    public int valueA;
    public int valueB;
    public Directions3D directionAtoB;

    public GridAdjacencyConstraint(int valueA, int valueB, Directions3D directionAtoB)
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

        return Equals(obj as GridAdjacencyConstraint);

    }

    public bool Equals(GridAdjacencyConstraint other)
    {
        return this.valueA == other.valueA && this.valueB == other.valueB && this.directionAtoB == other.directionAtoB;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return this.valueA ^ this.valueB ^ (int)this.directionAtoB;
    }

    public static bool operator ==(GridAdjacencyConstraint optionsA, GridAdjacencyConstraint optionsB)
    {
        return optionsA.Equals(optionsB);
    }

    public static bool operator !=(GridAdjacencyConstraint optionsA, GridAdjacencyConstraint optionsB)
    {
        return !optionsA.Equals(optionsB);
    }

    public bool isSupported(int valueToCheck, Vector3Int position, Vector3Int dimensions, List<int>[,,] allVariables)
    {
        if (valueToCheck != valueA)
        {
            return true;
        }

        List<int> variableToSearch = getVariableInConstraintDirection(position, dimensions, allVariables);

        if (variableToSearch == null)
        {
            return true;
        }

        foreach (int value in variableToSearch)
        {
            if (value == valueB)
            {
                return true;
            }
        }

        return false;
    }

    private List<int> getVariableInConstraintDirection(Vector3Int origin, Vector3Int dimensions, List<int>[,,] allVariables)
    {
        Vector3Int resultPosition = origin;
        switch (directionAtoB)
        {
            case Directions3D.UP:
                resultPosition += SolverUtils.DirectionsToVectors[Directions3D.UP];
                break;
            case Directions3D.DOWN:
                resultPosition += SolverUtils.DirectionsToVectors[Directions3D.DOWN];
                break;
            case Directions3D.FORWARD:
                resultPosition += SolverUtils.DirectionsToVectors[Directions3D.FORWARD];
                break;
            case Directions3D.RIGHT:
                resultPosition += SolverUtils.DirectionsToVectors[Directions3D.RIGHT];
                break;
            case Directions3D.BACK:
                resultPosition += SolverUtils.DirectionsToVectors[Directions3D.BACK];
                break;
            case Directions3D.LEFT:
                resultPosition += SolverUtils.DirectionsToVectors[Directions3D.LEFT];
                break;
        }

        if (SolverUtils.isInBounds(dimensions, resultPosition))
        {
            return allVariables[resultPosition.x, resultPosition.y, resultPosition.z];
        }
        else
        {
            return null;
        }
    }
}

