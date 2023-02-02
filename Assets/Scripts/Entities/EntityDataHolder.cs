using UnityEngine;

public class EntityDataHolder : MonoBehaviour
{
    [field: SerializeField] public EntityData Data { get; private set; }
}