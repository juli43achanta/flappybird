using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float fuerzaSalto = 6f;
    public float velRotacion = 8f;

    Rigidbody2D rb;
    bool muerto;
    GameManager gm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (muerto) return;
        if (gm == null) return;

        if (gm.gameOver) return;

        // Estado inicial - esperando que empiece
        if (!gm.jugando)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                gm.jugando = true;
                rb.gravityScale = 1f;
            }
            return;
        }

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            rb.velocity = Vector2.up * fuerzaSalto;
        }

        // Rotacion del pajaro segun velocidad
        float angulo = Mathf.Clamp(rb.velocity.y * velRotacion, -90f, -25f);
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (muerto) return;
        if (gm == null) return;
        if (gm.gameOver) return;

        muerto = true;
        gm.Morir();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (muerto) return;
        if (gm == null) return;
        if (gm.gameOver) return;
        if (!gm.jugando) return;

        gm.SumarPunto();
        other.enabled = false;
    }
}
