using System.Collections.Generic;
using UnityEngine;

public class AC1 : IConstraintSolver
{
    public bool run(Vector3Int dimensions, List<int>[,,] variables, List<GridAdjacencyConstraint> constraints)
    {
        while (true)
        {
            bool wasAnyValueRemoved = false;
            //for each constraint
            foreach (GridAdjacencyConstraint constraint in constraints)
            {
                //for each variable of that constraint
                for (int x = 0; x < dimensions.x; x++)
                {
                    for (int y = 0; y < dimensions.y; y++)
                    {
                        for (int z = 0; z < dimensions.z; z++)
                        {
                            //foreach value in domain of that variable
                            for (int i = 0; i < variables[x, y, z].Count; i++)
                            {
                                int value = variables[x, y, z][i];
                                //search for support of that value on the constraint
                                if (!constraint.isSupported(value, new Vector3Int(x, y, z), dimensions, variables))
                                {
                                    //if no support is found remove value from the domain
                                    variables[x, y, z].Remove(value);
                                    //if there is a variable with no possibilities,
                                    //there is no valid solution, return false
                                    if(variables[x, y, z].Count == 0)
                                    {
                                        return false;
                                    }
                                    wasAnyValueRemoved = true;
                                }
                            }
                        }
                    }
                }
                //if nothing has been removed in this loop, exit
                if (!wasAnyValueRemoved)
                {
                    return true;
                }
            }
        }
    }
}