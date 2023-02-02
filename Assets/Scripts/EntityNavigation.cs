using UnityEngine;

public class EntityNavigation : MonoBehaviour
{
    public enum NavigationState
    {
        Idle,
        MoveRandomly,
        MoveToPlayer,
    }

    public NavigationState NavState { get; set; }

    private void MoveTowardPlayer()
    {

    }
}