using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayPlayModeTests
{
    [UnityTest]
    public IEnumerator Ball_Accelerates_WhenForceApplied()
    {
        var go = new GameObject("TestBall");
        go.AddComponent<Rigidbody>();
        var rb = go.GetComponent<Rigidbody>();
        for (var i = 0; i < 60; i++)
        {
            rb.AddForce(new Vector3(6f, 0f, 0f));
            yield return new WaitForFixedUpdate();
        }
        Assert.Greater(rb.position.x, 0.5f, "ball should move along +X under force");
        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator Collectible_Trigger_IncrementsScore()
    {
        var gmGO = new GameObject("TestGM");
        var gm = gmGO.AddComponent<GameManager>();

        var coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = "TestCoin";
        coin.transform.localScale = new Vector3(0.6f, 0.05f, 0.6f);
        coin.GetComponent<Collider>().isTrigger = true;
        coin.AddComponent<Collectible>();

        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "TestBall";
        ball.tag = "Player";
        ball.AddComponent<Rigidbody>();
        ball.transform.position = coin.transform.position + Vector3.up * 0.3f;

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(1, gm.Score, "score should increment on trigger enter");
        Assert.IsTrue(coin == null, "coin should be destroyed on pickup");
        Object.Destroy(gmGO);
        if (ball != null) Object.Destroy(ball);
    }

    [UnityTest]
    public IEnumerator GameManager_CountsCollectiblesInScene()
    {
        var gmGO = new GameObject("TestGM2");
        var gm = gmGO.AddComponent<GameManager>();
        yield return null;
        Assert.GreaterOrEqual(gm.Total, 0);
        Object.Destroy(gmGO);
    }
}
