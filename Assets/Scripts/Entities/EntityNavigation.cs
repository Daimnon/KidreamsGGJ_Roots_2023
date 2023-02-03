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
    private EntityData _data;
    [NaughtyAttributes.ShowNativeProperty]
    public NavigationMode NavMode { get; private set; }
    private Vector2 XrandomOffset;
    private Vector2 YrandomOffset;
    private NavMeshAgent agent;
    private Transform _playerTransform;
    [SerializeField]
    private float distanceToNextTarget;
    [SerializeField] private Vector3 curTarget;

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
        var dataHolder = GetComponent<EntityDataHolder>();
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

                break;
        }
    }
    private void InitAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        curTarget = GetNextTarget();
        agent.SetDestination(new Vector3(curTarget.x, curTarget.y, 0));
        RotateToNewRoamingPos();
    }
    private void MoveToNextRandomLocation()
    {
        curTarget = GetNextTarget();
        agent.SetDestination(new Vector3(curTarget.x, curTarget.y, 0));
        RotateToNewRoamingPos();
    }
    private void MoveToPlayer() => Vector2.MoveTowards(transform.position, _playerTransform.position, _data.Speed);

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

    private Quaternion RotateTowardsNewPos(Vector3 newPos)
    {
        var offset = 90f;
        Vector3 dir = newPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.Euler(0, 0, 1 * (angle - offset));
        return newRotation;
    }
    private void RotateToNewRoamingPos()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, RotateTowardsNewPos(curTarget), 0.2f);
    }
}

    #endregion}