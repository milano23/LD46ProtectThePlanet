using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAsteroidWithLineRay : MonoBehaviour {
  readonly int layer = 10;
  [SerializeField]
  float rayDistance = 10.0f;

  public GameObject DetectedAsteroidWithLine() {
    int laserMask = 1 << layer;
    Vector3 forward = transform.TransformDirection(Vector3.up) * rayDistance;
    RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, rayDistance, laserMask);
    Debug.DrawRay(transform.position, forward, Color.red);
    if (hit) {
      Transform objectHit = hit.transform.parent;
      //Debug.Log("We ahave detected an incoming " + objectHit.gameObject.name);
      return objectHit.gameObject;
    }

    return null;
  }
}
