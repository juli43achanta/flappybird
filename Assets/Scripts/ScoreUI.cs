using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text restartText;

    private void Start()
    {
        if (scoreText != null)
            scoreText.text = "0";
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
        if (restartText != null)
            restartText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);
            if (restartText != null)
                restartText.gameObject.SetActive(true);
        }
    }
}
