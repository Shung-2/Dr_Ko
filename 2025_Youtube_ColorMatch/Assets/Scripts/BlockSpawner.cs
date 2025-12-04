using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameController      gameController;
    [SerializeField] 
    private Color[]             colors;
    [SerializeField] 
    private GameObject          blockPrefab;
    [SerializeField] 
    private float[]             spawnX;
    private float               spawnY = 6.5f;

    private float               lastSpawnTime = 0f;
    private MemoryPool          memoryPool;
    private Queue<GameObject>   blockQueue;
    public  Queue<GameObject>   BlockQueue => blockQueue;
    

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
            int colorIndex = Random.Range(0, colors.Length);
            
            var block = memoryPool.ActivatePoolItem(new Vector3(spawnX[xIndex], spawnY, 0f));
            block.GetComponent<Block>().Setup(gameController, this, colors[colorIndex]);
            blockQueue.Enqueue(block);
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
