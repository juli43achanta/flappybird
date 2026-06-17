using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject prefabTuberia;
    public float intervalo = 1.5f;
    public float velocidad = 3f;
    public float minY = -2f;
    public float maxY = 2f;
    public float espacio = 2.2f;

    float timer;

    void Update()
    {
        if (GameManager.instance == null) return;
        if (!GameManager.instance.jugando) return;

        timer += Time.deltaTime;

        if (timer >= intervalo)
        {
            float y = Random.Range(minY, maxY);
            Vector3 pos = new Vector3(transform.position.x, y, 0);

            GameObject tubo = Instantiate(prefabTuberia, pos, Quaternion.identity);

            // Poner el hueco entre tuberias
            Transform top = tubo.transform.Find("TopPipe");
            Transform bot = tubo.transform.Find("BottomPipe");

            if (top != null)
                top.localPosition = new Vector3(0, espacio / 2f, 0);

            if (bot != null)
                bot.localPosition = new Vector3(0, -espacio / 2f, 0);

            // Asignar velocidad
            PipePair pp = tubo.GetComponent<PipePair>();
            if (pp != null)
                pp.velocidad = velocidad;

            timer = 0;
        }
    }
}
