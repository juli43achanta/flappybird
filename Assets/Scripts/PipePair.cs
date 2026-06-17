using UnityEngine;

public class PipePair : MonoBehaviour
{
    public float velocidad = 3f;

    void Update()
    {
        if (GameManager.instance == null) return;
        if (!GameManager.instance.jugando) return;

        transform.position += Vector3.left * velocidad * Time.deltaTime;

        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }
}
