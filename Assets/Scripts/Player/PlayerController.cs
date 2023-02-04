using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using NaughtyAttributes;

public enum PlayerStates { Idle, Moving, Biting, FailedBiting }

public class PlayerController : MonoBehaviour
{
    protected delegate void PlayerState();
    protected PlayerState _playerState;

    [Header("Player Components")]
    [SerializeField] protected PlayerControls _playerControls;
    [SerializeField] protected SpriteRenderer _playerGraphics;
    [SerializeField] private SpriteDirection _spriteDir;
    [SerializeField] protected Rigidbody2D _rb;
    [SerializeField] private bool _debugPlayerState;
    public Rigidbody2D Rb => _rb;

    [Header("Player Data")]
    [SerializeField, Expandable] private PlayerData _data;
    public PlayerData Data { get => _data; set => value = _data; }

    [SerializeField] private int _engravedAmount;
    private EntityData _absorbedEntity; // TODO: Set absorbed entity when bite person / resurrect from grave
    private int _damageTaken; // separated from hp so we can calculate Absorbed enitty separately
    
    // TODO: Test (stats work + with absorbed entity)
    [ShowNativeProperty] public int Hp => StatHelper.GetHp(_data, _damageTaken, _absorbedEntity);
    [ShowNativeProperty] public int Damage => StatHelper.GetDamage(_data, _absorbedEntity);
    [ShowNativeProperty] public int Speed => StatHelper.GeSpeed(_data, _absorbedEntity);
    [ShowNativeProperty] public int Vision => StatHelper.GetVision(_data, _absorbedEntity);
    public int EngravedAmount { get => _engravedAmount; set => value = _engravedAmount; }

    [Header("World Data")]
    [SerializeField] private LayerMask _biteLayer;

    protected Vector2 _moveInput;
    protected InputAction _move, _bite;

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
        Initialize();
    }
    private void Start()
    {
        GameManager.Instance.CurrentPlayer = gameObject;
        GameManager.Instance.PlayerController = this;
    }
    private void Update()
    {
        _playerState.Invoke();
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

    private void Initialize()
    {
        _playerControls = new PlayerControls();
        _playerState = Idle;
    }

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
        Vector2 direction = _spriteDir.Vector;
        bool isWeak = _playerGraphics.sprite == _data.WeakSprite;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _data.BiteDistance, _biteLayer);
        if (hit)
        {
            ChangeState(PlayerStates.Biting);
            Vector2 originalPos = transform.position;
            Vector2 targetPos = new (transform.position.x + _data.BiteDistance - _data.BiteOffset, hit.transform.position.y);

            float moveToTarget = isWeak ? _data.MoveToTargetDurationWhileWeak : _data.MoveToTargetDurationWhileStrong;
            float moveBackFromTarget = isWeak ?_data.MoveBackFromTargetDurationWhileWeak :_data.MoveBackFromTargetDurationWhileStrong;

            DOTween.Sequence().
                Append(transform.DOMove(targetPos, moveToTarget).SetEase(_data.MoveToTargetCurveBiteSuccess)).
                Append(transform.DOMove(originalPos, moveBackFromTarget).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).SetDelay(_data.BiteTime)).
                OnComplete(() => ChangeState(PlayerStates.Idle));
            
            Debug.Log($"player {name} bite {hit.collider.gameObject.name}");
        }
        else
        {
            ChangeState(PlayerStates.FailedBiting);
            Vector2 originalPos = transform.position;
            Vector2 pos = (Vector2) transform.position + direction * _data.BiteDistance;
            float targetPosX = pos.x;
            
            float moveToTarget = isWeak ? _data.MoveToTargetDurationWhileWeak : _data.MoveToTargetDurationWhileStrong;
            float moveBackFromTarget = isWeak ? _data.MoveBackFromTargetDurationWhileWeak / 2 : _data.MoveBackFromTargetDurationWhileStrong / 2;

            DOTween.Sequence().
                Append(transform.DOMoveX(targetPosX, moveToTarget).SetEase(_data.MoveToTargetCurveFailedBite)).
                Append(transform.DOMoveX(originalPos.x, moveBackFromTarget).SetEase(_data.MoveBackFromTargetCurveFailedBite)).
                OnComplete(() => ChangeState(PlayerStates.Idle));

            Debug.Log($"player {name} didn't bite");
        }
    }

    #region States
    protected void Idle()
    {
        if (_debugPlayerState) Debug.Log($"player state is Idle");

        _moveInput = _move.ReadValue<Vector2>();

        if (_moveInput != Vector2.zero)
            ChangeState(PlayerStates.Moving);
    }
    protected void Moving()
    {
        if (_debugPlayerState) Debug.Log($"player state is Moving");

        _moveInput = _move.ReadValue<Vector2>();

        if (_moveInput == Vector2.zero)
            ChangeState(PlayerStates.Idle);
    }
    protected void Biting()
    {
        _moveInput = Vector2.zero;
        if (_debugPlayerState) Debug.Log($"player state is Biting");
    }
    protected void FailedBiting()
    {
        _moveInput = Vector2.zero;
        if (_debugPlayerState) Debug.Log($"player state tried to Bite and failed");
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
            case PlayerStates.FailedBiting:
                _playerState = FailedBiting;
                break;
        }
    }

    public void Kill()
    {
        Debug.Log("Player: Died!");
        GameManager.Instance.ChangeState(GameStates.VampireLordLoop);
        Destroy(gameObject);
    }

    public bool TakeDamage(int damage)
    {
        bool isAlive;
        _damageTaken += damage;

        Debug.Log($"Player.TakeDamage({damage}). Hp is now {Hp}");
        if (Hp <= 0)
        {
            isAlive = true;
            Kill();
            return isAlive;
        }

        isAlive = false;

        return isAlive;
    }

    private void OnDrawGizmos()
    {
        // cyan = biteRange
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _data.BiteDistance * Vector2.right);
    }
}
