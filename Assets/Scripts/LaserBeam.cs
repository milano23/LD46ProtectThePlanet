using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {
  SpriteRenderer spriteRender;
  [SerializeField]
  Vector3 Target;
  [SerializeField]
  bool firing = false;
  bool targeting = false;
  Vector3 tempScale;
  // Start is called before the first frame update
  void Start() {
    spriteRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
    spriteRender.enabled = false;
  }

  // Update is called once per frame
  void Update() {

    if (targeting) {
      Vector2 targetDirection = Target - transform.position;
      float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.AngleAxis(0 - angle, Vector3.forward);
    }

  }

  public void FireAtTarget(Vector3 target, float delayedTime, float distance) {
    Target = target;
    Vector2 targetDirection = target - transform.position;
    float offSet = distance * 0.5f;
    //float distance = Vector3.Distance(target, transform.position);
    Debug.Log("My Target is at: " + target);
    Debug.Log("How long my laser is: " + distance);
    tempScale = transform.localScale;
    transform.localScale = new Vector3(tempScale.x, distance + offSet, tempScale.z);
    if(!firing) {
      targeting = true;
      StartCoroutine(FireLaser(delayedTime));
    }
    firing = true;
  }

  IEnumerator FireLaser(float delayedTime) {
    //Debug.Log("Delayed time is: " + delayedTime);
    firing = true;
    spriteRender.enabled = true;
    yield return new WaitForSeconds(delayedTime);
    spriteRender.enabled = false;
    transform.localScale = tempScale;
    transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    targeting = false;
    firing = false;

  }
}
