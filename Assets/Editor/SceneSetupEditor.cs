using UnityEngine;
using UnityEditor;

public class SceneSetupEditor : EditorWindow
{
    [MenuItem("Tools/Flappy Bird/Setup Scene")]
    static void SetupScene()
    {
        // Crear carpetas
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
            AssetDatabase.CreateFolder("Assets", "Sprites");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        // Camara
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject c = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            c.tag = "MainCamera";
            cam = c.GetComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.5f, 0.8f, 1f);
        cam.transform.position = new Vector3(0, 0, -10);
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Fondo
        if (GameObject.Find("Background") == null)
        {
            Texture2D texF = MakeBackgroundTex();
            GuardarTextura(texF, "Background");
            Sprite sprF = Sprite.Create(texF, new Rect(0, 0, texF.width, texF.height), new Vector2(0.5f, 0.5f), 100);
            GameObject bg = new GameObject("Background", typeof(SpriteRenderer));
            bg.GetComponent<SpriteRenderer>().sprite = sprF;
            bg.GetComponent<SpriteRenderer>().sortingOrder = -10;
            bg.transform.position = new Vector3(0, 0, 1);
            bg.transform.localScale = new Vector3(25f, 15f, 1);
        }

        // Suelo
        if (GameObject.Find("Ground1") == null)
        {
            Texture2D texG = MakeGroundTex();
            GuardarTextura(texG, "Ground");
            Sprite sprG = Sprite.Create(texG, new Rect(0, 0, texG.width, texG.height), new Vector2(0.5f, 0.5f), 64);

            GameObject g1 = new GameObject("Ground1", typeof(SpriteRenderer), typeof(BoxCollider2D));
            g1.GetComponent<SpriteRenderer>().sprite = sprG;
            g1.GetComponent<SpriteRenderer>().sortingOrder = 5;
            g1.GetComponent<BoxCollider2D>().size = new Vector2(20, 2);
            g1.transform.position = new Vector3(0, -4.5f, 0);

            GameObject g2 = new GameObject("Ground2", typeof(SpriteRenderer), typeof(BoxCollider2D));
            g2.GetComponent<SpriteRenderer>().sprite = sprG;
            g2.GetComponent<SpriteRenderer>().sortingOrder = 5;
            g2.GetComponent<BoxCollider2D>().size = new Vector2(20, 2);
            g2.transform.position = new Vector3(20, -4.5f, 0);

            GroundMovement gm1 = g1.AddComponent<GroundMovement>();
            gm1.velocidad = 3f;
            gm1.otroSuelo = g2;
            g2.AddComponent<GroundMovement>();
        }

        // Pajaro
        GameObject pajaro = GameObject.Find("Bird");
        if (pajaro == null)
        {
            Texture2D texB = MakeBirdTex();
            GuardarTextura(texB, "Bird");
            Sprite sprB = Sprite.Create(texB, new Rect(0, 0, texB.width, texB.height), new Vector2(0.5f, 0.5f), 100);

            pajaro = new GameObject("Bird", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
            pajaro.GetComponent<SpriteRenderer>().sprite = sprB;
            pajaro.GetComponent<SpriteRenderer>().sortingOrder = 10;
            pajaro.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
            pajaro.GetComponent<Rigidbody2D>().mass = 0.5f;
            pajaro.GetComponent<CircleCollider2D>().radius = 0.35f;
            pajaro.AddComponent<BirdController>();
            pajaro.transform.position = new Vector3(-3, 0, 0);
            pajaro.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
        else
        {
            pajaro.GetComponent<CircleCollider2D>().radius = 0.35f;
        }

        // Prefab tuberia
        string pathPrefab = "Assets/Prefabs/PipePair.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(pathPrefab) == null)
        {
            Texture2D texP = MakePipeTex();
            GuardarTextura(texP, "Pipe");
            Sprite sprP = Sprite.Create(texP, new Rect(0, 0, texP.width, texP.height), new Vector2(0.5f, 0.5f), 100);

            GameObject top = new GameObject("TopPipe", typeof(SpriteRenderer), typeof(BoxCollider2D));
            top.GetComponent<SpriteRenderer>().sprite = sprP;
            top.GetComponent<SpriteRenderer>().sortingOrder = 1;
            top.GetComponent<BoxCollider2D>().size = new Vector2(1, 3.2f);

            GameObject bot = new GameObject("BottomPipe", typeof(SpriteRenderer), typeof(BoxCollider2D));
            bot.GetComponent<SpriteRenderer>().sprite = sprP;
            bot.GetComponent<SpriteRenderer>().sortingOrder = 1;
            bot.GetComponent<BoxCollider2D>().size = new Vector2(1, 3.2f);

            GameObject trigger = new GameObject("ScoreTrigger", typeof(BoxCollider2D));
            trigger.GetComponent<BoxCollider2D>().isTrigger = true;
            trigger.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 4f);

            GameObject pair = new GameObject("PipePair");
            top.transform.SetParent(pair.transform);
            bot.transform.SetParent(pair.transform);
            trigger.transform.SetParent(pair.transform);
            pair.AddComponent<PipePair>();

            PrefabUtility.SaveAsPrefabAsset(pair, pathPrefab);
            DestroyImmediate(pair);
        }

        // Spawner
        GameObject spawner = GameObject.Find("PipeSpawner");
        if (spawner == null)
        {
            spawner = new GameObject("PipeSpawner");
            spawner.transform.position = new Vector3(8, 0, 0);
        }
        PipeSpawner ps = spawner.GetComponent<PipeSpawner>();
        if (ps == null) ps = spawner.AddComponent<PipeSpawner>();
        ps.prefabTuberia = AssetDatabase.LoadAssetAtPath<GameObject>(pathPrefab);
        ps.intervalo = 1.5f;
        ps.velocidad = 3f;
        ps.minY = -2f;
        ps.maxY = 2f;
        ps.espacio = 1.2f;

        // GameManager
        if (FindObjectOfType<GameManager>() == null)
        {
            GameObject gmGO = new GameObject("GameManager");
            gmGO.AddComponent<GameManager>();
        }

        Selection.activeGameObject = pajaro;
        Debug.Log("Escena lista! Presiona Play para jugar.");
    }

    static void GuardarTextura(Texture2D tex, string nombre)
    {
        string path = "Assets/Sprites/" + nombre + ".png";
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (imp != null)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spritePixelsPerUnit = 100;
            imp.alphaIsTransparency = true;
            imp.SaveAndReimport();
        }
    }

    static Texture2D MakeBackgroundTex()
    {
        Texture2D tex = new Texture2D(1, 600);
        for (int y = 0; y < 600; y++)
        {
            float t = y / 600f;
            Color c = Color.Lerp(new Color(0.4f, 0.7f, 1f), new Color(0.7f, 0.9f, 1f), t);
            tex.SetPixel(0, y, c);
        }
        tex.Apply();
        return tex;
    }

    static Texture2D MakeGroundTex()
    {
        int w = 64, h = 128;
        Texture2D tex = new Texture2D(w, h);
        Color marron = new Color(0.55f, 0.35f, 0.15f);
        Color verde = new Color(0.3f, 0.7f, 0.2f);
        Color verdeOsc = new Color(0.2f, 0.55f, 0.15f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                // Parte de arriba: cesped con borde
                if (y >= h - 24)
                {
                    if (y >= h - 4 || x < 2 || x > w - 3)
                        tex.SetPixel(x, y, verdeOsc);
                    else
                        tex.SetPixel(x, y, verde);
                }
                else
                {
                    // Tierra con variacion
                    float ruido = Random.Range(-0.03f, 0.03f);
                    Color c = new Color(marron.r + ruido, marron.g + ruido, marron.b + ruido);
                    tex.SetPixel(x, y, c);
                }
            }
        }
        tex.Apply();
        return tex;
    }

    static Texture2D MakeBirdTex()
    {
        int s = 32;
        Texture2D tex = new Texture2D(s, s);
        Color amarillo = new Color(1f, 0.84f, 0f);
        Color naranja = new Color(1f, 0.55f, 0f);
        Color ala = new Color(0.9f, 0.7f, 0f);

        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float cx = x - 15;
                float cy = y - 15;
                float dist = Mathf.Sqrt(cx * cx + cy * cy);

                if (dist < 13f)
                {
                    tex.SetPixel(x, y, amarillo);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        // Ala
        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float cx = x - 16;
                float cy = y - 10;
                if (cx > 0 && cx < 8 && cy > -3 && cy < 3)
                {
                    Color c = tex.GetPixel(x, y);
                    if (c.a > 0)
                        tex.SetPixel(x, y, ala);
                }
            }
        }

        // Pico
        for (int i = 0; i < 6; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                int px = 26 + i;
                int py = 15 + j;
                if (px >= 0 && px < s && py >= 0 && py < s)
                    tex.SetPixel(px, py, naranja);
            }
        }

        tex.Apply();
        return tex;
    }

    static Texture2D MakePipeTex()
    {
        int w = 52, h = 320;
        Texture2D tex = new Texture2D(w, h);
        Color verdeOscuro = new Color(0.2f, 0.6f, 0.15f);
        Color verdeClaro = new Color(0.3f, 0.75f, 0.2f);
        Color borde = new Color(0.15f, 0.4f, 0.1f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color c = verdeClaro;

                if (x < 3 || x > w - 4)
                    c = borde;
                else if (x < 6 || x > w - 7)
                    c = verdeOscuro;

                // Cap inferior (para tubo de arriba)
                if (y < 25)
                {
                    if (y < 5)
                        c = borde;
                    else if (y < 10 || (x < 6 || x > w - 7))
                        c = verdeOscuro;
                    else
                        c = verdeClaro;
                }

                // Cap superior (para tubo de abajo)
                if (y > h - 26)
                {
                    if (y > h - 6)
                        c = borde;
                    else if (y > h - 11 || (x < 6 || x > w - 7))
                        c = verdeOscuro;
                    else
                        c = verdeClaro;
                }

                tex.SetPixel(x, y, c);
            }
        }
        tex.Apply();
        return tex;
    }
}
