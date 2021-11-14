using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WfcRunner))]
public class LevelBuilder : MonoBehaviour
{
    private WfcRunner wfcRunner;
    [SerializeField] private TileGrid levelGrid;
    [SerializeField] Vector3Int gridDimensions;

    void Start()
    {
        wfcRunner = GetComponent<WfcRunner>();
        wfcRunner.output = levelGrid;
        levelGrid.clear();
        levelGrid.resize(gridDimensions);
        wfcRunner.runAdjacentModelWithPathConstraint();
    }

}
