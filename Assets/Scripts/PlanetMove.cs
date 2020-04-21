using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMove : MonoBehaviour {
  public GameObject Planet;
  public float Speed = 2.0f;
  Vector3 planetRotate = Vector3.forward;
  GameManager gm;
  int planetLife = 3;
  bool gameIsRunning = false;
  // Start is called before the first frame update
  void Awake() {
    //gm = GameManager.Instance;
    //gm.OnStateChange += HandleOnStateChange;
  }

  private void OnEnable() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
  }

  private void OnDisable() {
    gm.OnStateChange -= HandleOnStateChange;
    gameIsRunning = false;
  }

  // Update is called once per frame
  void Update() {
    if (gameIsRunning) {
      Planet.transform.Rotate(planetRotate * Speed);
    }
  }

  public void HandleOnStateChange() {
    Debug.Log("Handling state change in Planet to: " + gm.gameState);
    if (gm.gameState == GameState.GameOver || gm.gameState == GameState.MainMenu) {
      gameIsRunning = false;
    }
    if (gm.gameState == GameState.Running) {
      gameIsRunning = true;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.transform.parent.tag == "Rocket") {
      Debug.Log("Are we getting hit");
      collision.transform.parent.SetParent(this.transform.parent);
    }

    if(collision.transform.parent.tag == "Vacuum") {
      Debug.Log("Are we getting hit");
      collision.transform.parent.SetParent(this.transform.parent);
    }

    if(collision.transform.parent.tag == "Asteroid") {
      Debug.Log("AAsteroid hit planet and we lost some life!");
      planetLife -= 1;
      if(planetLife <= 0) {
        gm.SetGameState(GameState.GameOver);
      }
    }
  }
}
