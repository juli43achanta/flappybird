using UnityEngine;

public class PipePair : MonoBehaviour
{
    public float speed = 3f;
    public float gapSize = 2.2f;

    [SerializeField] private Transform topPipe;
    [SerializeField] private Transform bottomPipe;
    [SerializeField] private Transform scoreTrigger;

    private void Awake()
    {
        SetupGap();
    }

    private void SetupGap()
    {
        if (topPipe != null)
            topPipe.localPosition = new Vector3(0, gapSize * 0.5f, 0);
        if (bottomPipe != null)
            bottomPipe.localPosition = new Vector3(0, -gapSize * 0.5f, 0);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }
}
