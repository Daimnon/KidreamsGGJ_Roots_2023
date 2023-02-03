using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public enum FaceDirection
{
    Left,
    Right,
}

[RequireComponent(typeof(EntityDataHolder))]
public partial class Entity : MonoBehaviour
{
    private enum EntityState
    {
        Idle,
        ChasingPlayer,
        RunningFromPlayer,
        Attacking,
    }
    
    [SerializeField] private Animator _anim;
    [SerializeField] private BreatheMoveAnim moveStaggerAnim;
    
    [Header("Raycasting")]
    [SerializeField] private LayerMask _playerRaycastMask;
    [SerializeField] private SpriteDirection _spriteDir;
    [SerializeField] private bool _attack;

    [Header("Test/Debug")]
    [SerializeField] private bool _showGizmos;
    [SerializeField] private Color _testRayColor;

    [SerializeField] private EntityNavigation.NavigationMode _startNavMode;
    private EntityNavigation _navigation;
    private EntityData _entityData;

    private readonly RaycastHit2D[] _raycastResultsCache = new RaycastHit2D[100];
    private PlayerController _cachedPlayer;
    private bool _playerInSight;

    [ShowNonSerializedField] private EntityState _state;
    private Action _updateAction;


    private void Awake()
    {
        InitData();
        _entityData.OnValidated += OnValidate;
        InitState();
    }

    private void OnValidate()
    {
        if (_spriteDir == null) _spriteDir = GetComponentInChildren<SpriteDirection>();
        if (_anim == null) _anim = GetComponentInChildren<Animator>();
        if (moveStaggerAnim == null) moveStaggerAnim = GetComponentInChildren<BreatheMoveAnim>();
        
        InitData();
    }

    private void InitData()
    {
        _entityData = GetComponent<EntityDataHolder>().Data;
        _navigation = GetComponent<EntityNavigation>();
        _updateAction = UpdateIdleState;

        if (Application.isPlaying)
        {
            _navigation.Speed = _entityData.Speed;
        }
    }

    private void InitState()
    {
        moveStaggerAnim.enabled = false;
        
        if (_startNavMode == EntityNavigation.NavigationMode.MoveToPlayer)
        {
            _cachedPlayer = FindObjectOfType<PlayerController>();
        }

        var playerTrans = _cachedPlayer != null ? _cachedPlayer.transform : null;
        _navigation.SetState(playerTrans, _startNavMode);
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;
     
        if (!_entityData) InitData(); // hack for edit mode
        Gizmos.color = _testRayColor;
        foreach (var rayDir in GetRayDirections())
        {
            Gizmos.DrawRay(transform.position, rayDir * _entityData.ViewDistance);
        }
    }

    private void Update()
    {
        UpdatePlayerInSight();
        _updateAction();
    }
    
    private void UpdatePlayerInSight()
    {
        var player = RayCastForPlayer();
        var foundPlayer = player != null;
        if (player) _cachedPlayer = player;
        if (foundPlayer && !_playerInSight)
        {
            OnPlayerFound();
        }
        else if (!foundPlayer && _playerInSight)
        {
            OnPlayerLost();
        }

        _playerInSight = foundPlayer;
    }

    private void OnPlayerFound()
    {
        Debug.Log(LogStr("Found player!!!!"));
    }

    private void OnPlayerLost()
    {
        Debug.Log(LogStr("Where player???!!"));
    }

    private PlayerController RayCastForPlayer()
    {
        foreach (var rayDir in GetRayDirections())
        {
            var size = Physics2D.RaycastNonAlloc(transform.position, rayDir, _raycastResultsCache, _entityData.ViewDistance, _playerRaycastMask);
            if (size == 0) continue;
            
            var player = SearchPlayerInResults(_raycastResultsCache);
            if (player != null) return player;
        }

        return null;
    }

    private PlayerController SearchPlayerInResults(RaycastHit2D[] results)
    {
        if (results == null || results.Length == 0) return null;
        return results
            .Select(res =>
            {
                if (_cachedPlayer && _cachedPlayer.gameObject == res.transform.gameObject)
                    return _cachedPlayer;
                return res.transform.GetComponent<PlayerController>();
            })
            .FirstOrDefault();
    }

    private IEnumerable<Vector3> GetRayDirections() =>
        GetRayDirections(_spriteDir.Vector, _entityData.ViewFOVAngle, _entityData.NumRays);

    private static IEnumerable<Vector3> GetRayDirections(Vector2 direction, float angleDegrees, int numRays)
    {
        var deltaRot = Quaternion.AngleAxis(angleDegrees / numRays, Vector3.forward);
        var curVec = Quaternion.AngleAxis(angleDegrees * 0.5f, Vector3.back) * direction;

        for (var i = 0; i < numRays; i++)
        {
            yield return curVec;
            curVec = deltaRot * curVec;
        }
    }

    private string LogStr(string msg) => $"{nameof(Entity)}: {msg}";
}
