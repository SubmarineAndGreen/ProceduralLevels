using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WfcSolver : MonoBehaviour
{
    [HideInInspector] public string modelFile;
    [SerializeField] private TileGrid outputGrid;
    [SerializeField] private TileSet tileSet;
}
