using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float fuerzaSalto = 8f;
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
            rb.linearVelocity = Vector2.up * fuerzaSalto;
        }

        // Rotacion del pajaro segun velocidad
        float angulo = Mathf.Clamp(rb.linearVelocity.y * velRotacion, -90f, -25f);
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // Morir solo si se cae por debajo del suelo
        if (transform.position.y < -6f)
        {
            muerto = true;
            gm.Morir();
        }
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
