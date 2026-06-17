using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public float velocidad = 3f;
    public GameObject otroSuelo;

    float ancho;

    void Start()
    {
        ancho = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (GameManager.instance == null) return;
        if (!GameManager.instance.jugando) return;

        // Mover suelos
        transform.position += Vector3.left * velocidad * Time.deltaTime;

        if (otroSuelo != null)
            otroSuelo.transform.position += Vector3.left * velocidad * Time.deltaTime;

        // Loop infinito
        if (transform.position.x <= -ancho)
        {
            if (otroSuelo != null)
                transform.position = new Vector3(otroSuelo.transform.position.x + ancho, transform.position.y, transform.position.z);
        }

        if (otroSuelo != null && otroSuelo.transform.position.x <= -ancho)
        {
            otroSuelo.transform.position = new Vector3(transform.position.x + ancho, otroSuelo.transform.position.y, otroSuelo.transform.position.z);
        }
    }
}
