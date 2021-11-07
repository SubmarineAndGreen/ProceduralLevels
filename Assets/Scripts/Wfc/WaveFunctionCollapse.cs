using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Priority_Queue;

public class WaveFunctionCollapse : MonoBehaviour {
    public int tries;
    public static bool logging = false;
    public bool buildGridOnFailedAttempt = true;
    public bool loadHandPlacedTiles = false;
    public static string logsPath;
    [HideInInspector] public string modelFile;
    public TileGrid outputGrid;
    public TileSet tileSet;
    [HideInInspector] public SimpleTiledModel model;
    //number of tile rotations included in model for each  unique tile <index, rotations>
    [HideInInspector] public Dictionary<int, int> uniqueTileCounts;
    //additional frequency wieghts based on number of rotations of a tile in a model
    private List<int> frequencyRotationWeights;
    //frequencyHints per tile/rotation pair in model taking rotationWeights in account
    private List<int> frequencyHints;

    private void Start() {
        const int seed = 1234;
        UnityEngine.Random.InitState(seed);
    }

    public bool run() {
        logsPath = $"{Application.dataPath}/logs.txt";
        File.WriteAllText(logsPath, String.Empty);

        WfcState state = new WfcState() {
            model = this.model,
            grid = new Grid3D<Cell>(outputGrid.dimensions),
            remainingUncollapsedCells = outputGrid.dimensions.x * outputGrid.dimensions.y * outputGrid.dimensions.z,
            entropyQueue = new SimplePriorityQueue<Cell, float>(),
            tileRemovals = new Stack<RemovalUpdate>()
        };


                state.grid.forEach((position, tile) => {
            Cell cell = new Cell(model);
            cell.position = position;
            if (loadHandPlacedTiles) {
                int tileIndex = outputGrid.tileIndices.at(position);
                int tileRotation = outputGrid.tileRotations.at(position);

                if (tileIndex != TileGrid.TILE_EMPTY) {
                    int modelIndex = SimpleTiledModel.tileIndexToModelIndex(tileIndex, tileRotation);
                    for (int i = 0; i < cell.possibleTiles.Count; i++) {
                        if (model.tileModelIndices[i] == modelIndex) {
                            cell.possibleTiles[i] = true;
                        } else {
                            cell.possibleTiles[i] = false;
                            state.tileRemovals.Push(new RemovalUpdate() {
                                tileIndex = model.tileModelIndices[i],
                                tilePosition = position
                            });
                        }
                    }
                    cell.isCollapsed = true;
                    state.remainingUncollapsedCells -= 1;
                }
            }
            state.grid.set(position, cell);

            if (!cell.isCollapsed) {
                state.entropyQueue.Enqueue(cell, cell.entropy());
            }
        });

        if(loadHandPlacedTiles) {
            state.propagate();
        }

        log(state.grid);
        if (!state.run() && !buildGridOnFailedAttempt) {
            return false;
        }

        Grid3D<int> tileIndices = new Grid3D<int>(outputGrid.dimensions);
        Grid3D<int> tileRotations = new Grid3D<int>(outputGrid.dimensions);

        state.grid.forEach((position, cell) => {
            int tile = TileGrid.TILE_EMPTY, rotation = TileGrid.NO_ROTATION;
            for (int i = 0; i < cell.possibleTiles.Count; i++) {
                bool possible = cell.possibleTiles[i];
                if (possible) {
                    tile = SimpleTiledModel.modelIndexToTileIndex(model.tileModelIndices[i]);
                    rotation = SimpleTiledModel.modelIndexToRotation(model.tileModelIndices[i]);
                }
            }
            tileIndices.set(position, tile);
            tileRotations.set(position, rotation);
        });

        outputGrid.tileIndices = tileIndices;
        outputGrid.tileRotations = tileRotations;
        outputGrid.rebuildGrid();

        return true;
    }

    public void run(int tries) {
        if (tries > 1) {
            logging = false;
        }
        while (tries > 0) {
            // Debug.Log("aaa");
            if (run()) {
                break;
            }
            tries--;
        }
        logging = true;
    }


    class Cell {
        public Vector3Int position;
        public List<bool> possibleTiles;
        public SimpleTiledModel model;
        public float sumOfPossibleTileWeights;
        public float sumOfPossibleTileWeightTimesLogWeights;
        public bool isCollapsed = false;
        public Dictionary<int, TileSupportCount> tileSupportCounts;


        public Cell(SimpleTiledModel model) {
            this.model = model;

            possibleTiles = new List<bool>();
            for (int i = 0; i < model.tileModelIndices.Count; i++) {
                possibleTiles.Add(true);
            }

            sumOfPossibleTileWeights = totalPossibleTileFrequency();

            sumOfPossibleTileWeightTimesLogWeights = 0;
            for (int i = 0; i < possibleTiles.Count; i++) {
                float weight = model.frequencyHints[i];
                sumOfPossibleTileWeightTimesLogWeights += weight * Mathf.Log(weight, 2);
            }

            this.tileSupportCounts = initialTileSupportCounts();
        }

        public Cell() {
        }

        public Dictionary<int, TileSupportCount> initialTileSupportCounts() {
            Dictionary<int, TileSupportCount> result = new Dictionary<int, TileSupportCount>();

            foreach (int tileModelIndex in model.tileModelIndices) {
                TileSupportCount counts = new TileSupportCount() { supportByDirection = new int[SolverUtils.nOfDirections] };

                foreach (Directions3D direction in SolverUtils.allDirections) {
                    foreach (TileRule rule in model.rules) {
                        //for each tile that may appear in direction of tileModelIndex
                        //add one to supports count

                        if (rule.valueA == tileModelIndex && rule.directionAtoB == direction) {
                            counts.supportByDirection[(int)direction] += 1;
                        }
                    }
                    // for (int i = 0; i < counts.supportByDirection.Length; i++) {
                    //     int count = counts.supportByDirection[i];
                    // }
                }
                for (int i = 0; i < counts.supportByDirection.Length; i++) {
                    int c = counts.supportByDirection[i];
                    if (c == 0) {
                        //no rules for that direction
                        //if there were no rules for some direction such tile couldnt be placed anywhere
                        //this breaks 2d grids among other things
                        counts.supportByDirection[i] = Int32.MaxValue;
                    }
                }
                result.Add(tileModelIndex, counts);
            }

            return result;
        }


        public void removeTile(int tileIndex) {
            int arrayIndex = -1;
            for (int i = 0; i < model.tileModelIndices.Count; i++) {
                int modelIndex = model.tileModelIndices[i];
                if (modelIndex == tileIndex) {
                    arrayIndex = i;
                    break;
                }
            }
            possibleTiles[arrayIndex] = false;
            float freq = model.frequencyHints[arrayIndex];
            sumOfPossibleTileWeights -= freq;
            sumOfPossibleTileWeightTimesLogWeights -= freq * Mathf.Log(freq, 2);
        }

        public float totalPossibleTileFrequency() {
            float total = 0;
            for (int i = 0; i < possibleTiles.Count; i++) {
                int tileIndex = model.tileModelIndices[i];
                bool possible = possibleTiles[i];
                if (possible) {
                    total += model.frequencyHints[i];
                }
            }
            return total;
        }

        //formula for entropy: E = - P(x1)*log(P(x1)) - P(x2)*log(P(x2)) - ... - P(xn)*log(P(xn))
        //for tiles with weights w1 ... wn, P(tile_n) = wn / (w1 + w2 + ... + wn)
        //this gives us
        // E = log(W) - (w1*log(w1) + w2*log(w2) + ... + wn*log(wn)) / W

        // public float entropy() {
        //     float totalWeight = totalPossibleTileFrequency();
        //     float sumOfWeightTimesLogWeight = 0;
        //     for(int i = 0; i < possibleTile.Count; i++) {
        //         if(possibleTile[i]) {
        //             float weight = model.frequencyHints[i];
        //             sumOfWeightTimesLogWeight += weight * Mathf.Log(weight, 2);
        //         }
        //     }

        //     return Mathf.Log(totalWeight, 2) - (sumOfWeightTimesLogWeight / totalWeight);
        // }

        //with caching for 'sum of weights' and 'sum of weight * log2 weight'
        public float entropy() {
            return Mathf.Log(sumOfPossibleTileWeights, 2)
             - (sumOfPossibleTileWeightTimesLogWeights / sumOfPossibleTileWeights);
        }
        // |   0   |   4   |   5   | 6  |
        //   <-- remaining  --> ^
        //                      |   width of one segment = weight of that tile index
        //                      |
        public int chooseTileIndex() {
            float remaining = UnityEngine.Random.Range(0, sumOfPossibleTileWeights);

            List<int> possibleTileIndices = new List<int>();
            for (int i = 0; i < possibleTiles.Count; i++) {
                if (possibleTiles[i]) {
                    possibleTileIndices.Add(i);
                }
            }

            for (int i = 0; i < possibleTileIndices.Count; i++) {
                float weight = model.frequencyHints[possibleTileIndices[i]];
                if (remaining >= weight) {
                    remaining -= weight;
                } else {
                    return model.tileModelIndices[possibleTileIndices[i]];
                }
            }

            // for (int i = 0; i < possibleTiles.Count; i++) {
            //     float weight = model.frequencyHints[i];
            //     if (remaining >= weight) {
            //         remaining -= weight;
            //     } else {
            //         return model.tileModelIndices[i];
            //     }
            // }

            throw new Exception("sumOfPossibleWeights was inconsistent with possible tiles and their frequencies");
        }
    }

    class WfcState {
        public SimpleTiledModel model;
        public Grid3D<Cell> grid;
        public int remainingUncollapsedCells;
        public SimplePriorityQueue<Cell, float> entropyQueue;
        // public SortedList<float, Cell> entropyList;
        public Stack<RemovalUpdate> tileRemovals;

        public Vector3Int chooseNextCell() {
            while (entropyQueue.Count > 0) {
                Cell cell = entropyQueue.Dequeue();
                if (!cell.isCollapsed) {
                    return cell.position;
                }
            }

            throw new Exception("entropyList is empty, but there are still uncollapsed cells");
        }

        public void collapseCellAt(Vector3Int position) {
            Cell cell = grid.at(position);
            int indexToLockIn = cell.chooseTileIndex();
            cell.isCollapsed = true;

            for (int i = 0; i < cell.possibleTiles.Count; i++) {
                if (model.tileModelIndices[i] != indexToLockIn) {
                    if(cell.possibleTiles[i] == false) {
                        continue;
                    }
                    cell.possibleTiles[i] = false;

                    tileRemovals.Push(new RemovalUpdate() {
                        tileIndex = model.tileModelIndices[i],
                        tilePosition = position
                    });
                }
            }
        }

        // false means contradiction / abort wfc
        public bool propagate() {
            while (tileRemovals.Count > 0) {
                RemovalUpdate removalUpdate = tileRemovals.Pop();

                //propagate the effect of removeal to neighbour in each direction
                foreach (Directions3D direction in SolverUtils.allDirections) {
                    Vector3Int neighbourPosition = removalUpdate.tilePosition + SolverUtils.DirectionsToVectors[direction];
                    if (!SolverUtils.isInBounds(grid.dimensions, neighbourPosition)) {
                        continue;
                    }
                    Cell neighbourCell = grid.at(neighbourPosition);
                    if(neighbourCell.isCollapsed) {
                        continue;
                    }
                    foreach (TileRule rule in model.rules) {
                        if (rule.valueA != removalUpdate.tileIndex || rule.directionAtoB != direction) {
                            continue;
                        }

                        Directions3D oppositeDirection = SolverUtils.oppositeDirections[direction];
                        TileSupportCount supportCount = neighbourCell.tileSupportCounts[rule.valueB];

                        //if the tile we removed was the last tile supporting some tile in neighbour
                        if (supportCount.supportByDirection[(int)oppositeDirection] == 1) {
                            //if there is no support in some direction this tile has already been removed
                            bool containsAnyZeroCounts = false;
                            foreach (int count in supportCount.supportByDirection) {
                                if (count <= 0) {
                                    containsAnyZeroCounts = true;
                                    break;
                                }
                            }
                            //if tile hasnt been removed yet
                            if (!containsAnyZeroCounts) {

                                neighbourCell.removeTile(rule.valueB);
                                WaveFunctionCollapse.log(grid, $"{rule.valueB} removed by propagation: {neighbourPosition} by rule {rule.valueA}, {rule.valueB}, {rule.directionAtoB}, original removal: {removalUpdate.tilePosition}, {removalUpdate.tileIndex}");
                                bool cellHasPossibleTiles = false;
                                neighbourCell.possibleTiles.ForEach((possible) => {
                                    cellHasPossibleTiles |= possible;
                                });
                                if (!cellHasPossibleTiles) {
                                    log(grid, "contradiction");
                                    // CONTRADICTION!! restart wfc
                                    return false;
                                }

                                entropyQueue.Enqueue(neighbourCell, neighbourCell.entropy());
                                tileRemovals.Push(new RemovalUpdate() {
                                    tileIndex = rule.valueB,
                                    tilePosition = neighbourPosition
                                });
                            }
                        }

                        supportCount.supportByDirection[(int)oppositeDirection] -= 1;
                    }
                }
            }
            return true;
        }

        public bool run() {
            while (remainingUncollapsedCells > 0) {
                Vector3Int nextPosition = chooseNextCell();
                collapseCellAt(nextPosition);
                WaveFunctionCollapse.log(grid, $"locked cell {nextPosition}");
                if (!propagate()) {
                    Debug.Log("Contradiction!");
                    return false;
                }
                remainingUncollapsedCells -= 1;
            }
            Debug.Log("Success");
            return true;
        }
    }

    struct RemovalUpdate {
        public int tileIndex;
        public Vector3Int tilePosition;
    }

    class TileSupportCount {
        public int[] supportByDirection;
    }

    private static string debugString(Grid3D<Cell> grid, string note = "") {
        string res = $"\n{note}\n";
        Vector3Int dims = grid.dimensions;
        for (int x = 0; x < dims.x; x++) {
            for (int z = 0; z < dims.z; z++) {
                Cell cell = grid.at(x, 0, z);
                res += "[";
                for (int i = 0; i < cell.possibleTiles.Count; i++) {
                    if (cell.possibleTiles[i]) {
                        res += cell.model.tileModelIndices[i] + ",";
                    } else {
                        res += "X,";
                    }
                }
                res += "]";
            }
            res += "\n";
        }
        return res;
    }

    private static void log(Grid3D<Cell> grid, string note = "") {
        if (!logging) return;
        File.AppendAllText(logsPath, debugString(grid, note));
    }
}




