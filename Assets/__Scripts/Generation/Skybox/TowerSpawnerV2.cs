using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawnerV2 : MonoBehaviour
{
    [SerializeField] GameObject towerPrefab;
    [SerializeField] int numberOfTowers = 15;
    // [SerializeField] float maxTowerY = 0, minTowerY = -30;
    [SerializeField] float towerY = -100f;
    [SerializeField] float maxDistance = 300, minDistance = 450;
    [SerializeField] int minScale = 4, maxScale = 12;
    [SerializeField] int minHeightScale = 3, maxHeightScale = 6;
    [SerializeField] float  maxPositionRadialOffset = 15;
    [SerializeField] float baseWindowTilingX = 8f, baseWindowTilingY = 15f;
    // const float baseWindowTilingY = 30f;

    private void Start() {
        for(int i = 0; i < numberOfTowers; i++) {
            float angle = Mathf.Lerp(0, 360, Mathf.InverseLerp(0, numberOfTowers - 1, i));
            float distance = Random.Range(minDistance, maxDistance);
            float heightOffset = towerY;/*Random.Range(minTowerY, maxTowerY);*/
            float heightScale = Random.Range(minHeightScale, maxHeightScale);
            float scale = Random.Range(minScale, maxScale);

            Quaternion radialPositionAngle = Quaternion.AngleAxis(angle + Random.Range(-maxPositionRadialOffset, maxPositionRadialOffset), transform.up);

            Vector3 position = transform.position + Vector3.up * heightOffset + radialPositionAngle * transform.forward * distance;

            GameObject towerObject = Instantiate(towerPrefab, position, Quaternion.Euler(0, Random.Range(-180, 180), 0));

            towerObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Skybox");
            towerObject.transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer("Skybox");

            towerObject.transform.SetParent(this.transform);
            towerObject.transform.localScale = new Vector3(scale, heightScale, scale);

            Renderer renderer = towerObject.transform.GetChild(0).GetComponent<Renderer>();
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);
            propBlock.SetVector("_WindowTiling", new Vector4(baseWindowTilingX * scale, baseWindowTilingY * heightScale, 0 ,0));
            renderer.SetPropertyBlock(propBlock);
        }
    }
}
