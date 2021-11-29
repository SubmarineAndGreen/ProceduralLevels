using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class Navigation {
    const int NEIGHBOURS_COUNT = 6;
    const int NO_VECTOR = -1;

    public static Vector3Int[] directionVectors = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.right,
        Vector3Int.back,
        Vector3Int.left
    };

    public static int[,,][,,] calculateVectorFieldForEachCell(int[,,] inputGrid, int unwalkableWeight) {
        Vector3Int dimensions = new Vector3Int(inputGrid.GetLength(0), inputGrid.GetLength(1), inputGrid.GetLength(2));
        int[,,][,,] vectorFields = new int[dimensions.x, dimensions.y, dimensions.z][,,];

        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                for (int z = 0; z < dimensions.z; z++) {
                    int[,,] distanceField = calculateDijkstraDistanceField(inputGrid, unwalkableWeight, new Vector3Int(x, y, z));
                    vectorFields[x, y, z] = calculateVectorField(distanceField, unwalkableWeight);
                }
            }
        }



        return vectorFields;
    }

    public static int[,,] calculateDijkstraDistanceField(int[,,] inputGrid, int unwalkableWeight, Vector3Int goalCell) {
        Vector3Int dimensions = new Vector3Int(inputGrid.GetLength(0), inputGrid.GetLength(1), inputGrid.GetLength(2));
        //number of steps to reach the starting cell from each cell / manhattan distance(?) to starting cell
        int[,,] distanceField = inputGrid;
        //have we already iterated over neighbours of some cell 
        bool[,,] wasCellVisited = new bool[dimensions.x, dimensions.y, dimensions.z];
        //queue of cells to iterate over neighbours of
        SimplePriorityQueue<Vector3Int, int> cellsToVisit = new SimplePriorityQueue<Vector3Int, int>();

        // CellNode[,,] nodes = new CellNode[dimensions.x, dimensions.y, dimensions.z];
        // for (int x = 0; x < dimensions.x; x++) {
        //     for (int y = 0; y < dimensions.y; y++) {
        //         for (int z = 0; z < dimensions.z; z++) {
        //             nodes[x,y,z] = new CellNode() {
        //                 position = new Vector3Int(x,y,z)
        //             };
        //         }
        //     }
        // }

        // FastPriorityQueue<CellNode> fqueue = new FastPriorityQueue<CellNode>(dimensions.x * dimensions.y * dimensions.z);

        //initialize queue with starting cell with distance 0
        cellsToVisit.Enqueue(goalCell, 0);
        //distance to goal from goal is 0 :^)
        distanceField[goalCell.x, goalCell.y, goalCell.z] = 0;

        while (cellsToVisit.Count != 0) {
            //from queue get cell with lowest distance to goal
            Vector3Int currentCell = cellsToVisit.Dequeue();
            int currentDistance = distanceField[currentCell.x, currentCell.y, currentCell.z];
            wasCellVisited[currentCell.x, currentCell.y, currentCell.z] = true;

            //go through all neighbours, if distance from this cell + step is less than already
            //recorded distance make that the new distance for the neighbouring cell
            for (int i = 0; i < NEIGHBOURS_COUNT; i++) {
                Vector3Int neighbourCell = currentCell + directionVectors[i];
                if (inBounds(neighbourCell, dimensions)) {
                    int neighbourDistance = distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z];
                    //skip neighbours that are blocked/unwalkable/walls or were already visited by algo
                    if (neighbourDistance != unwalkableWeight && !wasCellVisited[neighbourCell.x, neighbourCell.y, neighbourCell.z]) {
                        if (currentDistance + 1 < neighbourDistance) {
                            distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z] = currentDistance + 1;
                        }
                        //add neighbour to queue
                        cellsToVisit.Enqueue(neighbourCell, distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z]);
                    }
                }
            }
        }

        return distanceField;
    }

    public static int[,,] calculateVectorField(int[,,] distanceField, int unwalkableWeight) {
        Vector3Int dimensions = new Vector3Int(distanceField.GetLength(0), distanceField.GetLength(1), distanceField.GetLength(2));
        int[,,] vectorField = new int[dimensions.x, dimensions.y, dimensions.z];

        //iterate through all cells to find neighbour closest to goal in distance field
        //assign to that cell int representing vector from tested cell to that neighbour
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                for (int z = 0; z < dimensions.z; z++) {

                    int resultVector = NO_VECTOR;
                    //min found distance initially set to origin cell / cell for which we are currently finding the vector
                    int minDistance = distanceField[x, y, z];

                    //if cell is blocked ignore / leave NO_VECTOR
                    if (minDistance != unwalkableWeight) {
                        for (int i = 0; i < NEIGHBOURS_COUNT; i++) {
                            Vector3Int neighbourCell = new Vector3Int(x, y, z) + directionVectors[i];

                            if (inBounds(neighbourCell, dimensions)) {
                                int neighbourDistance = distanceField[neighbourCell.x, neighbourCell.y, neighbourCell.z];
                                //if neighbour cell is walkable and distance to goal is less
                                if (neighbourDistance != unwalkableWeight && neighbourDistance < minDistance) {
                                    //record new lowest distance
                                    minDistance = neighbourDistance;
                                    //make vector leading to this cell new result vector
                                    resultVector = i;
                                }
                            }
                        }
                    }

                    vectorField[x, y, z] = resultVector;
                }
            }
        }

        return vectorField;
    }

    private static bool inBounds(Vector3Int coords, Vector3Int bounds) {
        return coords.x >= 0 && coords.y >= 0 && coords.z >= 0 &&
        coords.x < bounds.x && coords.y < bounds.y && coords.z < bounds.z;
    }
}

// public class CellNode : FastPriorityQueueNode {
//     public Vector3Int position;
// }
