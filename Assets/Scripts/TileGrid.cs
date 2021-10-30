using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;

public class TileGrid : MonoBehaviour
{
    [HideInInspector] public string currentSaveFile;
    private Vector3Int dimensions = new Vector3Int(3, 3, 3);
    int[,,] tileIndices;

    private void Start() {
        
    }
}



