using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMove : MonoBehaviour {
  public GameObject Planet;
  public float Speed = 2.0f;
  [SerializeField]
  List<GameObject> defensesInstalled;
  Vector3 planetRotate = Vector3.forward;
  GameManager gm;
  ButtonHandler buttonHandler;
  int planetLife = 1;
  bool gameIsRunning = false;

  private void OnEnable() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    defensesInstalled = new List<GameObject>();
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
  }

  private void OnDisable() {
    gm.OnStateChange -= HandleOnStateChange;
    gameIsRunning = false;
    defensesInstalled.Clear();
  }

  // Update is called once per frame
  void Update() {
    if (gameIsRunning) {
      Planet.transform.Rotate(planetRotate * Speed);
    }
  }

  public void AddDefenseItemToList(GameObject defenseGO) {
    defensesInstalled.Add(defenseGO);
  }

  public void RemoveDefenseItemFromList(GameObject defenseGo) {
    defensesInstalled.Remove(defenseGo);
  }

  public int DefenseItemCount() {
    return defensesInstalled.Count;
  }

  public void HandleOnStateChange() {
    switch (gm.gameState) {
      case GameState.MainMenu:
        gameIsRunning = false;
        break;
      case GameState.GameOver:
        gameIsRunning = false;
        break;
      case GameState.Running:
        gameIsRunning = true;
        break;
      case GameState.Pause:
        gameIsRunning = false;
        break;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.transform.parent.tag == "Laser") {
      collision.transform.parent.SetParent(this.transform.parent);
    }

    if(collision.transform.parent.tag == "Vacuum") {
      collision.transform.parent.SetParent(this.transform.parent);
    }

    if(collision.transform.parent.tag == "Asteroid") {
      Debug.Log("AAsteroid hit planet and we lost some life!");
      if(defensesInstalled.Count <= 0) {
        gm.SetGameState(GameState.GameOver);
      } else {
        int index = Random.Range(0, defensesInstalled.Count);
        buttonHandler.MakeRoomOnPlatform();
        Destroy(defensesInstalled[index].gameObject);
        defensesInstalled.RemoveAt(index);
      }
    }
  }
}
