using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntityDataHolder))]
public class EntityNavigation : MonoBehaviour
{
    public enum NavigationMode
    {
        MoveRandomly,
        MoveToPlayer,
        RunFromPlayer,
    }
    [Header("AI Agent Data and setting")]
    [SerializeField] private NavMeshAgent agent;
    private EntityData _data;

    [NaughtyAttributes.ShowNativeProperty]
    public NavigationMode NavMode { get; private set; }
    
    [SerializeField] private Transform _playerTransform;
    [Header("RandomRoaming")]
    private float distanceToNextTarget;
    private Vector3 nextRandomTargetPos;

    [Header("Debug")]
    [SerializeField] private bool _showGizmos;
    [SerializeField] private Color _gizmoColor;

    public float Speed
    {
        get => agent.speed;
        set => agent.speed = value;
    }

    
    public void SetState(Transform playerTransform, NavigationMode navState)
    {
        Debug.Log($"EntityNavigation ({name}) SetState: {navState} (player: {playerTransform})", gameObject);
        if (playerTransform == null && navState == NavigationMode.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
        NavMode = navState;
        _playerTransform = playerTransform;

        agent.destination = NavMode switch
        {
            NavigationMode.MoveRandomly => GetNextTarget(),
            NavigationMode.MoveToPlayer => playerTransform.position,
            NavigationMode.RunFromPlayer => MapManager.Instance.GetRandomRunawayPlace(transform.position, playerTransform.position);
            _ => throw new ArgumentOutOfRangeException()
        };
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

    private void OnEnable()
    {
        agent.enabled = true;
    }

    private void OnDisable()
    {
        agent.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if(!_showGizmos || !Application.isPlaying) return;
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(agent.destination, 0.5f);
    }

    private void Update()
    {
        switch (NavMode)
        {
            case (NavigationMode.MoveRandomly):
                if (agent.remainingDistance <= Mathf.Max(1f, Speed))
                    MoveToNextRandomLocation();
                break;
            case (NavigationMode.MoveToPlayer):
                // Need something here?
                break;
        }
    }
    
    private void InitAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void MoveToNextRandomLocation()
    {
        nextRandomTargetPos = GetNextTarget();
        agent.SetDestination(new Vector3(nextRandomTargetPos.x, nextRandomTargetPos.y, transform.position.z));
    }
    
    private Vector3 GetNextTarget()
    {
        var trans =  MapManager.Instance.GetRandomPlaceTransform();
        return trans.position;
    }
}