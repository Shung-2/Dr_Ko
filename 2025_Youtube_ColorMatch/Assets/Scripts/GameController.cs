using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private BlockSpawner        blockSpawner;
    [SerializeField]
    private UIController        uiController;
    [HideInInspector] 
    public UnityAction<float>   OnMoveSpeedChanged;

    private readonly float      baseSpawnTime = 2.5f;
    private readonly float      spawnSpeedDelta = 0.1f;
    private readonly float      changeSpawnCycle = 10f;
    private float               lastChangeSpawnTime;
    private int                 currentScore;
    private AudioSource         audioSource;
    
    // 깜빡임 효과 관련 변수들
    private readonly float      baseBlinkDuration   = 2.0f; // 기본 깜빡임 주기 (2초)
    private readonly float      minBlinkDuration    = 1.0f; // 최소 깜빡임 주기 (1초)
    private float               blinkDuration;              // 현재 적용될 깜빡임 주기
    public  float               BlinkDuration => blinkDuration;

    [field: SerializeField] 
    public  float   GameSpeed   { get; private set; } = 1f;         // 게임 속도 1배, 1.1배, ...
    [field: SerializeField]
    public  float   SpawnTime   { get; private set; }               // Block 생성 주기
    [field: SerializeField]
    public  float   MoveSpeed   { get; private set; } = 1f;         // Block 이동 속도
    public  bool    IsGamePlay  { get; private set; } = false;
    public  bool    IsGameOver  { get; private set; } = false;

    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            uiController.UpdateScore(currentScore);
            // 깜빡임 주기 계산
            UpdateBlinkDifficulty();
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Reset();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        
        while (true)
        {
            if (Utils.IsAnyInputDown())
            {
                IsGamePlay = true;
                uiController.GameStart();
                
                // 코루틴 중지
                yield break;
            }

            yield return null;
        }
    }

    private void Reset()
    {
        IsGamePlay      = false;
        IsGameOver      = false;
        GameSpeed       = 1f;
        SpawnTime       = baseSpawnTime / GameSpeed;
        MoveSpeed       = 1f / SpawnTime;
        blinkDuration   = baseBlinkDuration;
    }

    private void Update()
    {
        if (IsGamePlay == false)
        {
            return;
        }
        
        // changeSpawnCycle 시간이 지날 때마다 Block 생성 주기(SpawnTime) 변경
        if (Time.time - lastChangeSpawnTime >= changeSpawnCycle)
        {
            SpawnTime = baseSpawnTime / GameSpeed;
            MoveSpeed = 1f / SpawnTime;
            GameSpeed += spawnSpeedDelta;
            OnMoveSpeedChanged?.Invoke(MoveSpeed);
            lastChangeSpawnTime = Time.time;
        }
    }

    private void UpdateBlinkDifficulty()
    {
        if (currentScore <= 50)
        {
            blinkDuration = baseBlinkDuration;
            return;
        }
        
        // 50점을 초과한 점수에 대해 10점마다 0.1초씩 감소
        int scoreTier = (currentScore - 51) / 10;
        float reduction = scoreTier * 0.1f;
        
        // Mathf.Max를 사용하여 최소 주기 이하로 내려가지 않도록 보정
        blinkDuration = Mathf.Max(minBlinkDuration, blinkDuration - reduction);
    }

    public void GameOver()
    {
        IsGameOver = true;
        audioSource.Play();
        
        int best = PlayerPrefs.GetInt(Constants.BestScore);
        if (currentScore > best)
        {
            best = currentScore;
            PlayerPrefs.SetInt(Constants.BestScore, best);
        }
        
        blockSpawner.ClearAll();
        uiController.GameOver(currentScore, best);
        StartCoroutine(ReloadScene());
    }
    
    private IEnumerator ReloadScene()
    {
        while (true)
        {
            if (Utils.IsAnyInputDown())
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                yield break;
            }

            yield return null;
        }
    }
}
