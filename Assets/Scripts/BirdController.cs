using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BirdController : MonoBehaviour
{
    [SerializeField] private float flapForce = 6f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float maxTiltUp = -25f;
    [SerializeField] private float maxTiltDown = -90f;

    private Rigidbody2D rb;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            rb.gravityScale = 1f;
        }
        else if (state == GameState.GameOver)
        {
            isDead = true;
        }
    }

    private void Update()
    {
        if (isDead) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

        float angle = Mathf.Clamp(rb.linearVelocity.y * rotationSpeed, maxTiltDown, maxTiltUp);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Flap()
    {
        rb.linearVelocity = Vector2.up * flapForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        GameManager.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        GameManager.Instance.AddScore();
        other.enabled = false;
    }
}
