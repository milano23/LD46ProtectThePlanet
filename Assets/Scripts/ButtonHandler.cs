using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonHandler : MonoBehaviour {
  [SerializeField]
  GameObject laserTurretPrefab;
  [SerializeField]
  GameObject vacuumTurretPrefab;
  [SerializeField]
  GameObject gameOverPanel;
  [SerializeField]
  GameObject mainMenuPanel;
  [SerializeField]
  GameObject pauseScreenPanel;
  [SerializeField]
  GameObject planet;
  GameManager gm;
  [SerializeField]
  TextMeshProUGUI costText;
  [SerializeField]
  TextMeshProUGUI noMoneyInfo;
  [SerializeField]
  TextMeshProUGUI buildPlatformQuantity;
  [SerializeField]
  TextMeshProUGUI turretDestroyedInfo;
  [SerializeField]
  TextMeshProUGUI yourHighScore;
  public int HighScore { get; set; } = 0;

  AsteroidSpawnManager asteroidSpawnManager;
  int amountOfTurretPlatformsAllowed = 3;
  int amountOfTurretPlatformsUsed = 0;

  public int DefenseFund { get; set; } = 15;
  // Start is called before the first frame update
  void Awake() {
    gm = GameManager.Instance;
    gm.OnStateChange += HandleOnStateChange;
    asteroidSpawnManager = AsteroidSpawnManager.Instance;
    //asteroidManager = GameObject.FindGameObjectWithTag("AsteroidManager");
    //planet = GameObject.FindGameObjectWithTag("Planet");
    //asteroidManager.gameObject.SetActive(false);
    //planet.gameObject.SetActive(false);
    gm.SetGameState(GameState.MainMenu);
    costText.text = DefenseFund.ToString();
    buildPlatformQuantity.text = amountOfTurretPlatformsAllowed.ToString();
    if (gameOverPanel == null) {
      Debug.Log("Game over panel has not been assigned.", this);
    }
  }

  // Update is called once per frame
  void Update() {
    costText.text = DefenseFund.ToString();

    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (gm.gameState == GameState.Running) {
        gm.SetGameState(GameState.Pause);
      } else if (gm.gameState == GameState.Pause) {
        gm.SetGameState(GameState.Running);
      }
    }
  }

  public void HandleOnStateChange() {
    switch (gm.gameState) {
      case GameState.MainMenu:
        planet = null;
        mainMenuPanel.transform.GetChild(0).gameObject.SetActive(true);
        planet = GameObject.FindGameObjectWithTag("Planet");
        planet.gameObject.SetActive(false);
        asteroidSpawnManager.gameObject.SetActive(false);
        if (gameOverPanel != null) {
          gameOverPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (pauseScreenPanel != null) {
          pauseScreenPanel.SetActive(false);
        }
        break;
      case GameState.GameOver:
        yourHighScore.text = HighScore.ToString();
        gameOverPanel.transform.GetChild(0).gameObject.SetActive(true);
        break;
      case GameState.Running:
        asteroidSpawnManager.gameObject.SetActive(true);
        planet.gameObject.SetActive(true);
        if (gameOverPanel != null) {
          gameOverPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (mainMenuPanel != null) {
          mainMenuPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (pauseScreenPanel != null) {
          pauseScreenPanel.SetActive(false);
        }
        break;
      case GameState.Tutorial:
        if (gameOverPanel != null) {
          gameOverPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (mainMenuPanel != null) {
          mainMenuPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (pauseScreenPanel != null) {
          pauseScreenPanel.SetActive(false);
        }
        break;
      case GameState.Pause:
        if (gameOverPanel != null) {
          gameOverPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if (mainMenuPanel != null) {
          mainMenuPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        if(pauseScreenPanel != null) {
          pauseScreenPanel.SetActive(true);
        }
        break;
    }
  }

  public void BuildLaserTurret() {
    if(DefenseFund > 0 && amountOfTurretPlatformsUsed < amountOfTurretPlatformsAllowed) {
      GameObject laserTurretGO = Instantiate(laserTurretPrefab);
      amountOfTurretPlatformsUsed += 1;
      DefenseFund -= 5;
    } else {
      StartCoroutine(NoMoneyMessage());
    }
    costText.text = DefenseFund.ToString();
  }

  public void BuildVacuumTurret() {
    if (DefenseFund > 0 && amountOfTurretPlatformsUsed < amountOfTurretPlatformsAllowed) {
      GameObject vacuumTurretGO = Instantiate(vacuumTurretPrefab);
      amountOfTurretPlatformsUsed += 1;
      DefenseFund -= 5;
    } else {
      StartCoroutine(NoMoneyMessage());
    }
    costText.text = DefenseFund.ToString();
  }

  public void BuildTurretPlatforms() {
    if(DefenseFund > 0) {
      amountOfTurretPlatformsAllowed += 1;
      DefenseFund -= 15;
    } else {
      StartCoroutine(NoMoneyMessage());
    }
    costText.text = DefenseFund.ToString();
    buildPlatformQuantity.text = amountOfTurretPlatformsAllowed.ToString();
  }

  public void MakeRoomOnPlatform() {
    amountOfTurretPlatformsUsed -= 1;
    StartCoroutine(DestroyedTurretMessage());
  }

  public void StartGame() {
    gm.SetGameState(GameState.Running);
  }

  public void StartGameWithTutorial() {
    gm.SetGameState(GameState.Tutorial);
  }

  public void PlayAgain() {
    StartCoroutine(LoadAsyncScene());
  }

  public void QuitGame() {
    Application.Quit();
  }

  public void GoToMainMenu() {
    StartCoroutine(LoadAsyncScene());
  }

  public void ResumeGame() {
    gm.SetGameState(GameState.Running);
  }

  IEnumerator LoadAsyncScene() {
    gm.OnStateChange -= HandleOnStateChange;
    gm.SetGameState(GameState.MainMenu);
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level1", LoadSceneMode.Single);
    Debug.Log("Are we trying to reload teh level again?");
    while (!asyncLoad.isDone) {
      yield return null;
    }
  }

  IEnumerator NoMoneyMessage() {
    noMoneyInfo.gameObject.SetActive(true);
    yield return new WaitForSeconds(5);
    noMoneyInfo.gameObject.SetActive(false);
  }

  IEnumerator DestroyedTurretMessage() {
    turretDestroyedInfo.gameObject.SetActive(true);
    yield return new WaitForSeconds(5);
    turretDestroyedInfo.gameObject.SetActive(false);
  }
}
