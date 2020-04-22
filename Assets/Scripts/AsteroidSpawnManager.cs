using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

//public delegate void OnAsteroidCountChangeHandler();
public class AsteroidSpawnManager : MonoBehaviour {
  private static AsteroidSpawnManager _instance;
  public static AsteroidSpawnManager Instance {
    [MethodImpl(MethodImplOptions.Synchronized)]
    get {
      if (_instance == null) {
        _instance = FindObjectOfType<AsteroidSpawnManager>();

        if (_instance == null) {
          GameObject container = new GameObject("AsteroidSpawnManager");
          _instance = container.AddComponent<AsteroidSpawnManager>();
        }
      }
      return _instance;
    }
  }

  public static event Action OnAsteroidCountChange;
  [SerializeField]
  List<GameObject> asteroidPrefabs;
  [SerializeField]
  float spawnRate = 0.0f;
  public List<GameObject> Asteroids;
  List<GameObject> PooledAsteroids;
  List<GameObject> usedAsteroids;
  Vector2 spawnLocation;
  float trackTime = 0;
  float timeToWait = 0.0f;
  float timeDelay = 0f;
  float waveIntermission = 0.0f;
  bool itIsIntermission = false;
  GameManager gm;
  ButtonHandler buttonHandler;
  bool gameIsRunning = false;
  bool gameIsOver = false;
  public int Wave { get; set; } = 0;
  public int AsteroidsPerWave { get; set; } = 0;
  public int AsteroidsSleeping { get; set; } = 0;
  public int ActivatedAsteroids { get; set; } = 0;
  public TextMeshProUGUI AsteroidIncomingInformation;
  public TextMeshProUGUI ScoreInformation;
  int pooledAmount = 0;
  int score = 0;
  int asteroidsGroupedAmount = 0;
  int asteroidStartCount = 0;
  int asteroidLengthCount = 0;
  // Start is called before the first frame update
  void Awake() {
    if (_instance != null && _instance != this) {
      Destroy(this.gameObject);
      return;
    }
    _instance = this;
    //DontDestroyOnLoad(this.gameObject);
  }

  private void OnEnable() {
    gameIsRunning = true;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    OnAsteroidCountChange += HandleAsteroidCountChange;
    buttonHandler = GameObject.Find("LevelController").GetComponent<ButtonHandler>();
    //AsteroidIncomingInformation = GameObject.Find("AsteroidsIncomingText").GetComponent<TextMeshProUGUI>();
    //AsteroidIncomingInformation.transform.parent.gameObject.SetActive(true);
    spawnRate = 5.0f;
    timeToWait = 5.0f;
    waveIntermission = 10.0f;
    pooledAmount = 1000;
    PooledAsteroids = new List<GameObject>();
    Asteroids = new List<GameObject>();
    usedAsteroids = new List<GameObject>();
    AsteroidsSleeping = 0;
    ActivatedAsteroids = 0;
    asteroidsGroupedAmount = 5;
    asteroidStartCount = 0;
    asteroidLengthCount = 0;
    score = 0;
    ScoreInformation.text = score.ToString();
    if (transform.childCount < pooledAmount) {
      for (int i = 0; i < pooledAmount; i++) {
        GameObject asteroidObject = Instantiate(asteroidPrefabs[UnityEngine.Random.Range(0, 4)]);
        asteroidObject.SetActive(false);
        asteroidObject.name = "Asteroid" + i;
        asteroidObject.transform.SetParent(transform);
        PooledAsteroids.Add(asteroidObject);
        //float timeToWait = spawnRate * i;
        //StartCoroutine(ActivateAsteroid(timeToWait, asteroidObject));
      }
    }
    CreateAsteroids();
  }

  private void OnDisable() {
    gm.OnStateChange -= HandleOnStateChange;
    gameIsRunning = false;
    Wave = 0;
    timeDelay = 0.0f;
    AsteroidsPerWave = 0;
    ActivatedAsteroids = 0;
    spawnRate = 5.0f;
    AsteroidsSleeping = 0;
    pooledAmount = 0;
    asteroidsGroupedAmount = 0;
    asteroidStartCount = 0;
    asteroidLengthCount = 0;
    score = 0;
    if (Asteroids.Count > 0) {
      //Debug.Log("Why am i coming in here if there are no more asteroids? " + Asteroids.Count);
      for (int i = 0; i < Asteroids.Count; i++) {
        Destroy(Asteroids[i]);
      }
      Asteroids.Clear();
    }
    if(usedAsteroids.Count > 0) {
      for (int i = 0; i < usedAsteroids.Count; i++) {
        Destroy(usedAsteroids[i]);
      }
      usedAsteroids.Clear();
    }
    if(PooledAsteroids.Count > 0) {
      for (int i = 0; i < PooledAsteroids.Count; i++) {
        Destroy(PooledAsteroids[i]);
      }
      PooledAsteroids.Clear();
    }
  }

  private void CreateAsteroids() {
    Wave += 1;
    AsteroidsPerWave += 5 * Wave;
    if(Wave == 7) {
      timeToWait = 5.0f;
    }
    timeToWait -= 0.75f;
    if (timeToWait <= 0) {
      timeToWait = 0.5f;
    }
    for (int i = 0; i < AsteroidsPerWave; i++) {
      Asteroids.Add(PooledAsteroids[i]);
    }
    AsteroidIncomingInformation.text = AsteroidsPerWave.ToString();
  }

  // Update is called once per frame
  void Update() {
      if (gameIsRunning) {
      trackTime += Time.deltaTime;
      if(trackTime > spawnRate) {
        if(ActivatedAsteroids < AsteroidsPerWave) {
          if (AsteroidsPerWave < 100) {
            ActivateAsteroid(Asteroids[ActivatedAsteroids]);
            AsteroidIncomingInformation.text = (AsteroidsPerWave - AsteroidsSleeping).ToString();
            timeDelay = timeToWait;
          } else {
            asteroidLengthCount = asteroidsGroupedAmount + ActivatedAsteroids;
            for (int i = ActivatedAsteroids; i < asteroidLengthCount; i++) {
              ActivateAsteroid(Asteroids[i]);
              AsteroidIncomingInformation.text = (AsteroidsPerWave - AsteroidsSleeping).ToString();
            }
            timeDelay = timeToWait;
          }
        } else {
          timeDelay = 100.0f;
        }
        spawnRate += timeDelay;
      }
    } else if (gameIsOver) {
      if (Asteroids.Count > 0) {
        for (int i = 0; i < Asteroids.Count; i++) {
          Destroy(Asteroids[i]);
        }
        Asteroids.Clear();
      }

      //if (usedAsteroids.Count > 0) {
      //  for (int i = 0; i < usedAsteroids.Count; i++) {
      //    Destroy(usedAsteroids[i]);
      //  }
      //  usedAsteroids.Clear();
      //}
    }
  }

  void ActivateAsteroid(GameObject asteroid) {
    if (!gameIsRunning) {
      return;
    }
    ActivatedAsteroids += 1;
    int randNum = UnityEngine.Random.Range(0, 4);
    switch (randNum) {
      case 0:
        spawnLocation = new Vector2(UnityEngine.Random.Range(8.5f, 9.5f), UnityEngine.Random.Range(-6.5f, 6.5f));
        break;
      case 1:
        spawnLocation = new Vector2(UnityEngine.Random.Range(-8.5f, -9.5f), UnityEngine.Random.Range(-6.5f, 6.5f));
        break;
      case 2:
        spawnLocation = new Vector2(UnityEngine.Random.Range(-9.5f, 9.5f), UnityEngine.Random.Range(5.5f, 6.5f));
        break;
      case 3:
        spawnLocation = new Vector2(UnityEngine.Random.Range(-9.5f, 9.5f), UnityEngine.Random.Range(-5.5f, -6.5f));
        break;
      default:
        spawnLocation = new Vector2(UnityEngine.Random.Range(8.5f, 9.5f), UnityEngine.Random.Range(-6.5f, 6.5f));
        break;
    }
    if (gameIsRunning) {
      if(asteroid != null) {
        asteroid.transform.position = spawnLocation;
        asteroid.transform.rotation = Quaternion.identity;
        asteroid.SetActive(true);
      }
    }
    //Asteroids.RemoveAt(0);
  }

  public void AccountForAsteroid(float scoreCalculator) {
    score += Mathf.FloorToInt(10 * scoreCalculator);
    AsteroidsSleeping += 1;
    if (AsteroidsPerWave == AsteroidsSleeping) {
      Asteroids.Clear();
      usedAsteroids.Clear();
      AsteroidsSleeping = 0;
      ActivatedAsteroids = 0;
      spawnRate = waveIntermission + trackTime;
      CreateAsteroids();
    }
      OnAsteroidCountChange();
  }

  public void HandleAsteroidCountChange() {
    AsteroidIncomingInformation.text = (AsteroidsPerWave - AsteroidsSleeping).ToString();
    ScoreInformation.text = score.ToString();
    buttonHandler.HighScore = score;
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
}
