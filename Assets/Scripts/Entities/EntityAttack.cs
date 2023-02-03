using UnityEngine;

[RequireComponent(typeof(EntityDataHolder))]
public class EntityAttack : MonoBehaviour
{
    private EntityData _data;
    [SerializeField] private SpriteDirection spriteDir;

    private void Awake()
    {
        _data = GetComponent<EntityDataHolder>().Data;
    }

    private void Update()
    {
        
    }
}