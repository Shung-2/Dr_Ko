using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Main")] 
    [SerializeField] 
    private GameObject      mainPanel;
    
    [Header("InGame")]
    [SerializeField]
    private TextMeshProUGUI textInGameScore;
    
    [Header("GameOver")]
    [SerializeField]
    private GameObject      gameOverPanel;
    [SerializeField]
    private CountingEffect  effectResultScore;
    [SerializeField]
    private TextMeshProUGUI textBestScore;

    public void GameStart()
    {
        mainPanel.SetActive(false);
        textInGameScore.gameObject.SetActive(true);
    }
    
    public void UpdateScore(int score)
    {
        textInGameScore.text = score.ToString();
    }

    public void GameOver(int current, int best)
    {
        textInGameScore.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
        
        // 카운팅 재생시간은 최소 1초로 설정하고, 점수 50당 1초씩 증가 (1~50 : 1초, 51~100 : 2초, ...)
        effectResultScore.Play(0, current, Mathf.Max(1f, Mathf.CeilToInt(current / 50f)));
        textBestScore.text = best.ToString();
    }
}
