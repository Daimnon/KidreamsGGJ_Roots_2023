using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

public enum PlayerStates { Idle, Moving, Biting }

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private SpriteRenderer _playerGraphics;
    [SerializeField] private BoxCollider2D _bodyCollider;
    [SerializeField] private Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;

    [Header("Player Data")]
    [SerializeField, Expandable] private PlayerData _data;
    [SerializeField] private float _biteDistance;

    [Header("World Data")]
    [SerializeField] private LayerMask _biteLayer;
    [SerializeField] private GameObject _body;

    private Vector2 _moveInput;
    private InputAction _move, _bite;
    private bool _isLookingRight = true;

    private delegate void State();
    private State _state;

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
        _state = Idle;
        _playerControls = new PlayerControls();
    }
    private void Update()
    {
        _state.Invoke();
        _moveInput = _move.ReadValue<Vector2>();
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
    private void Move()
    {
        Vector2 direction = new(_moveInput.x, _moveInput.y);
        _rb.velocity = _data.CalculatedSpeed * Time.fixedDeltaTime * direction;

        if (_moveInput.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _isLookingRight = false;
        }
        else if (_moveInput.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            _isLookingRight = true;
        }
    }
    #endregion

    private void Bite(InputAction.CallbackContext biteContext)
    {
        _playerGraphics.sprite = _data.WeakSprite ? _data.StrongSprite : _data.WeakSprite;
        Vector2 direction = _isLookingRight ? Vector2.right : Vector2.left;

        if (Physics2D.Raycast(transform.position, direction, _biteDistance, _biteLayer))
        {
            
            Debug.Log($"player {name} bite");
        }
    }

    #region States
    private void Idle()
    {

    }
    private void Moving()
    {

    }
    private void Biting()
    {

    }
    #endregion

    public void ChangeState(PlayerStates newState)
    {
        switch (newState)
        {
            case PlayerStates.Idle:
                _state = Idle;
                break;
            case PlayerStates.Moving:
                _state = Moving;
                break;
            case PlayerStates.Biting:
                _state = Biting;
                break;
        }
    }
}
