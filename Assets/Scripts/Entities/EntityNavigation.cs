using System;
using UnityEngine;
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

    public NavigationState NavMode { get; set; }
    private Vector2 XrandomOffset;
    private Vector2 YrandomOffset;
    public void SetState(Transform playerTransform, NavigationState navState)
    {
        if (playerTransform == null && navState == NavigationState.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
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
                MoveRandom();
                break;
        }
    }

    private void MoveRandom()
    {
    }

    private Vector2 GetRandomPositions()
    {
        float newX = transform.position.x + XrandomOffset.x;
        float newY = transform.position.y + YrandomOffset.y;

        return new Vector2(newX, newY); 
    }
}