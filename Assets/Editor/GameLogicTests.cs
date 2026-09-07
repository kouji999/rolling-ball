#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public static class GameLogicTests
{
    [Test]
    public static void MoveDirection_MapsInputToXZPlane()
    {
        var dir = GameLogic.MoveDirection(new Vector2(1f, 2f));
        Assert.AreEqual(new Vector3(1f, 0f, 2f), dir);
    }

    [Test]
    public static void MoveDirection_ZeroInputStaysZero()
    {
        Assert.AreEqual(Vector3.zero, GameLogic.MoveDirection(Vector2.zero));
    }

    [Test]
    public static void HasWon_RequiresAllCollectibles()
    {
        Assert.IsFalse(GameLogic.HasWon(3, 5));
        Assert.IsTrue(GameLogic.HasWon(5, 5));
        Assert.IsFalse(GameLogic.HasWon(0, 0));
    }
}
#endif
