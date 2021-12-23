using UnityEngine;

[System.Serializable]
public class StructureTile {
    public Vector3Int position;
    public uint openSidesMask;
    public bool walkable;

    public StructureTile()
    {
        walkable = true;
        openSidesMask = 0;
    }

    public void setSideOpen(Directions3D direction, bool isOpen) {
        openSidesMask = setOpenSidesMask(direction, isOpen, openSidesMask);
    }

    public bool isSideOpen(Directions3D direction) {
        return isSideOpenMask(direction, openSidesMask);
    }

    private uint setOpenSidesMask(Directions3D direction, bool isOpen, uint mask) {
        if(isOpen) {
            mask |= 1u << (int)direction;
        } else {
            mask &= ~(1u << (int)direction);
        }

        return mask;
    }

    private bool isSideOpenMask(Directions3D direction, uint mask) {
        return (mask & (1u << (int)direction)) != 0;
    }

    public uint getRotated(int rotation) {
        uint newMask = 0u;
        for(int i = (int)Directions3D.FORWARD; i < DirectionUtils.allDirections.GetLength(0); i++) {
            Directions3D currentSide = DirectionUtils.allDirections[i];
            if(isSideOpenMask(currentSide, openSidesMask)) {
                newMask = setOpenSidesMask(currentSide, true, newMask);
            }
        }

        return newMask;
    }
}