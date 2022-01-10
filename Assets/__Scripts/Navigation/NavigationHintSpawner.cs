using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationHintSpawner : MonoBehaviour {
    public List<Transform> followNodes;
    [SerializeField] GameObject hintPrefab;
    List<GameObject> hintObjects;

    private void Awake() {
        followNodes = new List<Transform>();
    }

    private void replaceHintObjects() {
        if(followNodes.Count < 2) {
            return;
        }

        for (int i = 0; i < followNodes.Count - 1; i++) {
            Transform node = followNodes[i];
            Transform nextNode = followNodes[i + 1];
            GameObject hintObject = Instantiate(hintPrefab, node.position, Quaternion.identity, this.transform);
            float scaleFactor = (nextNode.localPosition - node.position).magnitude;
            hintObject.transform.localScale = new Vector3(1, 1, scaleFactor);
            hintObject.transform.LookAt(nextNode, nextNode.up);
        }
    }

    private void replaceFollowNodes(List<Transform> newNodes) {
        followNodes.ForEach(node => {
            Destroy(node.gameObject);
        });

        followNodes = newNodes;
        replaceHintObjects();
    }
}
