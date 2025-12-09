using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchBlock : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameController  gameController;
    [SerializeField] 
    private BlockSpawner    blockSpawner;
    [SerializeField] 
    private int             blockID;
    public int BlockID => blockID; 

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

        if (BlockID.Equals(block.BlockID))
        {
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
