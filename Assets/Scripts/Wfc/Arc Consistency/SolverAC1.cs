using System;
using System.Collections.Generic;
using UnityEngine;

public class SolverAC1 {
    public bool constrainVariable(Array3D<Variable> variables, Vector3Int position, List<TileRule> rules) {
        bool removedAny = false;
        Variable variable = variables.at(position);

        for (int i = 0; i < variable.domain.Count; i++) {
            int tileIndex = variable.domain[i];

            foreach (Directions3D direction in Enum.GetValues(typeof(Directions3D))) {
                bool isSupportedFromDirection = false;
                //dont fail when there is no rules for some direction 
                //example: 2d roads have no rules for whats allowed upwards/downwards
                bool noRulesForDirection = true;
                foreach (TileRule rule in rules) {
                    if (rule.directionAtoB == direction) {
                        noRulesForDirection = false;
                        if (isSupported(tileIndex, position, variables, rule)) {
                            isSupportedFromDirection = true;
                            // Debug.Log($"supporting rule: A:{rule.valueA}, B:{rule.valueB}, dir:{rule.directionAtoB.ToString()}");
                        } else {
                            // Debug.Log($"contradicting rule: A:{rule.valueA}, B:{rule.valueB}, dir:{rule.directionAtoB.ToString()}");
                        }
                    }
                }

                if (!isSupportedFromDirection && !noRulesForDirection) {
                    Debug.Log(tileIndex + " failing direction:" + direction);
                    variable.domain.Remove(tileIndex);
                    removedAny = true;
                }
            }
        }

        return removedAny;
    }

    public bool run(Array3D<Variable> variables, List<TileRule> rules) {
        bool success = true;
        bool removedAnyValues;

        void contstrainAllVariables() {
            for (int x = 0; x < variables.dimensions.x; x++) {
                for (int y = 0; y < variables.dimensions.y; y++) {
                    for (int z = 0; z < variables.dimensions.z; z++) {
                        removedAnyValues = constrainVariable(variables, new Vector3Int(x, y, z), rules);
                        if (variables.at(x, y, z).domain.Count == 0) {
                            success = false;
                            return;
                        }
                    }
                }
            }
        }

        while (true) {
            removedAnyValues = true;

            while (removedAnyValues) {
                contstrainAllVariables();
            }

            if(!removedAnyValues) {
                break;
            }

            if (!success) {
                break;
            }
        }

        return success;
    }



    public bool isSupported(int value, Vector3Int tilePosition, Array3D<Variable> variables, TileRule rule) {
        if (rule.valueA != value) {
            return false;
        }

        if (!variables.inBounds(tilePosition + SolverUtils.DirectionsToVectors[rule.directionAtoB])) {
            return true;
        }

        if (variables.at(tilePosition + SolverUtils.DirectionsToVectors[rule.directionAtoB]).domain.Contains(rule.valueB)) {
            return true;
        }

        return false;
    }
}