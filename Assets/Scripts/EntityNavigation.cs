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
    public NavigationMode NavMode { get; set; }

    private void Awake()
    {
        NavMode = startMode;
    }
}