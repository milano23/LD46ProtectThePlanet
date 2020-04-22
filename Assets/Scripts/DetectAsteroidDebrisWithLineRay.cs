using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAsteroidDebrisWithLineRay : MonoBehaviour {
  readonly int layer = 12;
  [SerializeField]
  float rayDistance = 10.0f;

  public GameObject DetectedAsteroidDebrisWithLine() {
    int asteroidDebrisMask = 1 << layer;
    Vector3 forward = transform.TransformDirection(Vector3.up) * rayDistance;
    RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, rayDistance, asteroidDebrisMask);
    Debug.DrawRay(transform.position, forward, Color.red);
    if (hit) {
      Transform objectHit = hit.transform;
      //Debug.Log("We ahave detected an incoming " + objectHit.gameObject.name);
      return objectHit.gameObject;
    }

    return null;
  }
}
