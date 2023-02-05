using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using NaughtyAttributes;

public class VampireLordController : PlayerController
{
    [SerializeField] private LayerMask _graveLayer;

    private Grave _currentGrave;
    private bool _isTouchingGrave = false;


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
        _playerState = Idle;
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

    protected override void Initialize()
    {
        _playerControls = new PlayerControls();
        _playerState = Idle;
    }
    protected override void LaterInitialize()
    {
        // do nothing for now
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isTouchingGrave = collision.IsTouchingLayers(_graveLayer);

        if (_isTouchingGrave)
            _currentGrave = collision.GetComponent<Grave>();
    }

    protected override void Bite(InputAction.CallbackContext biteContext)
    {
        if (_isTouchingGrave)
        {
            GameManager.Instance.ChosenEngraved = _currentGrave.EngravedVillager;
            GameManager.Instance.InvokeResurrectPlayer();
        }
    }

    public override void Die()
    {
        Debug.Log("lol vampireLord died how? you lil hacker ;)");
    }
}
