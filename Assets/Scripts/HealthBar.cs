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
  PlanetMove planet;
  bool gameIsRunning = false;
  bool gameOver = false;
  GameManager gm;
  // Start is called before the first frame update
  void Start() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
    localScale = transform.localScale;
    healthBarStart = localScale.y;
    lifeExpectancy = Random.Range(70.0f, 100.0f);
    startingHealth = lifeExpectancy;
    PercentageofHealthLeft = lifeExpectancy / startingHealth;
  }

  // Update is called once per frame
  void Update() {
    if (gameIsRunning) {
      lifeExpectancy -= Time.deltaTime;
      PercentageofHealthLeft = lifeExpectancy / startingHealth;
      localScale.y = PercentageofHealthLeft * healthBarStart;
      transform.localScale = localScale;
      if (lifeExpectancy <= 0) {
        planet = transform.root.GetChild(0).GetComponent<PlanetMove>();
        planet.RemoveDefenseItemFromList(transform.parent.gameObject);
        buttonHandler.MakeRoomOnPlatform();
        Destroy(transform.parent.gameObject);
      }
    } else if (gameOver) {
      gm.OnStateChange -= HandleOnStateChange;
    }
  }

  void HandleOnStateChange() {
    switch (gm.gameState) {
      case GameState.GameOver:
        gameIsRunning = false;
        gameOver = true;
        break;
      case GameState.Running:
        gameIsRunning = true;
        gameOver = false;
        break;
      case GameState.Pause:
        gameIsRunning = false;
        gameOver = false;
        break;
    }
  }
}
