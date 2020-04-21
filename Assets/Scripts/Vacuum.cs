using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vacuum : MonoBehaviour {
  bool isDragging = false;
  bool constructing = false;
  bool installed = false;
  Vector2 rayPos = Vector2.zero;
  DetectAsteroidDebrisWithLineRay detectDebrisNode;
  GameManager gm;
  bool gameIsRunning = false;
  public float Distance = 1.5f;
  ButtonHandler buttonHandler;
  HealthBar healthBar;
  //float lifeExpectancy = 0.0f;
  //float timeTracker = 0.0f;
  // Start is called before the first frame update
  void Start() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    detectDebrisNode = transform.parent.GetChild(1).GetComponent<DetectAsteroidDebrisWithLineRay>();
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
    healthBar = transform.parent.GetChild(2).GetComponent<HealthBar>();
    //lifeExpectancy = Random.Range(45.0f, 60.0f);
  }

  // Update is called once per frame
  void Update() {
    if (gameIsRunning) {
      //timeTracker += Time.deltaTime;
      rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
      if (isDragging && !installed) {
        transform.parent.position = rayPos;
      }
      if (constructing) {
        float angle = Mathf.Atan2(rayPos.x, rayPos.y) * Mathf.Rad2Deg;
        float xPos = Mathf.Cos(Mathf.Atan2(rayPos.x, rayPos.y)) * Distance;
        float yPos = Mathf.Sin(Mathf.Atan2(rayPos.x, rayPos.y)) * Distance;
        transform.parent.localRotation = Quaternion.AngleAxis(0 - angle, Vector3.forward);
        transform.parent.localPosition = new Vector3(yPos, xPos, 0);
      }

      if (installed) {
        if (detectDebrisNode.DetectedAsteroidDebrisWithLine()) {
          GameObject asteroid = detectDebrisNode.DetectedAsteroidDebrisWithLine();
          float distance = Vector3.Distance(asteroid.transform.position, transform.parent.position);
          if (distance < 5.5) {
            asteroid.transform.position = Vector3.MoveTowards(asteroid.transform.position, transform.position, 5.0f * Time.deltaTime);
          }
        }

        //if (timeTracker > lifeExpectancy) {
        //  lifeExpectancy += timeTracker;
        //  buttonHandler.MakeRoomOnPlatform();
        //  Destroy(transform.parent.gameObject);
        //}
      }
    } else {
      gm.OnStateChange -= HandleOnStateChange;
      Destroy(this.gameObject);
    }
  }

  public void HandleOnStateChange() {
    Debug.Log("Handling state change in Vacuum to: " + gm.gameState);
    if (gm.gameState == GameState.MainMenu) {
      gameIsRunning = false;
    }
    if (gm.gameState == GameState.GameOver) {
      gameIsRunning = false;
    }
    if (gm.gameState == GameState.Running) {
      gameIsRunning = true;
    }
  }

  private void OnMouseDown() {
    if (!constructing || !installed) {
      isDragging = true;
    }
  }

  private void OnMouseUp() {
    isDragging = false;
    if (constructing) {
      constructing = false;
      installed = true;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (!installed) {
      Debug.Log("Are we hitting");
      if (collision.transform.parent.tag == "Planet") {
        isDragging = false;
        constructing = true;
        //this.transform.parent.LookAt()
      }
    } else {
      if (collision.transform.tag == "AsteroidDebris") {
        //Debug.Log("Where is my money!!!");
        buttonHandler.DefenseFund += 5;
      }
    }

  }

  private void OnTriggerStay2D(Collider2D collision) {
    if (!installed) {
      if (collision.gameObject.tag == "Planet") {
        //isDragging = false;
        this.transform.localPosition = rayPos;
      }
    }
  }
}
