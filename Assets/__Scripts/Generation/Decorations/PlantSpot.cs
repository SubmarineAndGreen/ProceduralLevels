using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpot : MonoBehaviour {
    [SerializeField] float chanceToSpawn = 0.5f;
    [SerializeField] List<GameObject> potPrefabs;
    [SerializeField] List<GameObject> plantPrefabs;
    [SerializeField] float scale = 0.1f;

    private void Start() {
        if (Random.Range(0f, 1f) <= chanceToSpawn) {
            GameObject potObject = Instantiate(potPrefabs[Random.Range(0, potPrefabs.Count)],
                                  transform.position,
                                  Quaternion.Euler(0, Random.Range(-180, 180), 0),
                                  transform);
            potObject.transform.localScale = new Vector3(scale, scale, scale);
            Pot pot = potObject.GetComponent<Pot>();
            GameObject plantObject = Instantiate(plantPrefabs[Random.Range(0, plantPrefabs.Count)],
                                     pot.plantOrigin.position,
                                     Quaternion.Euler(0, Random.Range(-180, 180), 0),
                                     potObject.transform);
            // plantObject.transform.localScale = new Vector3(plantScale, plantScale, plantScale);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}
