using System.Collections.Generic;

public class WfcTest {

    readonly GridAdjacencyConstraint[] testConstraints = 
    {
        new GridAdjacencyConstraint(1, 2, Direction.NORTH),
        new GridAdjacencyConstraint(3, 1, Direction.EAST)
    };

    List<int>[,] variables = 
    {
        {var(1,2,3,4), var(1,2,3,4), var(1,2,3,4)},
        {var(1,2,3,4), var(1,2,3,4), var(1,2,3,4)},
        {var(1,2,3,4), var(1,2,3,4), var(1,2,3,4)}
    };

    static List<int> var(params int[] values) {
        return new List<int>(values);
    }
}