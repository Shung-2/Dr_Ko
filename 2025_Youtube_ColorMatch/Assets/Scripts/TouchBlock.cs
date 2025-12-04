using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchBlock : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameController  gameController;
    [SerializeField] 
    private BlockSpawner    blockSpawner; 
    
    public Color Color { get; private set; }

    private void Awake()
    {
        Color = GetComponent<SpriteRenderer>().color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (blockSpawner.BlockQueue.Count == 0)
        {
            return;
        }

        if (!blockSpawner.BlockQueue.Peek().TryGetComponent(out Block block))
        {
            return;
        }
        
        // 동일 색상일 경우, 점수 획득 및 블록 행동 처리
        if (Color.Equals(block.Color))
        {
            // 점수 증가 & 블록 행동 처리
            gameController.CurrentScore++;
            block.CorrectAction();
        }
        // 다른 색상일 때 게임 오버 처리
        else
        {
            gameController.GameOver();
        }
    }
}
