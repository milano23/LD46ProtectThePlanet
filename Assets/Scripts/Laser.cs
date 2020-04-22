using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Laser : MonoBehaviour {
  bool isDragging = false;
  bool constructing = false;
  bool installed = false;
  Vector2 rayPos = Vector2.zero;
  DetectAsteroidWithSphereRay startRayNode;
  DetectAsteroidWithLineRay endRayNode;
  GameObject laserBeamGO;
  [SerializeField]
  LaserBeam laserBeam;
  [SerializeField]
  GameObject laserNode;
  [SerializeField]
  List<GameObject> asteroidsDetected;
  GameManager gm;
  PlanetMove planet;
  bool gameIsRunning = false;
  bool gameIsOver = false;
  ButtonHandler buttonHandler;
  public float Distance = 1.5f;

  // Start is called before the first frame update
  void Start() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    startRayNode = transform.parent.GetChild(1).GetComponent<DetectAsteroidWithSphereRay>();
    endRayNode = transform.parent.GetChild(2).GetComponent<DetectAsteroidWithLineRay>();
    laserBeamGO = transform.parent.GetChild(3).gameObject;
    laserBeamGO.SetActive(false);
    asteroidsDetected = new List<GameObject>();
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
  }

  // Update is called once per frame
  void Update() {
    if (gameIsRunning) {
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
        if (startRayNode.DetectedAsteroidWithSphere()) {
          if (!asteroidsDetected.Contains(startRayNode.DetectedAsteroidWithSphere())) {
            asteroidsDetected.Add(startRayNode.DetectedAsteroidWithSphere());
          }
        }
        if (endRayNode.DetectedAsteroidWithLine()) {
          if (asteroidsDetected.Contains(endRayNode.DetectedAsteroidWithLine())) {
            asteroidsDetected.Remove(endRayNode.DetectedAsteroidWithLine());
            foreach (GameObject asteroid in asteroidsDetected.ToList()) {
              if (asteroid.activeSelf == false) {
                asteroidsDetected.Remove(asteroid);
              }
            }
          }
        }

        if (asteroidsDetected.Count > 0) {
          float asteroidHealth = 0.7f;
          foreach (GameObject asteroid in asteroidsDetected.ToList()) {
            float distance = Vector3.Distance(asteroid.transform.position, transform.parent.position);
            if (distance < 2.8) {
              laserBeam.FireAtTarget(asteroid.transform.position, asteroidHealth, distance);
              asteroidsDetected.Remove(asteroid);
            }
          }
        }
      }
    } else if (gameIsOver) {
      gm.OnStateChange -= HandleOnStateChange;
      Destroy(this.gameObject);
    }
  }

  public void HandleOnStateChange() {
    switch (gm.gameState) {
      case GameState.MainMenu:
        gameIsRunning = false;
        gameIsOver = false;
        break;
      case GameState.GameOver:
        gameIsRunning = false;
        gameIsOver = true;
        break;
      case GameState.Running:
        gameIsRunning = true;
        gameIsOver = false;
        break;
      case GameState.Pause:
        gameIsRunning = false;
        gameIsOver = false;
        break;
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
      laserBeamGO.SetActive(true);
      planet = transform.root.GetChild(0).GetComponent<PlanetMove>();
      planet.AddDefenseItemToList(transform.parent.gameObject);
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (!installed) {
      if (collision.transform.parent.tag == "Planet") {
        isDragging = false;
        constructing = true;
        //this.transform.parent.LookAt()
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
