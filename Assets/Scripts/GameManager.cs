using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
    }

    public void SumarPunto()
    {
        score++;
    }

    public void Morir()
    {
        gameOver = true;
        jugando = false;
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

    void OnGUI()
    {
        if (!gameOver)
        {
            GUI.Label(new Rect(Screen.width / 2f - 25, 20, 100, 50), "" + score);
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2f - 80, Screen.height / 2f - 30, 200, 50), "GAME OVER");
            GUI.Label(new Rect(Screen.width / 2f - 80, Screen.height / 2f + 10, 200, 50), "Press SPACE or Click");
        }
    }
}
