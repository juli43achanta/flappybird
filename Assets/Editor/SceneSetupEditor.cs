using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

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
            Texture2D texF = MakeTexture(800, 600, new Color(0.5f, 0.8f, 1f), "Background");
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
            Texture2D texG = MakeTexture(64, 32, new Color(0.6f, 0.4f, 0.2f), "Ground");
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
            Texture2D texB = MakeTexture(32, 32, Color.yellow, "Bird");
            Sprite sprB = Sprite.Create(texB, new Rect(0, 0, texB.width, texB.height), new Vector2(0.5f, 0.5f), 100);

            pajaro = new GameObject("Bird", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
            pajaro.GetComponent<SpriteRenderer>().sprite = sprB;
            pajaro.GetComponent<SpriteRenderer>().sortingOrder = 10;
            pajaro.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
            pajaro.GetComponent<Rigidbody2D>().mass = 0.5f;
            pajaro.GetComponent<CircleCollider2D>().radius = 0.4f;
            pajaro.AddComponent<BirdController>();
            pajaro.transform.position = new Vector3(-3, 0, 0);
            pajaro.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }

        // Prefab tuberia
        string pathPrefab = "Assets/Prefabs/PipePair.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(pathPrefab) == null)
        {
            Texture2D texP = MakeTexture(52, 320, Color.green, "Pipe");
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
            PipeSpawner ps = spawner.AddComponent<PipeSpawner>();
            ps.prefabTuberia = AssetDatabase.LoadAssetAtPath<GameObject>(pathPrefab);
            ps.intervalo = 1.5f;
            ps.velocidad = 3f;
            ps.minY = -2f;
            ps.maxY = 2f;
            ps.espacio = 2.2f;
            spawner.transform.position = new Vector3(8, 0, 0);
        }

        // Canvas y textos
        if (FindObjectOfType<Canvas>() == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Score
            GameObject sGO = new GameObject("ScoreText", typeof(Text));
            sGO.transform.SetParent(canvasGO.transform);
            Text sText = sGO.GetComponent<Text>();
            sText.text = "0";
            sText.font = font;
            sText.fontSize = 72;
            sText.alignment = TextAnchor.UpperCenter;
            sText.color = Color.white;
            RectTransform sRT = sGO.GetComponent<RectTransform>();
            sRT.anchorMin = new Vector2(0.5f, 1);
            sRT.anchorMax = new Vector2(0.5f, 1);
            sRT.pivot = new Vector2(0.5f, 1);
            sRT.anchoredPosition = new Vector2(0, -50);

            // Game Over
            GameObject goGO = new GameObject("GameOverText", typeof(Text));
            goGO.transform.SetParent(canvasGO.transform);
            Text goText = goGO.GetComponent<Text>();
            goText.text = "GAME OVER";
            goText.font = font;
            goText.fontSize = 64;
            goText.alignment = TextAnchor.MiddleCenter;
            goText.color = Color.red;
            goText.gameObject.SetActive(false);
            RectTransform goRT = goGO.GetComponent<RectTransform>();
            goRT.anchorMin = new Vector2(0.5f, 0.5f);
            goRT.anchorMax = new Vector2(0.5f, 0.5f);
            goRT.pivot = new Vector2(0.5f, 0.5f);
            goRT.anchoredPosition = new Vector2(0, 50);

            // Restart
            GameObject rGO = new GameObject("RestartText", typeof(Text));
            rGO.transform.SetParent(canvasGO.transform);
            Text rText = rGO.GetComponent<Text>();
            rText.text = "Press SPACE or Click";
            rText.font = font;
            rText.fontSize = 36;
            rText.alignment = TextAnchor.MiddleCenter;
            rText.color = Color.white;
            rText.gameObject.SetActive(false);
            RectTransform rRT = rGO.GetComponent<RectTransform>();
            rRT.anchorMin = new Vector2(0.5f, 0.5f);
            rRT.anchorMax = new Vector2(0.5f, 0.5f);
            rRT.pivot = new Vector2(0.5f, 0.5f);
            rRT.anchoredPosition = new Vector2(0, -20);

            // Asignar textos al GameManager
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm == null)
            {
                GameObject gmGO = new GameObject("GameManager");
                gm = gmGO.AddComponent<GameManager>();
            }
            gm.scoreText = sText;
            gm.gameOverText = goText;
            gm.restartText = rText;
        }
        else
        {
            // Asegurar GameManager si no existe
            if (FindObjectOfType<GameManager>() == null)
            {
                GameObject gmGO = new GameObject("GameManager");
                gmGO.AddComponent<GameManager>();
            }
        }

        Selection.activeGameObject = pajaro;
        Debug.Log("Escena lista! Presiona Play para jugar.");
    }

    static Texture2D MakeTexture(int w, int h, Color c, string nombre)
    {
        Texture2D tex = new Texture2D(w, h);
        Color[] pix = new Color[w * h];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = c;
        tex.SetPixels(pix);
        tex.Apply();
        string path = "Assets/Sprites/" + nombre + ".png";
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (imp != null)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spritePixelsPerUnit = 100;
            imp.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
}
