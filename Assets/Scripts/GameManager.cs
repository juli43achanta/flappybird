using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text scoreText;
    public Text gameOverText;
    public Text restartText;

    public int score;
    public bool jugando;
    public bool gameOver;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        jugando = false;
        gameOver = false;
        score = 0;

        if (scoreText != null)
            scoreText.text = "" + score;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        if (restartText != null)
            restartText.gameObject.SetActive(false);
    }

    public void SumarPunto()
    {
        score++;
        if (scoreText != null)
            scoreText.text = "" + score;
    }

    public void Morir()
    {
        gameOver = true;
        jugando = false;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        if (restartText != null)
            restartText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
