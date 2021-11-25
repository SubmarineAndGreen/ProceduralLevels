
using System;

[Serializable]
public class connectionData {
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
                connections[(((int)dir - 2 + rotation) % 4) + 2] = false;
                break;
        }
    }

    public bool canConnectFromDirection(Directions3D dir, int rotation) {
        if (dir == Directions3D.UP || dir == Directions3D.DOWN) {
            return connections[(int)dir];
        } else {
            return connections[(((int)dir - 2 + rotation) % 4) + 2];
        }
    }
}

