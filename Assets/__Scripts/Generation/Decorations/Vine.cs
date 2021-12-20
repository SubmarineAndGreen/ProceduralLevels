using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    [SerializeField] private GameObject vineSegmentPrefab;
    float segmentHeight = 0.1f;
    // public float segmentCount = 3;

    // private void Start() {
    //     instantiateSegments();
    // }

    public void instantiateSegments(float segmentCount) {
        for(int count = 0; count < segmentCount; count++) {
            GameObject newSegment = Instantiate(vineSegmentPrefab,
                                                transform.position - Vector3.down * segmentHeight * count,
                                                Quaternion.identity);
            newSegment.transform.SetParent(this.transform);
        }
    }
}
