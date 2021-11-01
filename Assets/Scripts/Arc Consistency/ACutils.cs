using System;
using System.Collections.Generic;

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

    public bool isSupported(int valueToCheck, int variableX, int variableY, List<int>[,] allVariables)
    {
        if (valueToCheck != valueA)
        {
            return true;
        }

        List<int> variableToSearch = getVariableInConstraintDirection(variableX, variableY, allVariables);
        
        if(variableToSearch == null) {
            return true;
        }

        foreach(int value in variableToSearch) {
            if(value == valueB) {
                return true;
            }
        }

        return false;
    }

    private List<int> getVariableInConstraintDirection(int variableX, int variableY, List<int>[,] allVariables)
    {
        int newX = variableX, newY = variableY;
        switch (directionAtoB)
        {
            case Direction.NORTH:
                newX -= 1;
                break;
            case Direction.EAST:
                newY += 1;
                break;
            case Direction.SOUTH:
                newX += 1;
                break;
            case Direction.WEST:
                newY -= 1;
                break;
        }

        if(isInBounds(newX, newY)) {
            return allVariables[newX, newY];
        } else {
            return null;
        }

        bool isInBounds(int x, int y)
        {
            if (x < 0 || x >= allVariables.GetLength(0))
            {
                return false;
            }
            if (y < 0 || y >= allVariables.GetLength(1))
            {
                return false;
            }

            return true;
        }
    }
}