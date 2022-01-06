using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud2DSpawner : MonoBehaviour {
    [SerializeField] List<GameObject> cloudPrefabs;
    [SerializeField] float cloudMinDistance = 400, cloudMaxDistance = 600;
    [SerializeField] float cloudMinHeight = 0, cloudMaxHeight = 300;
    [SerializeField] int numberOfClouds = 100;
    [SerializeField] float minRotationSpeed = 2, maxRotationSpeed = 4;
    [SerializeField] float minCloudScale = 15f, maxCloudScale = 30f;
    List<Transform> cloudTransforms;
    List<float> cloudAngularVelocities;

    private void Start() {
        cloudTransforms = new List<Transform>();
        cloudAngularVelocities = new List<float>();

        for (int i = 0; i < numberOfClouds; i++) {
            GameObject cloudObject = Instantiate(cloudPrefabs[Random.Range(0, cloudPrefabs.Count)]);
            cloudObject.layer = LayerMask.NameToLayer("Skybox");
            Transform newCloud = cloudObject.transform;
            cloudTransforms.Add(newCloud);
            newCloud.SetParent(this.transform);
            cloudAngularVelocities.Add(Random.Range(minRotationSpeed, maxRotationSpeed));

            float height = Random.Range(cloudMinHeight, cloudMaxHeight);
            Vector3 heightOffset = Vector3.up * height;

            float distance = Random.Range(cloudMinDistance, cloudMaxDistance);

            newCloud.position = this.transform.position + heightOffset + Quaternion.AngleAxis(Random.Range(0f, 360f), this.transform.up) * this.transform.forward * distance;
            newCloud.localScale = Vector3.one * Random.Range(minCloudScale, maxCloudScale);
        }
    }

    private void Update() {
        for (int i = 0; i < cloudTransforms.Count; i++)
        {
            Transform cloudTransform = cloudTransforms[i];
            Vector3 offset = cloudTransform.localPosition;
            Vector3 spawnerPosition = this.transform.position;
            Quaternion rotation = Quaternion.AngleAxis(cloudAngularVelocities[i] * Time.deltaTime, Vector3.up);
            cloudTransform.position = spawnerPosition + rotation * offset;

            cloudTransform.LookAt(new Vector3(spawnerPosition.x, cloudTransform.position.y, spawnerPosition.z), cloudTransform.up);
        }
    }
}