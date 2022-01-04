using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour {
    [SerializeField] List<GameObject> cloudPrefabs;
    List<Transform> cloudTransforms;
    [SerializeField] float maxRandomAngleOffset = 5f;
    [SerializeField] int cloudLayers = 6, minCloudsPreLayer = 2, maxCloudsPerLayer = 4;
    [SerializeField] [Range(0, Mathf.PI / 2)] float layersRange;
    [SerializeField] float sphereRadius = 1f;
    [SerializeField] float minAngularVelocity = 10, maxAngluarVelocity = 30;
    [SerializeField] float cloudScaleFactor = 1;
    // [SerializeField]
    List<float> cloudAngularVelocity;

    private void Start() {
        cloudTransforms = new List<Transform>();
        cloudAngularVelocity = new List<float>();

        for (int i = 0; i <= cloudLayers; i++) {
            float layerAngle = Mathf.Lerp(0 + layersRange, Mathf.PI - layersRange, (float)i / cloudLayers);
            // Debug.Log(angle / Mathf.PI);
            int cloudsInLayer = Random.Range(minCloudsPreLayer, maxCloudsPerLayer);
            for (int j = 0; j < cloudsInLayer; j++) {
                float angle = -1;
                while (!(angle >= 0 + layersRange && angle <= Mathf.PI - layersRange)) {
                    float randomOffset = Random.Range(-maxRandomAngleOffset, maxRandomAngleOffset);
                    angle = layerAngle + randomOffset;
                }

                float layerY = Mathf.Cos(angle);
                float layerX = Mathf.Sin(angle);

                GameObject prefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Count)];
                GameObject cloudObject = Instantiate(prefab,
                                                     this.transform.position + new Vector3(layerX, layerY, 0).normalized * sphereRadius,
                                                     Quaternion.identity);
                cloudObject.GetComponent<MeshRenderer>().material.SetFloat("TimeOffset", Random.Range(0, 10));
                cloudTransforms.Add(cloudObject.transform);
                cloudObject.transform.localScale = new Vector3(cloudScaleFactor, cloudScaleFactor, cloudScaleFactor);
                cloudObject.transform.SetParent(this.transform);
                cloudObject.layer = LayerMask.NameToLayer("Skybox");
            }
        }

        foreach (Transform t in cloudTransforms) {
            cloudAngularVelocity.Add(Random.Range(minAngularVelocity, maxAngluarVelocity));
        }

        randomizeStartingRotation();
    }

    private void Update() {
        updateCloudPositions();
    }

    private void updateCloudPositions() {
        for (int i = 0; i < cloudTransforms.Count; i++) {
            Transform cloudTransform = cloudTransforms[i];
            Vector3 offset = cloudTransform.localPosition;
            Quaternion layerRotation = Quaternion.AngleAxis(cloudAngularVelocity[i] * Time.deltaTime, Vector3.up);
            cloudTransform.position = transform.position + layerRotation * offset;
            Vector3 toCenter = transform.position - cloudTransform.position;
            cloudTransform.rotation = Quaternion.LookRotation(toCenter);
            cloudTransform.Rotate(0, 90, 0);
            // Debug.DrawRay(cloudTransform.position, Vector3.Cross(toCenter, cloudTransform.right));
            // cloudTransform.Rotate()
        }
    }

    private void randomizeStartingRotation() {
        foreach (Transform cloudTransform in cloudTransforms) {
            Vector3 offset = cloudTransform.localPosition;
            Quaternion layerRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            cloudTransform.position = transform.position + layerRotation * offset;
        }
    }
}
