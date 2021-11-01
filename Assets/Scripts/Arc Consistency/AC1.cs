using System.Collections.Generic;

public class AC1 : IConstraintSolver
{
    public void run(List<int>[,] variables, GridAdjacencyConstraint[] constraints)
    {
        while (true)
        {
            bool wasAnyValueRemoved = false;
            //for each constraint
            foreach (GridAdjacencyConstraint constraint in constraints)
            {
                //for each variable of that constraint
                for (int i = 0; i < variables.GetLength(0); i++)
                {
                    for (int j = 0; j < variables.GetLength(1); j++)
                    {
                        //foreach value in domain of that variable
                        foreach (int value in variables[i, j])
                        {
                            //search for support of that value on the constraint
                            if(!constraint.isSupported(value, i, j, variables)) {
                                //if no support is found remove value from the domain
                                variables[i, j].Remove(value);
                                wasAnyValueRemoved = true;
                            }
                        }
                    }
                }
                //if nothing has been removed in this loop, exit
                if(!wasAnyValueRemoved) {
                    return;
                }
            }
        }
    }
}