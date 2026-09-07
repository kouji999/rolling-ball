using UnityEngine;

public static class GameLogic
{
    public static Vector3 MoveDirection(Vector2 input)
    {
        return new Vector3(input.x, 0f, input.y);
    }

    public static bool HasWon(int score, int total)
    {
        return total > 0 && score >= total;
    }
}
