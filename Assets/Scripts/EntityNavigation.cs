using UnityEngine;

public class EntityNavigation : MonoBehaviour
{
    public enum NavigationState
    {
        Idle,
        MoveRandomly,
        MoveToPlayer,
    }
    private EntityData entity;
    public NavigationState NavState { get; set; }
    public void SetState(Transform playerPos , NavigationState navState)
    {
        if(playerPos == null && navState == NavigationState.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
        else
            Vector2.MoveTowards(playerPos , )
    }
}