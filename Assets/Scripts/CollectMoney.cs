using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMoney : MonoBehaviour {
  ButtonHandler buttonHandler;
  // Start is called before the first frame update
  void Start() {
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.transform.tag == "AsteroidDebris") {
      //Debug.Log("Where is my money!!!");
      buttonHandler.DefenseFund += 5;
    }

  }
}
