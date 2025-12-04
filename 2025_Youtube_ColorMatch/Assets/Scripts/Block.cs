using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    private GameController      gameController;
    private BlockSpawner        blockSpawner;
    private SpriteRenderer      spriteRenderer;
    private MovementTransform   movement;
    private AudioSource         audioSource;
    private ScaleEffect         scaleEffect;
    private bool                isHittable;

    public  Color Color => spriteRenderer.color;

    private void Awake()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        movement        = GetComponent<MovementTransform>();
        audioSource     = GetComponent<AudioSource>();
        scaleEffect     = GetComponent<ScaleEffect>();
    }

    public void Setup(GameController gameController, BlockSpawner blockSpawner, Color color)
    {
        this.gameController     = gameController;
        this.blockSpawner       = blockSpawner;
        spriteRenderer.color    = color;
        movement.MoveSpeed      = gameController.MoveSpeed;
        transform.localScale    = Vector3.one * 0.8f;
        isHittable              = false;
    }

    public void CorrectAction()
    {
        isHittable = true;
        // 큐에서 제거
        blockSpawner.RemoveFromQueue();
        // 이동 속도, 사운드, 애니메이션 재생
        movement.MoveSpeed = 0f;
        audioSource.Play();
        scaleEffect.Play(transform.localScale, transform.localScale * 1.5f, 0.5f);
        // Fade Out 애니메이션 재생
        StartCoroutine(FadeEffect.Fade(spriteRenderer, 1f, 0f, 0.5f,
            () => { blockSpawner.DeactiveBlock(gameObject); }));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHittable == true || gameController.IsGameOver == true)
        {
            return;
        }
        
        if (collision.CompareTag("DeadLine"))
        {
            gameController.GameOver();
        }
    }
}
