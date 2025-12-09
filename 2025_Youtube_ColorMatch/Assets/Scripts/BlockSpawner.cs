using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameController      gameController;
    [SerializeField] 
    private Sprite[]            catSprites;
    [SerializeField] 
    private GameObject          blockPrefab;
    [SerializeField] 
    private float[]             spawnX;
    private float               spawnY = 6.5f;

    private float               lastSpawnTime = 0f;
    private MemoryPool          memoryPool;
    private Queue<GameObject>   blockQueue;
    public  Queue<GameObject>   BlockQueue => blockQueue;

    // 블록 깜짝일 확률
    private const float BLINK_PROBABILITY = 0.2f;

    private void Awake()
    {
        memoryPool = new MemoryPool(blockPrefab, 5, transform);
        blockQueue = new Queue<GameObject>();
    }

    private void OnEnable()
    {
        gameController.OnMoveSpeedChanged += HandleMoveSpeedChanged;
    }

    private void OnDisable()
    {
        gameController.OnMoveSpeedChanged -= HandleMoveSpeedChanged;
    }

    private void Update()
    {
        if (gameController.IsGamePlay == false || gameController.IsGameOver == true)
        {
            return;
        }
        
        if (Time.time - lastSpawnTime >= gameController.SpawnTime)
        {
            int xIndex = Random.Range(0, spawnX.Length);
            int spriteIndex = Random.Range(0, catSprites.Length);
            
            var blockGO = memoryPool.ActivatePoolItem(new Vector3(spawnX[xIndex], spawnY, 0f));
            var block = blockGO.GetComponent<Block>();

            // 깜빡임 여부 결정 및 Setup 메소드 호출
            bool shouldBlink = false;
            if (gameController.CurrentScore > 50 && Random.value < BLINK_PROBABILITY)
            {
                shouldBlink = true;
            }
            
            // Block의 Setup에 깜짝임 정보 전달
            block.Setup(gameController, this, spriteIndex, catSprites[spriteIndex], shouldBlink, gameController.BlinkDuration);
            blockQueue.Enqueue(blockGO);
            lastSpawnTime = Time.time;
        }
    }

    public void RemoveFromQueue()
    {
        blockQueue.Dequeue();
    }

    public void DeactiveBlock(GameObject remove)
    {
        memoryPool.DeactivatePoolItem(remove);
    }

    public void ClearAll()
    {
        while (blockQueue != null && blockQueue.Count > 0)
        {
            // Block.CorrectAction()에서 Dequeue()를 하기 때문에 꺼내지 않고, Peek()를 사용한다.
            var go = blockQueue.Peek();
            if (go != null && go.TryGetComponent(out Block block))
            {
                block.CorrectAction();
            }
        }
    }

    private void HandleMoveSpeedChanged(float newSpeed)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).TryGetComponent(out MovementTransform mt))
            {
                mt.MoveSpeed = newSpeed;
            }
        }
    }
}
