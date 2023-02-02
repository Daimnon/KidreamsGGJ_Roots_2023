using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private BoxCollider2D _bodyCollider;
    [SerializeField] private Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;

    [Header("Player Data")]
    [SerializeField] private float _moveSpeed = 100;

    //[Header("World Data")]
    

    private Vector2 _moveInput;
    private InputAction _move, _bite, _jump;

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
    }
    private void Update()
    {
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
        _rb.velocity = _moveSpeed * Time.fixedDeltaTime * direction;
    }
    #endregion

    private void Bite(InputAction.CallbackContext fireContext)
    {
        Debug.Log($"player {name} fired");
    }
}
