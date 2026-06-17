using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float pipeSpeed = 3f;
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 2f;
    [SerializeField] private float gapSize = 2.2f;

    private float timer;

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPipe();
            timer = 0;
        }
    }

    private void SpawnPipe()
    {
        float yPos = Random.Range(minY, maxY);
        Vector2 spawnPos = new Vector2(transform.position.x, yPos);
        GameObject pipe = Instantiate(pipePrefab, spawnPos, Quaternion.identity);

        PipePair pair = pipe.GetComponent<PipePair>();
        if (pair != null)
        {
            pair.speed = pipeSpeed;
            pair.gapSize = gapSize;
        }
    }
}
