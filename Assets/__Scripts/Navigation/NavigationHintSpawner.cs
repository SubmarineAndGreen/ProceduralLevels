using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationHintSpawner : MonoBehaviour {
    public List<Vector3> followNodes;
    [SerializeField] GameObject hintPrefab;
    List<GameObject> hintObjects;

    private void Awake() {
        if (followNodes == null) {
            followNodes = new List<Vector3>();
        }
        hintObjects = new List<GameObject>();
    }

    private void Start() {
        replaceHintObjects();
    }

    private void replaceHintObjects() {
        hintObjects.ForEach(obj => {
            Destroy(obj);
        });

        hintObjects = new List<GameObject>();

        if (followNodes.Count < 2) {
            return;
        }

        for (int i = 0; i < followNodes.Count - 1; i++) {
            Vector3 node = followNodes[i];
            Vector3 nextNode = followNodes[i + 1];
            GameObject hintObject = Instantiate(hintPrefab, node, Quaternion.identity, this.transform);
            hintObjects.Add(hintObject);
            float scaleFactor = (nextNode - node).magnitude;
            hintObject.transform.localScale = new Vector3(1, 1, scaleFactor);

            hintObject.transform.LookAt(nextNode);

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            Renderer renderer = hintObject.GetComponentInChildren<Renderer>();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("TilingY", scaleFactor);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void replaceFollowNodes(List<Vector3> newNodes) {
        hintObjects.ForEach(obj => {
            for(int i = 0; i < obj.transform.childCount; i++) {
                Destroy(obj.transform.GetChild(i).gameObject);
            }
            Destroy(obj);
        });
        followNodes = newNodes;
        replaceHintObjects();
    }
}
