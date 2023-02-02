using UnityEngine;

public class EntityNavigation : MonoBehaviour
{
    public enum NavigationState
    {
        Idle,
        MoveRandomly,
        MoveToPlayer,
    }
<<<<<<< HEAD:Assets/Scripts/EntityNavigation.cs
    private EntityData entity;
    public NavigationState NavState { get; set; }
    public void SetState(Transform playerTransform , NavigationState navState)
    {
        if (playerTransform == null && navState == NavigationState.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
        else
            Vector2.MoveTowards(transform.position, playerTransform.position, 1);
=======

    [SerializeField] private NavigationMode startMode;
    private EntityData _data;
    public NavigationMode NavMode { get; set; }

    private void Awake()
    {
        NavMode = startMode;
        var dataHolder = GetComponent<EntityDataHolder>();
        if (dataHolder == null)
        {
            Debug.LogError($"No EntityData found on Entity {name}", gameObject);
            return;
        }
        
        _data = dataHolder.Data;
>>>>>>> main:Assets/Scripts/Entities/EntityNavigation.cs
    }
}