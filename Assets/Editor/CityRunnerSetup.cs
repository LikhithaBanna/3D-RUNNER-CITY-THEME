using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class CityRunnerSetup
{
    [MenuItem("City Runner/Create Demo Scene")]
    public static void CreateScene()
    {
        if (!EditorUtility.DisplayDialog("Create City Runner", "Create a playable demo scene? This creates a new scene.", "Create", "Cancel"))
            return;

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Ensure tags exist (built-in tags are used for player and custom objects are added below).
        AddTag("Obstacle"); AddTag("Coin"); AddTag("Gem");

        GameObject manager = new GameObject("GameManager");
        GameManager gm = manager.AddComponent<GameManager>();

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 1f, 0);
        player.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        player.GetComponent<Renderer>().material.color = new Color(0.15f, 0.55f, 1f);
        player.AddComponent<PlayerRunner>();

        GameObject cam = Camera.main.gameObject;
        cam.transform.position = new Vector3(0, 6, -9);
        CameraFollow follow = cam.AddComponent<CameraFollow>();
        follow.target = player.transform;

        GameObject spawner = new GameObject("TrackSpawner");
        TrackSpawner ts = spawner.AddComponent<TrackSpawner>();
        ts.player = player.transform;

        // UI
        Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        Text hud = MakeText(canvas.transform, "HUD", new Vector2(0, -25), new Vector2(650, 50), 22, TextAnchor.UpperCenter);
        hud.text = "Distance: 0 m    Coins: 0    Gems: 0";
        Text message = MakeText(canvas.transform, "Message", Vector2.zero, new Vector2(700, 250), 28, TextAnchor.MiddleCenter);
        GameObject buttonObj = new GameObject("StartRestartButton");
        buttonObj.transform.SetParent(canvas.transform, false);
        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.1f, 0.45f, 0.9f);
        RectTransform br = buttonObj.GetComponent<RectTransform>();
        br.anchorMin = new Vector2(.5f,.25f); br.anchorMax = new Vector2(.5f,.25f);
        br.sizeDelta = new Vector2(180, 55); br.anchoredPosition = Vector2.zero;
        Button button = buttonObj.AddComponent<Button>();
        Text bt = MakeText(buttonObj.transform, "ButtonText", Vector2.zero, new Vector2(180,55), 20, TextAnchor.MiddleCenter);
        bt.text = "START / RESTART";
        button.onClick.AddListener(() => {
            if (gm.gameOver) gm.RestartGame();
            else gm.StartGame();
        });

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        gm.SetupUI(hud, message, buttonObj);
        RenderSettings.ambientLight = new Color(.65f,.65f,.65f);
        EditorSceneManager.SaveScene(scene, "Assets/CityRunDemo.unity");
        Selection.activeGameObject = player;
        EditorUtility.DisplayDialog("City Runner Ready", "Scene saved as Assets/CityRunDemo.unity. Press Play. Click START / RESTART to begin.", "OK");
    }

    static Text MakeText(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize, TextAnchor anchor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = fontSize;
        t.color = Color.white;
        t.alignment = anchor;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(.5f,1f); rt.anchorMax = new Vector2(.5f,1f);
        rt.pivot = new Vector2(.5f,1f); rt.anchoredPosition = pos; rt.sizeDelta = size;
        return t;
    }

    static void AddTag(string tag)
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (asset == null || asset.Length == 0) return;
        SerializedObject so = new SerializedObject(asset[0]);
        SerializedProperty tags = so.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        so.ApplyModifiedProperties();
    }
}
