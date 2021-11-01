using System;
using System.Collections.Generic;
using UnityEngine;

public class GridAdjacencyConstraint
{
    int valueA;
    int valueB;
    Directions3D directionAtoB;

    public GridAdjacencyConstraint(int valueA, int valueB, Directions3D directionAtoB)
    {
        this.valueA = valueA;
        this.valueB = valueB;
        this.directionAtoB = directionAtoB;
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
                resultPosition += ACUtils.DirectionsToVectors[Directions3D.UP];
                break;
            case Directions3D.DOWN:
                resultPosition += ACUtils.DirectionsToVectors[Directions3D.DOWN];
                break;
            case Directions3D.FORWARD:
                resultPosition += ACUtils.DirectionsToVectors[Directions3D.FORWARD];
                break;
            case Directions3D.RIGHT:
                resultPosition += ACUtils.DirectionsToVectors[Directions3D.RIGHT];
                break;
            case Directions3D.BACK:
                resultPosition += ACUtils.DirectionsToVectors[Directions3D.BACK];
                break;
            case Directions3D.LEFT:
                resultPosition += ACUtils.DirectionsToVectors[Directions3D.LEFT];
                break;
        }

        if (ACUtils.isInBounds(dimensions, resultPosition))
        {
            return allVariables[resultPosition.x, resultPosition.y, resultPosition.z];
        }
        else
        {
            return null;
        }
    }
}