using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private readonly int DirectionHash = Animator.StringToHash("Direction");

    private CharacterMovement _movement;
    private Animator _anim;
    private SpriteRenderer _sprite;
    private int _direction; // 0 아래, 1 좌우, 2 위

    private void Awake()
    {
        _movement = GetComponent<CharacterMovement>();
        _anim = GetComponentInChildren<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _movement.OnMoveInputChanged += Apply;
    }

    private void OnDisable()
    {
        _movement.OnMoveInputChanged -= Apply;
    }

    private void Apply(Vector2 input)
    {
        bool isMoving = input.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                _direction = input.y > 0 ? 2 : 0;
            }
            else
            {
                _direction = 1;
                _sprite.flipX = input.x > 0;
            }
        }

        if (_anim.runtimeAnimatorController == null) 
            return;
        _anim.SetBool(IsMovingHash, isMoving);
        _anim.SetInteger(DirectionHash, _direction);
    }
}