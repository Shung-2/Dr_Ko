using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField] 
    private float moveSpeed = 1f;

    [SerializeField] 
    private Vector3 moveDirection = Vector3.down;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value);
    }

    void Update()
    {
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }
}
