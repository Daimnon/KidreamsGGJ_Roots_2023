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

    private int _hp;
    
    [SerializeField] private Animator _anim;
    [SerializeField] private BreatheMoveAnim moveStaggerAnim;
    
    [Header("Raycasting")]
    [SerializeField] private LayerMask _playerRaycastMask;
    [SerializeField] private SpriteDirection _spriteDir;
    [SerializeField] private bool _canAttack;

    [Header("Test/Debug")]
    [SerializeField] private bool _showGizmos;
    [SerializeField] private Color _testRayColor;

    [SerializeField] private EntityNavigation.NavigationMode _startNavMode;
    private EntityNavigation _navigation;
    public EntityData Data { get; private set; }

    private readonly RaycastHit2D[] _raycastResultsCache = new RaycastHit2D[100];
    private PlayerController _cachedPlayer;
    private bool _playerInSight;

    [ShowNonSerializedField] private EntityState _state;
    private Action _updateAction;
    private Transform CachedPlayerTransform => _cachedPlayer ? _cachedPlayer.transform : null;
    
    private EntityState PlayerSeenState =>
        _canAttack
            ? EntityState.ChasingPlayer
            : EntityState.RunningFromPlayer;

    
    public static event Action<Entity> OnEntityDeath;
    
    private void Awake()
    {
        InitData();
        Data.OnValidated += OnValidate;
        InitState();
        _hp = Data.Hp;
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
        Data = GetComponent<EntityDataHolder>().Data;
        _navigation = GetComponent<EntityNavigation>();
        _updateAction = UpdateIdleState;

        if (Application.isPlaying)
        {
            _navigation.Speed = Data.Speed;
        }
    }

    private void InitState()
    {
        moveStaggerAnim.enabled = false;
        
        if (_startNavMode == EntityNavigation.NavigationMode.MoveToPlayer)
        {
            _cachedPlayer = FindObjectOfType<PlayerController>();
        }

        _navigation.SetState(CachedPlayerTransform, _startNavMode);
        
        TransitionToIdle(EntityState.Idle);
    }

    private void Update()
    {
        UpdatePlayerInSight();
        _updateAction();
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;
     
        if (!Data) InitData(); // hack for edit mode
        Gizmos.color = _testRayColor;
        foreach (var rayDir in GetRayDirections())
        {
            Gizmos.DrawRay(transform.position, rayDir * Data.ViewDistance);
        }
    }

    /// <param name="damage">true if entity died</param>
    public bool TakeDamage(int damage)
    {
        _hp -= damage;
        if (_hp == 0)
        {
            Kill();
            return true;
        }

        return false;
    }

    private void Kill()
    {
        OnEntityDeath?.Invoke(this);
        Destroy(this);
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
            var size = Physics2D.RaycastNonAlloc(transform.position, rayDir, _raycastResultsCache, Data.ViewDistance, _playerRaycastMask);
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
        GetRayDirections(_spriteDir.Vector, Data.ViewFOVAngle, Data.NumRays);

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

    private bool IsPlayerInAttackRange()
    {
        var distToPlayer = Vector2.Distance(CachedPlayerTransform.position, transform.position);
        var isInAttackRange = distToPlayer < Data.AttackRange;
        return isInAttackRange;
    }
}
