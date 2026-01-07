using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    int score = 0;
    public Text scoreText;

    private void Awake()
    {
        instance = this;
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }
}