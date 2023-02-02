using UnityEngine;

[RequireComponent(typeof(EntityNavigation)), RequireComponent(typeof(EntityData))]
public class Entity : MonoBehaviour
{
    private EntityNavigation.NavigationMode _startNavMode;
    private EntityNavigation _navigation;
    private EntityData _data;

    private void Awake()
    {
        _data = GetComponent<EntityDataHolder>().Data;
        _navigation = GetComponent<EntityNavigation>();
        _navigation.NavMode = _startNavMode;
    }
    
    
}