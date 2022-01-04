using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour {
    [SerializeField] GameObject towerWallPrefab, towerCapPrefab;
    [SerializeField] float minTowerDistance = 100, maxTowerDistance = 300;
    [SerializeField] float minAngleChange = 10, maxAngleChange = 30;
    [SerializeField] int minHeight = 50, maxHeight = 200;
    [SerializeField] int minWidth = 2, maxWidth = 8;
    [SerializeField] float maxTowerRotation = 15;
    [SerializeField] float towerBaseYOffset = -50f; 
    LayerMask skyboxLayer;

    void Start() {
        skyboxLayer = LayerMask.NameToLayer("Skybox");
        spawnTowers();
    }

    void spawnTowers() {
        float angle = Random.Range(minAngleChange, maxAngleChange) / 2;
        while (angle < 360) {
            float distance = Random.Range(minTowerDistance, maxTowerDistance);
            Vector3 position = Quaternion.AngleAxis(angle, transform.up) * transform.forward * distance + Vector3.up * towerBaseYOffset;
            float width = Random.Range(minWidth, maxWidth);
            float height = Random.Range(minHeight, maxHeight);

            createTower(position, width, height);

            angle += Random.Range(minAngleChange, maxAngleChange);
        }
    }

    void createTower(Vector3 position, float width, float height) {
        const float tileWidth = 4f;
        GameObject tower = new GameObject("Tower");
        tower.transform.SetParent(this.transform);
        tower.transform.localPosition = position;
        Vector3 towerWorldPosition = tower.transform.position;

        float totalWidth = width * tileWidth - tileWidth;
        float totalHeight = height * tileWidth;
        Vector3 firstTileOrigin = towerWorldPosition + new Vector3(0, 0, -totalWidth / 2);

        for (float currentHeight = 0; currentHeight < height; currentHeight++) {
            for (float positionOffset = 0; positionOffset < totalWidth; positionOffset += tileWidth) {
                float yOffset = currentHeight * tileWidth;
                GameObject wall = Instantiate(towerWallPrefab, firstTileOrigin + new Vector3(0, yOffset, positionOffset), Quaternion.identity);
                wall.layer = skyboxLayer;
                wall.transform.SetParent(tower.transform);
                if (yOffset >= totalHeight - tileWidth) {
                    GameObject towerCapObject = Instantiate(towerCapPrefab, firstTileOrigin + new Vector3(0, currentHeight * tileWidth, positionOffset), Quaternion.identity);
                    towerCapObject.transform.SetParent(tower.transform);
                    towerCapObject.layer = skyboxLayer;
                }
            }
        }

        firstTileOrigin = towerWorldPosition + new Vector3(0, 0, totalWidth / 2 - tileWidth);

        for (float currentHeight = 0; currentHeight < height; currentHeight++) {
            for (float positionOffset = 0; positionOffset < totalWidth; positionOffset += tileWidth) {
                float yOffset = currentHeight * tileWidth;
                GameObject wall = Instantiate(towerWallPrefab, firstTileOrigin + new Vector3(-positionOffset, yOffset, 0), Quaternion.Euler(0, -90, 0));
                wall.transform.SetParent(tower.transform);
                wall.layer = skyboxLayer;
                if (yOffset >= totalHeight - tileWidth) {
                    GameObject towerCapObject = Instantiate(towerCapPrefab, firstTileOrigin + new Vector3(-positionOffset, currentHeight * tileWidth), Quaternion.identity);
                    towerCapObject.transform.SetParent(tower.transform);
                    towerCapObject.layer = skyboxLayer;
                }
            }
        }

        float randomRotationY = Random.Range(-maxTowerRotation, maxTowerRotation); 
        tower.transform.rotation = Quaternion.LookRotation(towerWorldPosition - (this.transform.position + Vector3.up * towerBaseYOffset), tower.transform.up) * Quaternion.Euler(0, 135 + randomRotationY, 0);
    }
}
