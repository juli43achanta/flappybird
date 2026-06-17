using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SceneSetupEditor : EditorWindow
{
    private const string PrefabPath = "Assets/Prefabs/PipePair.prefab";

    [MenuItem("Tools/Flappy Bird/Setup Scene")]
    public static void SetupScene()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Stop the game before setting up the scene.");
            return;
        }

        CreateSpritesFolder();
        SetupCamera();
        SetupBackground();
        SetupGround();
        GameObject bird = SetupBird();
        GameObject pipeSpawner = SetupPipeSpawner();
        SetupCanvas();
        SetupGameManager(bird, pipeSpawner);

        Selection.activeGameObject = bird;

        Debug.Log("Flappy Bird scene setup complete! Press Play to start.");
    }

    private static void CreateSpritesFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
            AssetDatabase.CreateFolder("Assets", "Sprites");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
    }

    private static Texture2D CreateColoredTexture(int width, int height, Color color, string name)
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        string path = $"Assets/Sprites/{name}.png";
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cam = camGO.GetComponent<Camera>();
            camGO.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.5f, 0.8f, 1f);
        cam.transform.position = new Vector3(0, 0, -10);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    private static void SetupBackground()
    {
        GameObject bg = GameObject.Find("Background");
        if (bg != null) return;

        Texture2D bgTex = CreateColoredTexture(800, 600, new Color(0.5f, 0.8f, 1f), "Background");
        bg = new GameObject("Background", typeof(SpriteRenderer));
        Sprite bgSprite = Sprite.Create(bgTex, new Rect(0, 0, bgTex.width, bgTex.height), new Vector2(0.5f, 0.5f), 100);
        bg.GetComponent<SpriteRenderer>().sprite = bgSprite;
        bg.GetComponent<SpriteRenderer>().sortingOrder = -10;
        bg.transform.position = new Vector3(0, 0, 1);
        bg.transform.localScale = new Vector3(25f, 15f, 1);
    }

    private static void SetupGround()
    {
        if (GameObject.Find("Ground1") != null) return;

        Texture2D groundTex = CreateColoredTexture(64, 32, new Color(0.6f, 0.4f, 0.2f), "Ground");
        Sprite groundSprite = Sprite.Create(groundTex, new Rect(0, 0, groundTex.width, groundTex.height), new Vector2(0.5f, 0.5f), 64);

        GameObject ground1 = new GameObject("Ground1", typeof(SpriteRenderer), typeof(BoxCollider2D));
        ground1.GetComponent<SpriteRenderer>().sprite = groundSprite;
        ground1.GetComponent<SpriteRenderer>().sortingOrder = 5;
        ground1.GetComponent<BoxCollider2D>().size = new Vector2(20, 2);
        ground1.transform.position = new Vector3(0, -4.5f, 0);
        ground1.transform.localScale = new Vector3(1, 1, 1);

        GameObject ground2 = new GameObject("Ground2", typeof(SpriteRenderer), typeof(BoxCollider2D));
        ground2.GetComponent<SpriteRenderer>().sprite = groundSprite;
        ground2.GetComponent<SpriteRenderer>().sortingOrder = 5;
        ground2.GetComponent<BoxCollider2D>().size = new Vector2(20, 2);
        ground2.transform.position = new Vector3(20, -4.5f, 0);
        ground2.transform.localScale = new Vector3(1, 1, 1);

        GroundMovement gm = ground1.AddComponent<GroundMovement>();
        ground2.AddComponent<GroundMovement>();
        SerializedObject so = new SerializedObject(gm);
        so.FindProperty("secondGround").objectReferenceValue = ground2.transform;
        so.ApplyModifiedProperties();
    }

    private static GameObject SetupBird()
    {
        GameObject bird = GameObject.Find("Bird");
        if (bird != null) return bird;

        Texture2D birdTex = CreateColoredTexture(32, 32, Color.yellow, "Bird");
        Sprite birdSprite = Sprite.Create(birdTex, new Rect(0, 0, birdTex.width, birdTex.height), new Vector2(0.5f, 0.5f), 100);

        bird = new GameObject("Bird", typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(CircleCollider2D));
        bird.GetComponent<SpriteRenderer>().sprite = birdSprite;
        bird.GetComponent<SpriteRenderer>().sortingOrder = 10;
        bird.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        bird.GetComponent<Rigidbody2D>().mass = 0.5f;
        bird.GetComponent<CircleCollider2D>().radius = 0.4f;
        bird.AddComponent<BirdController>();
        bird.transform.position = new Vector3(-3, 0, 0);
        bird.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        return bird;
    }

    private static void CreatePipePrefab()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null)
            return;

        Texture2D pipeTex = CreateColoredTexture(52, 320, Color.green, "Pipe");
        Sprite pipeSprite = Sprite.Create(pipeTex, new Rect(0, 0, pipeTex.width, pipeTex.height), new Vector2(0.5f, 0.5f), 100);

        GameObject topPipe = new GameObject("TopPipe", typeof(SpriteRenderer), typeof(BoxCollider2D));
        topPipe.GetComponent<SpriteRenderer>().sprite = pipeSprite;
        topPipe.GetComponent<SpriteRenderer>().sortingOrder = 1;
        topPipe.GetComponent<BoxCollider2D>().size = new Vector2(1, 3.2f);

        GameObject bottomPipe = new GameObject("BottomPipe", typeof(SpriteRenderer), typeof(BoxCollider2D));
        bottomPipe.GetComponent<SpriteRenderer>().sprite = pipeSprite;
        bottomPipe.GetComponent<SpriteRenderer>().sortingOrder = 1;
        bottomPipe.GetComponent<BoxCollider2D>().size = new Vector2(1, 3.2f);

        GameObject scoreTrigger = new GameObject("ScoreTrigger", typeof(BoxCollider2D));
        scoreTrigger.GetComponent<BoxCollider2D>().isTrigger = true;
        scoreTrigger.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 4f);

        GameObject pipePair = new GameObject("PipePair");
        topPipe.transform.SetParent(pipePair.transform);
        bottomPipe.transform.SetParent(pipePair.transform);
        scoreTrigger.transform.SetParent(pipePair.transform);

        PipePair pairComponent = pipePair.AddComponent<PipePair>();

        SerializedObject so = new SerializedObject(pairComponent);
        so.FindProperty("topPipe").objectReferenceValue = topPipe.transform;
        so.FindProperty("bottomPipe").objectReferenceValue = bottomPipe.transform;
        so.FindProperty("scoreTrigger").objectReferenceValue = scoreTrigger.transform;
        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(pipePair, PrefabPath);
        Object.DestroyImmediate(pipePair);
    }

    private static GameObject SetupPipeSpawner()
    {
        GameObject spawner = GameObject.Find("PipeSpawner");
        if (spawner != null) return spawner;

        CreatePipePrefab();

        spawner = new GameObject("PipeSpawner");
        PipeSpawner ps = spawner.AddComponent<PipeSpawner>();
        spawner.transform.position = new Vector3(8, 0, 0);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        SerializedObject so = new SerializedObject(ps);
        so.FindProperty("pipePrefab").objectReferenceValue = prefab;
        so.ApplyModifiedProperties();

        return spawner;
    }

    private static void SetupCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null) return;

        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject scoreGO = new GameObject("ScoreText", typeof(Text));
        scoreGO.transform.SetParent(canvasGO.transform);
        Text scoreText = scoreGO.GetComponent<Text>();
        scoreText.text = "0";
        scoreText.font = font;
        scoreText.fontSize = 72;
        scoreText.alignment = TextAnchor.UpperCenter;
        scoreText.color = Color.white;
        RectTransform scoreRT = scoreGO.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(0.5f, 1);
        scoreRT.anchorMax = new Vector2(0.5f, 1);
        scoreRT.pivot = new Vector2(0.5f, 1);
        scoreRT.anchoredPosition = new Vector2(0, -50);

        GameObject gameOverGO = new GameObject("GameOverText", typeof(Text));
        gameOverGO.transform.SetParent(canvasGO.transform);
        Text gameOverText = gameOverGO.GetComponent<Text>();
        gameOverText.text = "GAME OVER";
        gameOverText.font = font;
        gameOverText.fontSize = 64;
        gameOverText.alignment = TextAnchor.MiddleCenter;
        gameOverText.color = Color.red;
        gameOverText.gameObject.SetActive(false);
        RectTransform goRT = gameOverGO.GetComponent<RectTransform>();
        goRT.anchorMin = new Vector2(0.5f, 0.5f);
        goRT.anchorMax = new Vector2(0.5f, 0.5f);
        goRT.pivot = new Vector2(0.5f, 0.5f);
        goRT.anchoredPosition = new Vector2(0, 50);

        GameObject restartGO = new GameObject("RestartText", typeof(Text));
        restartGO.transform.SetParent(canvasGO.transform);
        Text restartText = restartGO.GetComponent<Text>();
        restartText.text = "Press SPACE or Click to Restart";
        restartText.font = font;
        restartText.fontSize = 36;
        restartText.alignment = TextAnchor.MiddleCenter;
        restartText.color = Color.white;
        restartText.gameObject.SetActive(false);
        RectTransform rRT = restartGO.GetComponent<RectTransform>();
        rRT.anchorMin = new Vector2(0.5f, 0.5f);
        rRT.anchorMax = new Vector2(0.5f, 0.5f);
        rRT.pivot = new Vector2(0.5f, 0.5f);
        rRT.anchoredPosition = new Vector2(0, -20);

        ScoreUI scoreUI = canvasGO.AddComponent<ScoreUI>();
        SerializedObject so = new SerializedObject(scoreUI);
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("gameOverText").objectReferenceValue = gameOverText;
        so.FindProperty("restartText").objectReferenceValue = restartText;
        so.ApplyModifiedProperties();
    }

    private static void SetupGameManager(GameObject bird, GameObject pipeSpawner)
    {
        GameObject gmGO = GameObject.Find("GameManager");
        if (gmGO != null) return;

        gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();
    }
}
