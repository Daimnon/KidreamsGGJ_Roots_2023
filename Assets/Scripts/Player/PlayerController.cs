using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using NaughtyAttributes;

public enum PlayerStates { Idle, Moving, Biting }

public class PlayerController : MonoBehaviour
{
    protected delegate void PlayerState();
    protected PlayerState _playerState;

    [Header("Player Components")]
    [SerializeField] protected PlayerControls _playerControls;
    [SerializeField] protected SpriteRenderer _playerGraphics;
    [SerializeField] protected Rigidbody2D _rb;
    
    public Rigidbody2D Rb => _rb;

    [Header("Player Data")]
    [SerializeField, Expandable] private PlayerData _data;
    public PlayerData Data { get => _data; set => value = _data; }

    [Header("World Data")]
    [SerializeField] private LayerMask _biteLayer;

    protected Vector2 _moveInput;
    protected InputAction _move, _bite;
    protected bool _isLookingRight = true;

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        _move = _playerControls.Player.Move;
        _move.Enable();

        _bite = _playerControls.Player.Bite;
        _bite.Enable();
        _bite.started += Bite;
    }
    private void Awake()
    {
        _playerControls = new PlayerControls();
        GameManager.Instance.CurrentPlayer = gameObject;
        GameManager.Instance.PlayerController = this;
        _playerState = Idle;
    }
    private void Update()
    {
        _playerState.Invoke();

        if (_data.Hp <= 0)
            Kill();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnDisable()
    {
        _move.Disable();
        _bite.Disable();
    }
    #endregion

    #region FixedUpdate Methods
    protected void Move()
    {
        Vector2 direction = new(_moveInput.x, _moveInput.y);
        _rb.velocity = _data.CalculatedSpeed * Time.fixedDeltaTime * direction;
        // Moved Sprite direction logic to SpriteDirectionFromVelocity for reuse (in Entity)
    }
    #endregion

    protected void Bite(InputAction.CallbackContext biteContext)
    {
        Vector2 direction = _isLookingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _data.BiteDistance, _biteLayer);
        if (hit)
        {
            ChangeState(PlayerStates.Biting);
            Vector2 originalPos = transform.position;
            Vector2 targetPos = new (transform.position.x + _data.BiteDistance - _data.BiteOffset, hit.transform.position.y);

            float moveToTarget = _playerGraphics == _data.WeakSprite ? _data.MoveToTargetDurationWhileWeak : _data.MoveToTargetDurationWhileStrong;
            float moveBackFromTarget = _playerGraphics == _data.WeakSprite ?_data.MoveBackFromTargetDurationWhileWeak :_data.MoveBackFromTargetDurationWhileStrong;

            DOTween.Sequence().
                Append(transform.DOMove(targetPos, moveToTarget).SetEase(_data.MoveToTargetCurve)).
                Append(transform.DOMove(originalPos, moveBackFromTarget).SetEase(_data.MoveBackFromTargetCurve).SetDelay(_data.BiteTime)).
                OnComplete(() => ChangeState(PlayerStates.Idle));
            
            Debug.Log($"player {name} bite {hit.collider.gameObject.name}");
        }
    }

    #region States
    protected void Idle()
    {
        Debug.Log($"player state is Idle");

        _moveInput = _move.ReadValue<Vector2>();

        if (_moveInput != Vector2.zero)
            ChangeState(PlayerStates.Moving);
    }
    protected void Moving()
    {
        Debug.Log($"player state is Moving");

        _moveInput = _move.ReadValue<Vector2>();

        if (_moveInput == Vector2.zero)
            ChangeState(PlayerStates.Idle);
    }
    protected void Biting()
    {
        _moveInput = Vector2.zero;
        Debug.Log($"player state is Biting");
    }
    #endregion

    public void ChangeState(PlayerStates newState)
    {
        switch (newState)
        {
            case PlayerStates.Idle:
                _playerState = Idle;
                break;
            case PlayerStates.Moving:
                _playerState = Moving;
                break;
            case PlayerStates.Biting:
                _playerState = Biting;
                break;
        }
    }

    private void Kill()
    {
        GameManager.Instance.ChangeState(GameStates.VampireLordLoop);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // cyan = biteRange
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _data.BiteDistance * Vector2.right);
    }
}
