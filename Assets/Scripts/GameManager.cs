using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum GameState {None, Tutorial, MainMenu, Running, Pause, GameOver };
public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour {

  private static GameManager _instance;
  public static GameManager Instance {
    [MethodImpl(MethodImplOptions.Synchronized)]
    get {
      if (_instance == null) {
        _instance = FindObjectOfType<GameManager>();

        if (_instance == null) {
          GameObject container = new GameObject("GameManager");
          _instance = container.AddComponent<GameManager>();
        }
      }
      return _instance;
    }
  }

  public GameState gameState { get; private set; }
  public event OnStateChangeHandler OnStateChange;
  public bool TutorialPlayed = false;

  // Start is called before the first frame update
  void Awake() {
    if(_instance != null && _instance != this) {
      Destroy(this.gameObject);
      return;
    }

    _instance = this;
    DontDestroyOnLoad(this.gameObject);
  }

  // Update is called once per frame
  void Update() {
  }

  public void SetGameState(GameState state) {
    this.gameState = state;
    OnStateChange();
  }
}
