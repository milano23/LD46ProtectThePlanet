using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDebris : MonoBehaviour {
  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.transform.tag == "VacuumNozel") {
      //Debug.Log("I have hit the vacuum");
      Destroy(this.gameObject);
    }
  }
}
