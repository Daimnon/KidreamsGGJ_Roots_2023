using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EntityNavigation)), RequireComponent(typeof(EntityDataHolder))]
public class Entity : MonoBehaviour
{
    [Header("Raycasting")]
    [SerializeField] private LayerMask _playerRaycastMask;

    [Header("Test/Debug")]
    [SerializeField] private bool _showGizmos;
    [SerializeField] private Color _testRayColor;
    [SerializeField] private float _testRayDistance;
    [SerializeField] private float _testFovDegrees;
    [SerializeField] private int _testNumRays;
    
    private EntityNavigation.NavigationMode _startNavMode;
    private EntityNavigation _navigation;
    private EntityData _entityData;

    private readonly RaycastHit2D[] _raycastResultsCache = new RaycastHit2D[100];
    private PlayerController _cachedPlayer;
    private bool _playerInSight;

    private void Awake()
    {
        _entityData = GetComponent<EntityDataHolder>().Data;
        _navigation = GetComponent<EntityNavigation>();
    }

    private void OnDrawGizmos()
    {
        if (!_entityData) _entityData = GetComponent<EntityDataHolder>().Data; // hack for edit mode
        if (!_showGizmos) return;
        Gizmos.color = _testRayColor;
        foreach (var rayDir in GetRayDirections(_entityData.ViewFOVAngle, _entityData.NumRays))
        {
            Gizmos.DrawRay(transform.position, rayDir * _entityData.ViewDistance);
        }
    }

    private void Update()
    {
        UpdatePlayerInSight();
    }

    private void UpdatePlayerInSight()
    {
        var player = RayCastForPlayer(_testFovDegrees, _testRayDistance, _testNumRays);
        var foundPlayer = player != null;
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

    private PlayerController RayCastForPlayer(float fovAngleDeg, float rayDistance, int numRays)
    {
        foreach (var rayDir in GetRayDirections(fovAngleDeg, numRays))
        {
            var size = Physics2D.RaycastNonAlloc(transform.position, rayDir, _raycastResultsCache, rayDistance, _playerRaycastMask);
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

    private IEnumerable<Vector3> GetRayDirections(float angleDegrees, int numRays)
    {
        var deltaRot = Quaternion.AngleAxis(angleDegrees / numRays, Vector3.forward);
        var curVec = Quaternion.AngleAxis(angleDegrees * 0.5f, Vector3.back) * transform.up;

        for (var i = 0; i < numRays; i++)
        {
            yield return curVec;
            curVec = deltaRot * curVec;
        }
    }

    private string LogStr(string msg) => $"{nameof(Entity)}: {msg}";
}
