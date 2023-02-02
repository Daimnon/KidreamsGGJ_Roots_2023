using UnityEngine;

public class EntityNavigation : MonoBehaviour
{
    public enum NavigationMode
    {
        Idle,
        MoveRandomly,
        MoveToPlayer,
    }

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
    }
}