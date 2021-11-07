using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour {
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

    public void wfc() {
        WfcState wfc = new WfcState() {
            model = this.model,
            grid = new Grid3D<Cell>(outputGrid.dimensions),
            remainingUncollapsedCells = outputGrid.dimensions.x * outputGrid.dimensions.y * outputGrid.dimensions.z,
            entropyList = new SortedList<float, Cell>(),
            tileRemovals = new Stack<RemovalUpdate>()
        };

        wfc.grid.forEach(position => {
            Cell cell = new Cell(model);
            cell.position = position;
            cell.entropyNoise = UnityEngine.Random.Range(0, 0.001f) + UnityEngine.Random.Range(0, 0.0001f);
            wfc.grid.set(position, cell);
            wfc.entropyList.Add(cell.entropy(), cell);
        });

        wfc.run();

        Grid3D<int> tileIndices = new Grid3D<int>(outputGrid.dimensions);
        Grid3D<int> tileRotations = new Grid3D<int>(outputGrid.dimensions);

        wfc.grid.forEach((position, cell) => {
            int tile = TileGrid.TILE_EMPTY, rotation = TileGrid.NO_ROTATION;
            for (int i = 0; i < cell.possibleTiles.Count; i++) {
                bool possible = cell.possibleTiles[i];
                if(possible) {
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
    }

    class Cell {
        public Vector3Int position;
        public List<bool> possibleTiles;
        public SimpleTiledModel model;
        public float sumOfPossibleTileWeights;
        public float sumOfPossibleTileWeightTimesLogWeights;
        //introduce noise to make sure that at no point two tiles have equal entropy, for resolving draws
        //when picking tile with lowest entropy (sorted list cant have 2 same keys)
        public float entropyNoise;
        public bool isCollapsed = false;
        public List<TileSupportCount> tileSupportCounts;



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

            entropyNoise = UnityEngine.Random.Range(0, 0.00001f) + UnityEngine.Random.Range(0, 0.0001f);
        }

        public Cell() {
        }

        public List<TileSupportCount> initialTileSupportCounts() {
            List<TileSupportCount> result = new List<TileSupportCount>();

            foreach (int tileModelIndex in model.tileModelIndices) {
                TileSupportCount counts = new TileSupportCount() { supportByDirection = new int[SolverUtils.nOfDirections] };

                foreach (Directions3D direction in SolverUtils.allDirections) {
                    foreach (TileRule rule in model.rules) {
                        //for each tile that may appear in direction of tileModelIndex
                        //add one to supports count
                        if (rule.valueA == tileModelIndex && rule.directionAtoB == direction) {
                            // counts[(int)direction] += 1; 
                            counts.supportByDirection[(int)direction] += 1;
                        }
                    }
                }
                result.Add(counts);
            }

            return result;
        }


        public void removeTile(int tileIndex) {
            possibleTiles[tileIndex] = false;
            float freq = model.frequencyHints[tileIndex];
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
             - (sumOfPossibleTileWeightTimesLogWeights / sumOfPossibleTileWeights) + entropyNoise;
        }
        // |   0   |   4   |   5   | 6  |
        //   <-- remaining  --> ^
        //                      |   width of one segment = weight of that tile index
        //                      |
        public int chooseTileIndex() {
            float remaining = UnityEngine.Random.Range(0, sumOfPossibleTileWeights);

            for (int i = 0; i < possibleTiles.Count; i++) {
                float weight = model.frequencyHints[i];
                if (remaining >= weight) {
                    remaining -= weight;
                } else {
                    return model.tileModelIndices[i];
                }
            }

            throw new Exception("sumOfPossibleWeights was inconsistent with possible tiles and their frequencies");
        }
    }

    class WfcState {
        public SimpleTiledModel model;
        public Grid3D<Cell> grid;
        public int remainingUncollapsedCells;
        public SortedList<float, Cell> entropyList;
        public Stack<RemovalUpdate> tileRemovals;

        public Vector3Int chooseNextCell() {
            while (entropyList.Count > 0) {
                Cell cell = entropyList.Values[0];
                entropyList.RemoveAt(0);
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
                    if(!SolverUtils.isInBounds(grid.dimensions, neighbourPosition)) {
                        continue;
                    }
                    Cell neighbourCell = grid.at(neighbourPosition);
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
                                if (count == 0) containsAnyZeroCounts = true;
                            }
                            //if tile hasnt been removed yet
                            if (!containsAnyZeroCounts) {
                                neighbourCell.removeTile(rule.valueB);
                                bool cellHasPossibleTiles = false;
                                neighbourCell.possibleTiles.ForEach((possible) => {
                                    cellHasPossibleTiles |= possible;
                                });
                                if (!cellHasPossibleTiles) {
                                    // CONTRADICTION!! restart wfc
                                    return false;
                                }

                                entropyList.Add(neighbourCell.entropy(), neighbourCell);
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

        public void run() {
            while(remainingUncollapsedCells > 0) {
                Vector3Int nextPosition = chooseNextCell();
                collapseCellAt(nextPosition);
                if(!propagate()) {
                    Debug.Log("Contradiction!");
                    return;
                }
                remainingUncollapsedCells -= 1;
            }
        }
    }

    struct RemovalUpdate {
        public int tileIndex;
        public Vector3Int tilePosition;
    }

    class TileSupportCount {
        public int[] supportByDirection;
    }
}




