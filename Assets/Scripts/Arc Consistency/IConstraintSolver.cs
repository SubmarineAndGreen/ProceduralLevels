using System.Collections.Generic;

public interface IConstraintSolver {
    void run(List<int>[,] variables, GridAdjacencyConstraint[] constraints);
}