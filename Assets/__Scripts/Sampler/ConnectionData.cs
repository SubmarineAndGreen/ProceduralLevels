
using System;
using UnityEngine;

[Serializable]
public class ConnectionData {
    const int directionCount = 6;
    public bool[] connections;

    public void setAllTrue() {
        connections = new bool[directionCount];
        for (int i = 0; i < connections.Length; i++) {
            connections[i] = true;
        }
    }

    public void banConnection(Directions3D dir, int rotation) {
        switch (dir) {
            case Directions3D.UP:
                connections[(int)dir] = false;
                break;
            case Directions3D.DOWN:
                connections[(int)dir] = false;
                break;
            default:
                connections[directionBeforeRotation(dir, rotation)] = false;
                break;
        }
    }

    public bool canConnectFromDirection(Directions3D dir, int rotation) {
        if (dir == Directions3D.UP || dir == Directions3D.DOWN) {
            return connections[(int)dir];
        } else {
            return connections[directionBeforeRotation(dir, rotation)];
        }
    }

    private int directionBeforeRotation(Directions3D dir, int rotation) {
        return MathUtils.mod(((int)dir - 2 - rotation), 4) + 2;
    }
}




