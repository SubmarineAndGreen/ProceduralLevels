using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public int playerLayer;
    [HideInInspector] public List<Vector3Int> validSpawningTiles;
    private void Awake() {
        instance = this;
        validSpawningTiles = new List<Vector3Int>();
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public Vector3Int getRandomValidSpawningTile() {
        return validSpawningTiles[Random.Range(0, validSpawningTiles.Count - 1)];
    }

}