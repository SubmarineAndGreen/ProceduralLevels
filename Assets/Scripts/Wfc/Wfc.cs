using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wfc : MonoBehaviour
{
    [HideInInspector] public string modelFile;
    // [SerializeField] private TileGrid inputGrid;
    [SerializeField] private TileGrid outputGrid;
    [SerializeField] private ConstraintSolvers solverName;
    [SerializeField] private int numberOfTries;

    enum ConstraintSolvers
    {
        AC1
    }

    Dictionary<ConstraintSolvers, IConstraintSolver> constraintSolvers = new Dictionary<ConstraintSolvers, IConstraintSolver>()
    {
        {ConstraintSolvers.AC1, new AC1()}
    };

    public int[,,] run()
    {
        WfcModel model = WfcModel.loadFromFile(modelFile);
        IConstraintSolver solver = constraintSolvers[solverName];
        int currentTry = 0;
        Vector3Int dimensions = outputGrid.dimensions;

        List<int>[,,] variables = new List<int>[dimensions.x, dimensions.y, dimensions.z];
        Stack<Vector3Int> scanLineOrderStack = new Stack<Vector3Int>();

        initializeVariables();

        Vector3Int positionToCollapse = Vector3Int.zero;
        while (true)
        {
            currentTry++;
            if (currentTry > numberOfTries)
            {
                Debug.Log("Reached max tries");
                return null;
            }

            initializeDataStructures();

            bool success = false;
            while (true)
            {

                pickTile();
                if (scanLineOrderStack.Count == 0)
                {
                    success = true;
                    break;
                }

                collapseVariable(positionToCollapse);

                if (!solver.run(outputGrid.dimensions, variables, model.constraints))
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                int[,,] ret = new int[dimensions.x, dimensions.y, dimensions.z];

                for (int x = 0; x < dimensions.x; x++)
                {
                    for (int y = 0; y < dimensions.y; y++)
                    {
                        for (int z = 0; z < dimensions.z; z++)
                        {
                            ret[x, y, z] = variables[x, y, z][0];
                        }
                    }
                }

                Debug.Log("Success");
                return ret;
            }
            else
            {
                Debug.Log("try: " + currentTry);
            }
        }

        void initializeVariables()
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int z = 0; z < dimensions.z; z++)
                    {

                        for (int i = 0; i < model.tileIds.Count; i++)
                        {
                            variables[x, y, z] = new List<int>();
                            variables[x, y, z].Add(model.tileIds[i]);
                        }
                    }
                }
            }
        }

        void initializeDataStructures()
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int z = 0; z < dimensions.z; z++)
                    {
                        scanLineOrderStack.Push(new Vector3Int(x, y, z));
                    }
                }
            }
        }


        void pickTile()
        {
            while (scanLineOrderStack.Count != 0)
            {
                positionToCollapse = scanLineOrderStack.Pop();
                if (variables[positionToCollapse.x, positionToCollapse.y, positionToCollapse.z].Count != 1)
                {
                    break;
                }
            }
        }

        void collapseVariable(Vector3Int tilePosition)
        {
            int collapsedValueIndex = UnityEngine.Random.Range(0, model.tileIds.Count - 1);
            //TODO: dont use linq
            variables[tilePosition.x, tilePosition.y, tilePosition.z] =
                variables[tilePosition.x, tilePosition.y, tilePosition.z].Where((val, index) =>
                    index == collapsedValueIndex
                ).ToList();
        }
    }


}




