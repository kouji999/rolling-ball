using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text statusText;

    public int Score { get; private set; }
    public int Total { get; private set; }

    private void Awake()
    {
        Instance = this;
        Total = FindObjectsByType<Collectible>().Length;
        UpdateHud();
    }

    public void RegisterCollect()
    {
        Score++;
        UpdateHud();
        if (GameLogic.HasWon(Score, Total))
        {
            statusText.text = "YOU WIN! Press R to restart";
        }
    }

    private void UpdateHud()
    {
        if (scoreText != null) scoreText.text = $"Score: {Score}/{Total}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
