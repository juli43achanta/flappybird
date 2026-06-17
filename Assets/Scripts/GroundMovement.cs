using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform secondGround;

    private float groundWidth;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        groundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        transform.position += Vector3.left * speed * Time.deltaTime;
        if (secondGround != null)
            secondGround.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= startPos.x - groundWidth)
        {
            transform.position = secondGround.position + Vector3.right * groundWidth;
        }
        if (secondGround != null && secondGround.position.x <= startPos.x - groundWidth)
        {
            secondGround.position = transform.position + Vector3.right * groundWidth;
        }
    }
}
