using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EntityDataHolder))]
public class EntityNavigation : MonoBehaviour
{
    public enum NavigationMode
    {
        Idle,
        MoveRandomly,
        MoveToPlayer,
    }
    [Header("AI Agent Data and setting")]
    private EntityData _data;
    [NaughtyAttributes.ShowNativeProperty]
    public NavigationMode NavMode { get; private set; }
    private Vector2 XrandomOffset;
    private Vector2 YrandomOffset;
    private NavMeshAgent agent;
    [SerializeField]
    private Transform _playerTransform;
    [Header("RandomRoaming")]
    private float distanceToNextTarget;
    private Vector3 nextRandomTargetPos;

    
    public void SetState(Transform playerTransform, NavigationMode navState)
    {
        if (playerTransform == null && navState == NavigationMode.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
        NavMode = navState;
        _playerTransform = playerTransform;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        NavMode = NavigationMode.MoveRandomly;
        _data = GetComponent<EntityDataHolder>().Data;
    }
    private void Start()
    {
        InitAgent();
    }
    private void Update()
    {
        switch (NavMode)
        {
            case (NavigationMode.MoveRandomly):
                if (agent.remainingDistance <= 3)
                    MoveToNextRandomLocation();
                break;
            case (NavigationMode.MoveToPlayer):
                MoveToPlayer();
                break;
        }
    }
    private void InitAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        nextRandomTargetPos = GetNextTarget();
        agent.SetDestination(new Vector3(nextRandomTargetPos.x, nextRandomTargetPos.y, transform.position.z));
    }
    private void MoveToNextRandomLocation()
    {
        nextRandomTargetPos = GetNextTarget();
        agent.SetDestination(new Vector3(nextRandomTargetPos.x, nextRandomTargetPos.y, transform.position.z));
    }
    private void MoveToPlayer()
    {
        if (_playerTransform == null && NavMode == NavigationMode.MoveToPlayer)
            Debug.LogError("playerTransform is Null");
        else
            Vector2.MoveTowards(transform.position, _playerTransform.position, _data.Speed);
    }

    #region WalkingStateLogic
    private Vector3 GetRandomDir()
    {
        var position = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1),transform.position.z);
        return position;
    }
    private Vector3 GetNextTarget()
    {
        float DistanceToNextTarget = Random.Range(-5, 5);
        return GetRandomDir() * DistanceToNextTarget;
    }

}

    #endregion}