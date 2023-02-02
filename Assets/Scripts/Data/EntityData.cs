using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "ScriptableObject/Data/Entity Data", order = 20)]
public class EntityData : ScriptableObject
{
    private const int _speedModifier = 100;
    
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Hp { get; set; }
    [field: SerializeField] public int Speed { get; set; }
    [field: SerializeField] public int Vision { get; set; }
    
    public int CalculatedSpeed => Speed * _speedModifier;
}
