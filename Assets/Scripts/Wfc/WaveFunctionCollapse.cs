using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI debugVariablesText;
    [HideInInspector] public string modelFile;
    // [SerializeField] private TileGrid inputGrid;
    public TileGrid outputGrid;
    [SerializeField] private int numberOfTries;
    [HideInInspector] public SolverAC1 solver = new SolverAC1();
    [HideInInspector] public SimpleTiledModel model;
    [HideInInspector] public Grid3D<Variable> variables;

    public void run() {
        initialize();
        bool allCollapsed = false;

        while (!allCollapsed) {
            allCollapsed = true;
            foreach (Variable variable in variables) {
                if(!variable.collapsed)
                {
                    allCollapsed = false;
                    variable.lockValue();
                    break;
                }
            }
            bool ok = solver.run(variables, model.rules);
            if(!ok) 
            {
                Debug.Log("Solver failed!");
                break;
            }
        }

        populateGrid();
    }

    public void populateGrid() {
        Grid3D<int> tileIndices = new Grid3D<int>(outputGrid.dimensions); 
        Grid3D<int> tileRotations = new Grid3D<int>(outputGrid.dimensions); 

        variables.forEach((Vector3Int position, Variable variable) => {

            if(variable.domain.Count == 1) {
                int index = variable.domain[0];
                tileIndices.set(position, SimpleTiledModel.modelIndexToTileIndex(index));
                tileRotations.set(position, SimpleTiledModel.modelIndexToRotation(index));
            } else {
                tileIndices.set(position, TileGrid.TILE_EMPTY);
                tileRotations.set(position, TileGrid.NO_ROTATION);
            }
        });

        outputGrid.tileIndices = tileIndices;
        outputGrid.tileRotations = tileRotations;
        outputGrid.rebuildGrid();
    }

    public void initialize() {
        model = SimpleTiledModel.loadFromFile(modelFile);
        variables = new Grid3D<Variable>(outputGrid.dimensions);
        variables.updateEach(_ => new Variable(model.tileIds));
    }

    private void Update() {
        updateDebugUI();
    }

    public void updateDebugUI() {
        if (variables == null) {
            return;
        }


        const string variablesText = "Possible Tiles: ";

        debugVariablesText.text = variablesText;
        foreach (int value in variables.at(outputGrid.cursorPosition).domain) {
            debugVariablesText.text += value.ToString() + " ";
        }
    }

    //TODO visualistion
}

public class Variable {
    public List<int> domain;
    public bool collapsed => domain.Count == 1;

    public void lockValue() {
        int pick = UnityEngine.Random.Range(0, domain.Count - 1);
        domain = domain.Where(val => val == domain[pick]).ToList();
    }

    public Variable(List<int> tileSet) {
        domain = new List<int>(tileSet);
    }
}





