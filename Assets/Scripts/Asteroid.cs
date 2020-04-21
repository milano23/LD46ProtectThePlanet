using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {
  GameManager gm;
  [SerializeField]
  float speed = 0.5f;
  [SerializeField]
  float rotationSpeed = 1.0f;
  [SerializeField]
  float newScale = 1.0f;
  Vector3 target = Vector3.zero;
  Vector3 rotationAngle = Vector3.forward;
  [SerializeField]
  bool imHit = false;
  bool hitPlanet = false;
  [SerializeField]
  GameObject explosionParticle;
  [SerializeField]
  GameObject asteroidDebris;
  GameObject explosion;
  AsteroidSpawnManager asm;
  bool gameIsRunning = false;
  public float Speed {
    get { return speed; }
  }
  public float RotionaSpeed {
    get {return rotationSpeed; }
  }
  // Start is called before the first frame update
  void Start() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    asm = AsteroidSpawnManager.Instance;
    speed = Random.Range(0.1f, 0.6f);
    rotationSpeed = Random.Range(0.5f, 1.5f);
    newScale = Random.Range(0.5f, 4);
    transform.localScale = new Vector3(newScale, newScale, newScale);
    explosion = Instantiate(explosionParticle, transform.position, Quaternion.identity);
  }

  // Update is called once per frame
  void Update() {
    if (gameIsRunning) {
      transform.parent.Rotate(rotationAngle * RotionaSpeed);
      transform.parent.position = Vector3.MoveTowards(this.transform.parent.position, target, Speed * Time.deltaTime);
    } else {
      gm.OnStateChange -= HandleOnStateChange;
      Destroy(this.gameObject);
    }
  }

  public void HandleOnStateChange() {
    if(gm.gameState == GameState.GameOver) {
      //Debug.Log("Are we destroying the asteroids early");
      gameIsRunning = false;
      //Destroy(this.gameObject);
    }
    if (gm.gameState == GameState.Running) {
      gameIsRunning = true;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.transform.tag == "Laser") {
      //Debug.Log("You got me!!!!");
      if (!imHit) {
        StartCoroutine(DestroyAsteroid(0.5f));
      }
      imHit = true;
    }
    if(collision.transform.parent.tag == "Planet") {
    //Debug.Log("Are we getting hit by that ginormous laser beam!!!!");
      hitPlanet = true;
      if (!imHit) {
        StartCoroutine(DestroyAsteroid(0.5f));
      }
      imHit = true;
    }
  }

  IEnumerator DestroyAsteroid(float delayTime) {
    yield return new WaitForSeconds(delayTime);
    asm.AccountForAsteroid();
    explosion.transform.position = transform.position;
    explosion.GetComponent<ParticleSystem>().Play();
    if (!hitPlanet) {
      Instantiate(asteroidDebris, transform.position, Quaternion.identity);
    }
    transform.parent.position = new Vector2(7.0f, 6.0f);
    imHit = false;
    transform.parent.gameObject.SetActive(false);
    hitPlanet = false;
  }
}
