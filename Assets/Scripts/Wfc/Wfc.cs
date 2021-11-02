using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wfc : MonoBehaviour
{
    [HideInInspector] public string modelFile;
    [SerializeField] private TileGrid inputGrid;
    [SerializeField] private TileGrid outputGrid;
    [SerializeField] private TileSet tileSet;

    [SerializeField] ConstraintSolver solver;

    enum ConstraintSolver
    {
        AC1
    }

    Dictionary<ConstraintSolver, IConstraintSolver> constraintSolvers = new Dictionary<ConstraintSolver, IConstraintSolver>()
    {
        {ConstraintSolver.AC1, new AC1()}
    };

    void run()
    {

    }

    private void Start() {

    }
}




