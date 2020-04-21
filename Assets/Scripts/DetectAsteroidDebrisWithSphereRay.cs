using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAsteroidDebrisWithSphereRay : MonoBehaviour {
  readonly int layer = 12;
  [SerializeField]
  float sphereRayRadius = 5.0f;
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public GameObject DetectedAsteroidDebrisWithSphere() {
    int asteroidDebrisMask = 1 << layer;
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, sphereRayRadius, asteroidDebrisMask);

    foreach (Collider2D collider2D in hitColliders) {
      Transform objectHit = collider2D.transform;
      //Debug.Log("We have detected an incoming " + objectHit.gameObject.name);
      return objectHit.gameObject;
    }

    return null;
  }

  private void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(this.transform.position, sphereRayRadius);
  }
}
