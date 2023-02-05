using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using NaughtyAttributes;

public class VampireLordController : PlayerController
{
    [SerializeField] private LayerMask _graveLayer;

    private GraveTomb _currentGraveTomb;
    public GraveTomb CurrentGraveTomb { get => _currentGraveTomb; set => _currentGraveTomb = value; }

    [SerializeField] private bool _isTouchingGrave = false;
    public bool IsTouchingGrave { get => _isTouchingGrave; set => _isTouchingGrave = value; }


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

    protected override void Bite(InputAction.CallbackContext biteContext)
    {
        if (_isTouchingGrave)
        {
            GameManager.Instance.ChosenEngraved = _currentGraveTomb.EngravedVillagerData;
            GameManager.Instance.InvokeResurrectPlayer();
        }
    }

    public override void Die()
    {
        Debug.Log("lol vampireLord died how? you lil hacker ;)");
    }
}
