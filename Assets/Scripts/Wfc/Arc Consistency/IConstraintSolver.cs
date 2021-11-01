using System.Collections.Generic;
using UnityEngine;

public interface IConstraintSolver {
    void run(Vector3Int dimensions, List<int>[,,] variables, GridAdjacencyConstraint[] constraints);
}