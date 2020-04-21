using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidDebris : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.transform.parent.tag == "Vacuum") {
      //Debug.Log("I have hit the vacuum");
      Destroy(this.gameObject);
    }
  }
}
