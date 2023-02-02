using System;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(EntityDataHolder))]
public class EntityNavigation : MonoBehaviour
{
    public enum NavigationState
    {
        Idle,
        MoveRandomly,
        MoveToPlayer,
    }
    private EntityData _data;

    public NavigationState NavMode { get; private set; }
    private Vector2 XrandomOffset;
    private Vector2 YrandomOffset;
    private NavMeshAgent agent;
    private Transform _playerTransform;
    [SerializeField]
    private float distanceToNextTarget;
    public void SetState(Transform playerTransform, NavigationState navState)
    {
        if (playerTransform == null && navState == NavigationState.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
        NavMode = navState;
        _playerTransform = playerTransform;
    }

    private void Awake()
    {
        NavMode = NavigationState.Idle;
        var dataHolder = GetComponent<EntityDataHolder>();
        Debug.LogError($"No EntityData found on Entity {name}", gameObject);
        return;
    }
    private void Update()
    {
        switch(NavMode)
        {
            case (NavigationState.MoveRandomly):
                MoveRandom();
                break;
            case (NavigationState.MoveToPlayer):
                Vector2.MoveTowards(transform.position, _playerTransform.position, _data.Speed);
                break;
        }
    }

    private void MoveRandom()
    {
        if(agent.remainingDistance <= 2)
        {
            Vector2 newPos = GetNewRandomPositions();
            agent.SetDestination(newPos);
        }
    }

    private Vector2 GetNewRandomPositions()
    {
        float newX = transform.position.x + XrandomOffset.x;
        float newY = transform.position.y + YrandomOffset.y;

        return new Vector2(newX, newY); 
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, transform.up * distanceToNextTarget,Color.red);
    }
#endif
}