using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
  enum TutorialState { None, AsteroidInfo, DefenseFundInfo, LaserInfo, VacuumInfo, PlatformInfo, PlacementInfo, Finished };
  delegate void OnTutorialStateChangeHandler();

  static event OnTutorialStateChangeHandler tutorialStateChanged;
  TutorialState tutorialState { get; set; }

  [SerializeField]
  GameObject howToPlayPanel;
  [SerializeField]
  GameObject asteroidQuantityInfo;
  [SerializeField]
  GameObject defenseFundInfo;
  [SerializeField]
  GameObject laserDefenseInfo;
  [SerializeField]
  GameObject vacuumDefenseInfo;
  [SerializeField]
  GameObject defensePlatformInfo;
  [SerializeField]
  GameObject placementInfo;
  GameManager gm;
  float timeTracker = 0.0f;
  float tutorialPanelTimer = 16.0f;

  // Start is called before the first frame update
  //void Start() {
  //  tutorialStateChanged += HandleTutorialStateChanged;
  //  gm = GameManager.Instance;
  //  gm.OnStateChange += HandleGameStateChange;
  //  timeTracker = 0.0f;
  //  tutorialPanelTimer = 15.0f;
  //}

  private void OnEnable() {
    tutorialStateChanged += HandleTutorialStateChanged;
    gm = GameManager.Instance;
    gm.OnStateChange += HandleGameStateChange;
    timeTracker = 0.0f;
    tutorialPanelTimer = 15.0f;
  }

  private void OnDisable() {
    tutorialStateChanged -= HandleTutorialStateChanged;
    gm.OnStateChange -= HandleGameStateChange;
  }

  // Update is called once per frame
  void Update() {
    timeTracker -= Time.deltaTime;

    if (tutorialState != TutorialState.None) {
      if (Input.GetMouseButtonDown(0)) {
        timeTracker = 0.1f;
      }
      if (timeTracker <= 0.0f) {
        SetState(NextTutorialState(tutorialState));
      }
    }
  }

  TutorialState NextTutorialState(TutorialState state) => Enum.GetValues(state.GetType())
    .Cast<TutorialState>()
    .Concat(new[] { default(TutorialState) })
    .SkipWhile(e => !state.Equals(e))
    .Skip(1).First();
  TutorialState PreviousTutorialState(TutorialState state) => Enum.GetValues(state.GetType())
    .Cast<TutorialState>()
    .Concat(new[] { default(TutorialState) })
    .Reverse().SkipWhile(e => !state.Equals(e))
    .Skip(1).First();

  void SetState(TutorialState state) {
    this.tutorialState = state;
    tutorialStateChanged();
  }

  void HandleTutorialPanels() {
    switch (tutorialState) {
      case TutorialState.AsteroidInfo:
        if (howToPlayPanel.activeSelf == false) {
          howToPlayPanel.SetActive(true);
        }
        if (asteroidQuantityInfo.activeSelf == false) {
          asteroidQuantityInfo.SetActive(true);
        }
        break;
      case TutorialState.DefenseFundInfo:
        asteroidQuantityInfo.SetActive(false);
        if (defenseFundInfo.activeSelf == false) {
          defenseFundInfo.SetActive(true);
        }
        break;
      case TutorialState.LaserInfo:
        defenseFundInfo.SetActive(false);
        if (laserDefenseInfo.activeSelf == false) {
          laserDefenseInfo.SetActive(true);
        }
        break;
      case TutorialState.VacuumInfo:
        laserDefenseInfo.SetActive(false);
        if (vacuumDefenseInfo.activeSelf == false) {
          vacuumDefenseInfo.SetActive(true);
        }
        break;
      case TutorialState.PlatformInfo:
        vacuumDefenseInfo.SetActive(false);
        if (defensePlatformInfo.activeSelf == false) {
          defensePlatformInfo.SetActive(true);
        }
        break;
      case TutorialState.PlacementInfo:
        defensePlatformInfo.SetActive(false);
        if (placementInfo.activeSelf == false) {
          placementInfo.SetActive(true);
        }
        tutorialPanelTimer = 25.0f;
        break;
      case TutorialState.None:
        howToPlayPanel.SetActive(false);
        asteroidQuantityInfo.SetActive(false);
        defenseFundInfo.SetActive(false);
        laserDefenseInfo.SetActive(false);
        vacuumDefenseInfo.SetActive(false);
        defensePlatformInfo.SetActive(false);
        placementInfo.SetActive(false);
        break;
    }
    timeTracker = tutorialPanelTimer;
  }

  void HandleTutorialStateChanged() {
    if (tutorialState == TutorialState.Finished) {
      gm.SetGameState(GameState.Running);
      SetState(TutorialState.None);
    } else {
      HandleTutorialPanels();
    }
  }

  void HandleGameStateChange() {
    if(gm.gameState == GameState.Tutorial && tutorialState == TutorialState.None) {
      SetState(TutorialState.AsteroidInfo);
    }
    if(gm.gameState != GameState.Tutorial) {
      SetState(TutorialState.None);
    }
  }
}
