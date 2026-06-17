using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Ready;
    public int Score { get; private set; }

    public static System.Action<int> OnScoreChanged;
    public static System.Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        OnStateChanged?.Invoke(CurrentState);
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        OnStateChanged?.Invoke(CurrentState);
    }

    public void AddScore()
    {
        Score++;
        OnScoreChanged?.Invoke(Score);
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        OnStateChanged?.Invoke(CurrentState);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
