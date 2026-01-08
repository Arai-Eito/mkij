using UnityEngine;
using UnityEngine.UI;
using unityroom.Api;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    int score = 0;
    public Text scoreText;

    bool uploaded = false;

    private void Awake()
    {
        instance = this;
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
    }

    public void UploadScore()
    {
        if (uploaded) return; // すでにアップロード済みなら何もしない。
        // ボードNo1にスコアを送信する。
        UnityroomApiClient.Instance.SendScore(1, score, ScoreboardWriteMode.HighScoreDesc);
        Debug.Log(score + "点をアップロードしました。");
        uploaded = true;
    }
}