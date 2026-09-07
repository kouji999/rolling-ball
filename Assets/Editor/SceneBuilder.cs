using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SceneBuilder
{
    private static Material _groundMat;
    private static Material _wallMat;
    private static Material _playerMat;
    private static Material _coinMat;

    [MenuItem("Autopilot/Build Main Scene")]
    public static void BuildMain()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EnsureMaterials();

        var light = new GameObject("Directional Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        light.intensity = 1.1f;

        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -0.5f, 0f);
        ground.transform.localScale = new Vector3(24f, 1f, 24f);
        ground.GetComponent<Renderer>().sharedMaterial = _groundMat;

        BuildWall("Wall North", new Vector3(0f, 0.75f, 12f), new Vector3(24.5f, 1.5f, 0.5f));
        BuildWall("Wall South", new Vector3(0f, 0.75f, -12f), new Vector3(24.5f, 1.5f, 0.5f));
        BuildWall("Wall East", new Vector3(12f, 0.75f, 0f), new Vector3(0.5f, 1.5f, 24.5f));
        BuildWall("Wall West", new Vector3(-12f, 0.75f, 0f), new Vector3(0.5f, 1.5f, 24.5f));

        var player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0.5f, 0f);
        player.transform.localScale = new Vector3(1f, 1f, 1f);
        player.GetComponent<Renderer>().sharedMaterial = _playerMat;
        var rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        player.AddComponent<PlayerController>();

        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0f, 9f, -9f);
        camGO.AddComponent<AudioListener>();
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        var camCtrl = camGO.AddComponent<CameraController>();
        camCtrl.target = player.transform;

        var gm = new GameObject("GameManager").AddComponent<GameManager>();

        for (var i = 0; i < 8; i++)
        {
            var angle = i * Mathf.PI * 2f / 8f;
            var coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.name = $"Collectible_{i:00}";
            coin.transform.position = new Vector3(Mathf.Cos(angle) * 7f, 0.6f, Mathf.Sin(angle) * 7f);
            coin.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
            coin.GetComponent<Renderer>().sharedMaterial = _coinMat;
            coin.GetComponent<Collider>().isTrigger = true;
            coin.AddComponent<Collectible>();
        }

        BuildHud(gm);

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        var path = "Assets/Scenes/Main.unity";
        EditorSceneManager.SaveScene(scene, path);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };
        AssetDatabase.SaveAssets();
        Debug.Log($"[SceneBuilder] DONE scene={path} collectibles={Object.FindObjectsByType<Collectible>().Length}");
    }

    private static void BuildWall(string name, Vector3 pos, Vector3 scale)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().sharedMaterial = _wallMat;
    }

    private static void BuildHud(GameManager gm)
    {
        var canvasGO = new GameObject("Canvas");
        canvasGO.AddComponent<Canvas>();
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var canvas = canvasGO.GetComponent<Canvas>();
        var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 24);

        var scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(canvas.transform, false);
        var scoreText = scoreGO.AddComponent<Text>();
        scoreText.font = font;
        scoreText.fontSize = 32;
        scoreText.color = Color.white;
        scoreText.rectTransform.anchorMin = new Vector2(0f, 1f);
        scoreText.rectTransform.anchorMax = new Vector2(0f, 1f);
        scoreText.rectTransform.pivot = new Vector2(0f, 1f);
        scoreText.rectTransform.anchoredPosition = new Vector2(24f, -24f);
        scoreText.rectTransform.sizeDelta = new Vector2(400f, 44f);

        var statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(canvas.transform, false);
        var statusText = statusGO.AddComponent<Text>();
        statusText.font = font;
        statusText.fontSize = 40;
        statusText.fontStyle = FontStyle.Bold;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.white;
        statusText.rectTransform.anchorMin = new Vector2(0.5f, 0.85f);
        statusText.rectTransform.anchorMax = new Vector2(0.5f, 0.85f);
        statusText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        statusText.rectTransform.sizeDelta = new Vector2(600f, 60f);

        gm.scoreText = scoreText;
        gm.statusText = statusText;
    }

    private static void EnsureMaterials()
    {
        System.IO.Directory.CreateDirectory("Assets/Materials");
        _groundMat = CreateMaterial("Ground", new Color(0.28f, 0.30f, 0.33f));
        _wallMat = CreateMaterial("Wall", new Color(0.16f, 0.19f, 0.24f));
        _playerMat = CreateMaterial("Player", new Color(0.20f, 0.55f, 1.00f), 0.9f, 0.75f);
        _coinMat = CreateMaterial("Coin", new Color(1.00f, 0.76f, 0.20f), 0.8f, 0.85f);
    }

    private static Material CreateMaterial(string name, Color color, float metallic = 0.05f, float smoothness = 0.4f)
    {
        var path = $"Assets/Materials/{name}.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, path);
        }
        var hasColor = mat.HasProperty("_BaseColor") || mat.HasProperty("_Color");
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        EditorUtility.SetDirty(mat);
        return mat;
    }
}
