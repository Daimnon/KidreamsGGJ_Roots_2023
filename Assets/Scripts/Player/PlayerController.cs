using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using System;

public enum PlayerStates { Idle, Moving, Attacking, Biting, Eating, FailedBiting }

public class PlayerController : MonoBehaviour
{
    // state machine decleration
    protected delegate void PlayerState();
    protected PlayerState _playerState;

    [Header("Player Data")]
    [SerializeField, Expandable] private PlayerData _data;
    public PlayerData Data { get => _data; set => value = _data; }

    private bool _isWeak;
    public bool IsWeak => _isWeak;

    // TODO: Set kills/absorption absorbed entity when bite person / resurrect from grave
    [ShowNativeProperty] public int Hp => StatHelper.GetHp(_data, _damageTaken, _killedEntitiesData, AbsorbedEntity, _data.CommonData);
    //[ShowNativeProperty] public int Damage => StatHelper.GetDamage(_data, _killedEntities, AbsorbedEntity, _data.CommonData);
    [ShowNativeProperty] public int Speed => StatHelper.GeSpeed(_data, _killedEntitiesData, AbsorbedEntity, _data.CommonData);
    [ShowNativeProperty] public int Vision => StatHelper.GetVision(_data, _killedEntitiesData, AbsorbedEntity, _data.CommonData);
    
    private int _currentStrongValue => Hp + Speed + Vision;

    public EntityData AbsorbedEntity;
    private readonly List<EntityData> _killedEntitiesData = new();
    private int _damageTaken; // separated from hp so we can calculate Absorbed enitty separately

    // TODO: Test (stats work + with absorbed entity)

    [Header("Player Components")]
    [SerializeField] protected PlayerControls _playerControls;
    [SerializeField] protected SpriteRenderer _playerGraphics;
    //[SerializeField] private SpriteDirection _spriteDir;
    [SerializeField] protected Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;

    [SerializeField] private BreatheMoveAnim _moveAnim;
    [SerializeField] private bool _debugPlayerState;
    //[SerializeField] private SpriteDirection _spriteDir;

    [Header("World Data")]
    [SerializeField] private LayerMask _biteLayer;

    private Entity _lastPrey;
    private Vector2 _lastAttackingOriginPos, _lastTargetPos;
    private int _hpStatCounter = 0, _speedStatCounter = 0, _visionStatCounter = 0;
    private float _moveToTargetDuration, _moveBackFromTargetDuration, _biteOffset;

    protected Vector2 _moveInput, _spriteDirection;
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
        LaterInitialize();
    }
    private void Update()
    {
        _playerState.Invoke();

        if (_currentStrongValue >= _data.StrongValue)
            _isWeak = false;
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
    private void OnValueChangedInMoveInput()
    {
        //if (DetectValueChangeInMoveInput())
        //{
        //    
        //
        //    
        //    _biteOffset = _data.BiteOffset * _spriteDirection.x;
        //}
        
        //if (DetectValueChangeInIsWeak())
        //{
        //    switch (IsWeak)
        //    {
        //        case true:
        //            _playerGraphics.sprite = _data.WeakSprite;
        //            _moveToTargetDuration = _data.MoveToTargetDurationWhileWeak;
        //            _moveBackFromTargetDuration = _data.MoveBackFromTargetDurationWhileWeak;
        //            break;
        //        case false:
        //            _playerGraphics.sprite = _data.StrongSprite;
        //            _moveToTargetDuration = _data.MoveToTargetDurationWhileStrong;
        //            _moveBackFromTargetDuration = _data.MoveBackFromTargetDurationWhileStrong;
        //            break;
        //    }
        //}
    }
    #endregion

    private void Initialize()
    {
        _playerControls = new PlayerControls();
        _isWeak = _data.IsWeak;
        _moveToTargetDuration = _data.MoveToTargetDurationWhileWeak;
        _moveBackFromTargetDuration = _data.MoveBackFromTargetDurationWhileWeak;
        _biteOffset = _data.BiteOffset;
        UIManager.Instance.InitializePlayerUI();
        _playerState = Idle;
    }
    private void LaterInitialize()
    {
        GameManager.Instance.UnderworldOverlay.SetRegularMode();
        GameManager.Instance.PlayerController = this;
    }

    #region FixedUpdate Methods
    protected void Move()
    {
        Vector2 direction = new(_moveInput.x, _moveInput.y);
        _rb.velocity = _data.CalculatedSpeed * Time.fixedDeltaTime * direction;

        FlipSpriteToMoveDirection();
    }
    #endregion

    protected virtual void Bite(InputAction.CallbackContext biteContext)
    {
        //Vector2 direction = _spriteDir.Vector;
        //_playerGraphics.sprite = _isWeak ? _data.WeakSprite : _data.StrongSprite;

        _lastPrey = null;
        _lastAttackingOriginPos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _spriteDirection, _data.BiteDistance, _biteLayer);

        if (hit)
        {
            _lastPrey = hit.transform.GetComponentInParent<Entity>();

            if (!_lastPrey)
                return;

            Vector2 pos = (Vector2)transform.position + _spriteDirection * _data.BiteDistance;
            _lastTargetPos = new(pos.x, hit.transform.position.y);


            //_moveToTargetDuration = _isWeak ? _data.MoveToTargetDurationWhileWeak : _data.MoveToTargetDurationWhileStrong;
            //_moveBackFromTargetDuration = _isWeak ? _data.MoveBackFromTargetDurationWhileWeak : _data.MoveBackFromTargetDurationWhileStrong;


            ChangeState(PlayerStates.Attacking);
            _lastPrey.CaptureEntity();

            //bool isEntityAlive = ; // instakill
            if (_lastPrey is Villager && GameManager.Instance.Engraved.Count < 5)
                GameManager.Instance.Engraved.Add(_lastPrey as Villager);

            if (!_lastPrey.TakeDamage(_lastPrey.Data.Hp))
                _killedEntitiesData.Add(_lastPrey.Data);
            
                

            //_absorbedEntites.Add(entity.Data);

            transform.DOMove(_lastTargetPos + new Vector2(_data.BiteOffset, 0f), _moveToTargetDuration).SetEase(_data.MoveToTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Biting));

            Debug.Log($"player {name} bite {hit.collider.gameObject.name}");

        }
        else
        {
            Vector2 pos = (Vector2)transform.position + _spriteDirection * _data.BiteDistance;
            _lastTargetPos = new(pos.x, transform.position.y);

            ChangeState(PlayerStates.Attacking);
            //Vector2 originalPos = transform.position;
            //Vector2 pos = (Vector2) transform.position + _spriteDirection * _data.BiteDistance;
            //float targetPosX = pos.x;
            
            //float moveToTarget = _isWeak ? _data.MoveToTargetDurationWhileWeak : _data.MoveToTargetDurationWhileStrong;
            //float moveBackFromTarget = _isWeak ? _data.MoveBackFromTargetDurationWhileWeak / 2 : _data.MoveBackFromTargetDurationWhileStrong / 2;

            transform.DOMoveX(_lastTargetPos.x, _moveToTargetDuration).SetEase(_data.MoveToTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.FailedBiting));

            //DOTween.Sequence().
            //    Append(transform.DOMoveX(targetPosX, moveToTarget).SetEase(_data.MoveToTargetCurveFailedBite)).
            //    Append(transform.DOMoveX(originalPos.x, moveBackFromTarget).SetEase(_data.MoveBackFromTargetCurveFailedBite)).
            //    OnComplete(() => ChangeState(PlayerStates.Idle));

            Debug.Log($"player {name} didn't bite");
        }
    }

    #region States
    protected void Idle()
    {
        if (_debugPlayerState) Debug.Log($"player state is Idle");

        _moveInput = _move.ReadValue<Vector2>();
        _lastPrey = null;

        if (_playerGraphics.sprite == _data.WeakAttackingSprite || _playerGraphics.sprite == _data.StrongAttackingSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakSprite : _data.StrongSprite;

        if (_moveInput != Vector2.zero)
            ChangeState(PlayerStates.Moving);
    }
    protected void Moving()
    {
        if (_debugPlayerState) Debug.Log($"player state is Moving");

        _moveInput = _move.ReadValue<Vector2>();

        var hasInput = _moveInput != Vector2.zero;
        if (!hasInput)
            ChangeState(PlayerStates.Idle);
        _moveAnim.enabled = hasInput;
    }
    protected void Attacking()
    {
        if (_debugPlayerState) Debug.Log($"player state is Attacking");

        _moveInput = Vector2.zero;

        if (_playerGraphics.sprite != _data.WeakAttackingSprite && _playerGraphics.sprite != _data.StrongAttackingSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakAttackingSprite : _data.StrongAttackingSprite;

        ChangeState(PlayerStates.Biting);
    }
    protected void Biting()
    {
        if (_debugPlayerState) Debug.Log($"player state is Biting");

        _moveInput = Vector2.zero;

        if (!_lastPrey)
            return;

        if (_playerGraphics.sprite == _data.WeakAttackingSprite || _playerGraphics.sprite == _data.StrongAttackingSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[0] : _data.StrongEatingAnimation[0];

        if (_playerGraphics.sprite == _data.WeakEatingAnimation[0] || _playerGraphics.sprite == _data.StrongEatingAnimation[0])
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[1] : _data.StrongEatingAnimation[1];
        else
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[0] : _data.StrongEatingAnimation[0];

        if (_lastPrey is not Villager)
        {
            transform.DOMove(_lastAttackingOriginPos, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Eating));
        }
        else if (_lastPrey is Villager)
        {
            transform.DOMove(_lastAttackingOriginPos, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Idle));
        }
    }
    protected void Eating()
    {
        if (_debugPlayerState) Debug.Log($"player state is Eating");

        _moveInput = Vector2.zero;

        if (!_lastPrey)
            return;

        if (_playerGraphics.sprite == _data.WeakAttackingSprite || _playerGraphics.sprite == _data.StrongAttackingSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[0] : _data.StrongEatingAnimation[0];

        if (_playerGraphics.sprite == _data.WeakEatingAnimation[0] || _playerGraphics.sprite == _data.StrongEatingAnimation[0])
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[1] : _data.StrongEatingAnimation[1];
        else
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[0] : _data.StrongEatingAnimation[0];

        transform.DOMove(_lastAttackingOriginPos, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Idle));

        //if (_playerGraphics.sprite != _data.WeakEatingAnimation[0] && _playerGraphics.sprite != _data.WeakEatingAnimation[1])
        //    _playerGraphics.sprite = _data.WeakEatingAnimation[0];
        //else if (_playerGraphics.sprite != _data.WeakEatingAnimation[0] && _playerGraphics.sprite != _data.WeakEatingAnimation[1])
        //
        //

        //_lastTargetPos = null;
        //_lastAttackingPos = null;
    }
    protected void FailedBiting()
    {
        if (_debugPlayerState) Debug.Log($"player state tried to Bite and failed");
        _moveInput = Vector2.zero;

        if (_playerGraphics.sprite == _data.WeakAttackingSprite || _playerGraphics.sprite == _data.StrongAttackingSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakSprite : _data.StrongSprite;

        transform.DOMoveX(_lastAttackingOriginPos.x, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveFailedBite).
            OnComplete(() => ChangeState(PlayerStates.Idle));
    }
    #endregion

    private void FlipSpriteToMoveDirection()
    {
        if (_moveInput.x < 0)
        {
            _spriteDirection = new(-Mathf.Abs(transform.localScale.x), 0);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (_moveInput.x > 0)
        {
            _spriteDirection = new(Mathf.Abs(transform.localScale.x), 0);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

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
            case PlayerStates.Attacking:
                _playerState = Attacking;
                break;
            case PlayerStates.Biting:
                _playerState = Biting;
                break;
            case PlayerStates.Eating:
                _playerState = Eating;
                break;
            case PlayerStates.FailedBiting:
                _playerState = FailedBiting;
                break;
        }
    }

    public virtual void Kill()
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
