using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
  ButtonHandler buttonHandler;
  Vector3 localScale;
  float healthBarStart;
  float lifeExpectancy = 0.0f;
  float startingHealth = 0.0f;
  public float PercentageofHealthLeft = 0.0f;
  // Start is called before the first frame update
  void Start() {
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
    localScale = transform.localScale;
    healthBarStart = localScale.y;
    lifeExpectancy = Random.Range(70.0f, 100.0f);
    startingHealth = lifeExpectancy;
    PercentageofHealthLeft = lifeExpectancy / startingHealth;
  }

  // Update is called once per frame
  void Update() {
    lifeExpectancy -= Time.deltaTime;
    PercentageofHealthLeft = lifeExpectancy / startingHealth;
    localScale.y = PercentageofHealthLeft * healthBarStart;
    transform.localScale = localScale;
    if(lifeExpectancy <= 0) {
      buttonHandler.MakeRoomOnPlatform();
      Destroy(transform.parent.gameObject);
    }
  }
}
