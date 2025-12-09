using System;
using System.Collections;
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
    
    public  int     BlockID     { get; private set; } 

    private void Awake()
    {
        spriteRenderer  = GetComponent<SpriteRenderer>();
        movement        = GetComponent<MovementTransform>();
        audioSource     = GetComponent<AudioSource>();
        scaleEffect     = GetComponent<ScaleEffect>();
    }

    public void Setup(GameController gameController, BlockSpawner blockSpawner, int blockID, Sprite sprite, bool shouldBlink, float blinkDuration)
    {
        this.gameController     = gameController;
        this.blockSpawner       = blockSpawner;
        BlockID                 = blockID;
        spriteRenderer.sprite   = sprite;
        spriteRenderer.color    = Color.white;
        movement.MoveSpeed      = gameController.MoveSpeed;
        transform.localScale    = Vector3.one;
        isHittable              = false;
        
        // 코루틴 정지
        StopAllCoroutines();
        if (shouldBlink)
        {
            StartCoroutine(BlinkProcess(blinkDuration));
        }
    }
    
    // 깜빡임 효과를 처리하는 코루틴
    private IEnumerator BlinkProcess(float duration)
    {
        // isHittable이 true가 되면(플레이어가 맞추면) 코루틴 중단
        while (!isHittable)
        {
            yield return StartCoroutine(FadeEffect.Fade(spriteRenderer, 1f, 0f, duration / 2));
            yield return StartCoroutine(FadeEffect.Fade(spriteRenderer, 0f, 1f, duration / 2));
        }
    }

    public void CorrectAction()
    {
        isHittable = true;
        
        StopAllCoroutines();
        spriteRenderer.color = Color.white;
        
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
