using System.Collections.Generic;
using UnityEngine;

public interface IConstraintSolver {
    bool run(Vector3Int dimensions, List<int>[,,] variables, List<GridAdjacencyConstraint> constraints);
}