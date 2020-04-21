using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rocket : MonoBehaviour {
  bool isDragging = false;
  bool constructing = false;
  bool installed = false;
  Vector2 rayPos = Vector2.zero;
  DetectAsteroidWithSphereRay startRayNode;
  DetectAsteroidWithLineRay endRayNode;
  [SerializeField]
  LaserBeam laserBeam;
  [SerializeField]
  GameObject laserNode;
  [SerializeField]
  List<GameObject> asteroidsDetected;
  GameManager gm;
  bool gameIsRunning = false;
  ButtonHandler buttonHandler;
  HealthBar healthBar;
  //float lifeExpectancy = 0.0f;
  //float startingHealth = 0.0f;
  //public float PercentageofHealthLeft = 0.0f;
  //float timeTracker = 0.0f;

  //public float alignSpeed = 2.0f;
  //public float Speed = 15.0f;
  public float Distance = 1.5f;
  // Start is called before the first frame update
  void Start() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    startRayNode = transform.parent.GetChild(1).GetComponent<DetectAsteroidWithSphereRay>();
    endRayNode = transform.parent.GetChild(2).GetComponent<DetectAsteroidWithLineRay>();
    healthBar = transform.parent.GetChild(3).GetComponent<HealthBar>();
    //laserBeam = transform.parent.GetChild(3).GetComponent<LaserBeam>();
    asteroidsDetected = new List<GameObject>();
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
    //lifeExpectancy = Random.Range(60.0f, 90.0f);
    //startingHealth = lifeExpectancy;
    //PercentageofHealthLeft = lifeExpectancy / startingHealth;
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
        if (startRayNode.DetectedAsteroidWithSphere()) {
          if (!asteroidsDetected.Contains(startRayNode.DetectedAsteroidWithSphere())) {
            asteroidsDetected.Add(startRayNode.DetectedAsteroidWithSphere());
            //Debug.Log("Detected asteroid list contains: " + asteroidsDetected.Count + " asteroids. Added");
            //foreach (GameObject asteroid in asteroidsDetected) {
            //  Debug.Log("Names on the detected asteroid list is: " + asteroid.name);
            //}
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
            //Debug.Log("Detected asteroid list contains: " + asteroidsDetected.Count + " asteroids. Removed");
          }
        }

        if (asteroidsDetected.Count > 0) {
          float asteroidHealth = 0.7f;
          foreach (GameObject asteroid in asteroidsDetected.ToList()) {
            float distance = Vector3.Distance(asteroid.transform.position, transform.parent.position);
            //if(asteroid.activeSelf == false) {
            //}
            if (distance < 2.8) {
            Debug.Log("Distance of " + distance + "from asteroid " + asteroid.name);
              //Debug.Log("Fire on " + asteroid.name);
              laserBeam.FireAtTarget(asteroid.transform.position, asteroidHealth, distance);
              asteroidsDetected.Remove(asteroid);
            }
          }
        }
        //PercentageofHealthLeft = lifeExpectancy / startingHealth;
        //if(lifeExpectancy <= 0) {
        //  buttonHandler.MakeRoomOnPlatform();
        //  Destroy(transform.parent.gameObject);
        //}
      }
    } else {
      gm.OnStateChange -= HandleOnStateChange;
      Destroy(this.gameObject);
    }
  }

  IEnumerator ForgetAboutThatAsteroid(GameObject asteroid, float delayedTime) {
    yield return new WaitForSeconds(delayedTime);
    asteroidsDetected.Remove(asteroid);
  }

  public void HandleOnStateChange() {
    Debug.Log("Handling state change in Planet to: " + gm.gameState);
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
