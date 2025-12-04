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

    private readonly float      baseSpawnTime = 2f;
    private readonly float      spawnSpeedDelta = 0.1f;
    private readonly float      changeSpawnCycle = 10f;
    private float               lastChangeSpawnTime;
    private int                 currentScore;
    private AudioSource         audioSource;

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
        IsGamePlay  = false;
        IsGameOver  = false;
        GameSpeed   = 1f;
        SpawnTime   = baseSpawnTime / GameSpeed;
        MoveSpeed   = 1f / SpawnTime;
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
