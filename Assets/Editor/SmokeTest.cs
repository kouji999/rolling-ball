using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SmokeTest
{
    private static int _failures;

    [MenuItem("Autopilot/Run Smoke Tests")]
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);

        var player = GameObject.Find("Player");
        Check(player != null, "Player exists");
        Check(player.GetComponent<Rigidbody>() != null, "Player has Rigidbody");
        Check(player.GetComponent<PlayerController>() != null, "Player has PlayerController");
        Check(player.CompareTag("Player"), "Player tagged");

        var collectibles = Object.FindObjectsByType<Collectible>();
        Check(collectibles.Length == 8, $"8 collectibles (found {collectibles.Length})");
        foreach (var c in collectibles)
        {
            var col = c.GetComponent<Collider>();
            Check(col != null && col.isTrigger, $"{c.name} trigger collider");
        }

        var gm = Object.FindObjectsByType<GameManager>().Length > 0 ? Object.FindObjectsByType<GameManager>()[0] : null;
        Check(gm != null, "GameManager exists");
        Check(gm.scoreText != null, "GameManager scoreText wired");
        Check(gm.statusText != null, "GameManager statusText wired");

        var cam = Object.FindObjectsByType<CameraController>().Length > 0 ? Object.FindObjectsByType<CameraController>()[0] : null;
        Check(cam.target == player.transform, "Camera targets player");

        Check(GameObject.Find("Ground") != null, "Ground exists");
        Check(GameObject.Find("Wall North") != null, "Wall North exists");
        Check(GameObject.Find("Directional Light") != null, "Light exists");

        Check(GameLogic.MoveDirection(new Vector2(1f, 2f)) == new Vector3(1f, 0f, 2f), "MoveDirection maps XZ");
        Check(!GameLogic.HasWon(7, 8), "HasWon false below total");
        Check(GameLogic.HasWon(8, 8), "HasWon true at total");

        if (_failures == 0)
        {
            Debug.Log("[SmokeTest] ALL SMOKE TESTS PASSED");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.Log($"[SmokeTest] {_failures} FAILURES");
            EditorApplication.Exit(3);
        }
    }

    private static void Check(bool cond, string label)
    {
        if (cond) Debug.Log($"[SmokeTest] PASS {label}");
        else { Debug.LogError($"[SmokeTest] FAIL {label}"); _failures++; }
    }
}
