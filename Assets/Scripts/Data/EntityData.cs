using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "ScriptableObject/Data/Entity Data", order = 20)]
public class EntityData : ScriptableObject
{
    public string Name { get; set; }
    public int Hp { get; set; }
    public int Speed { get; set; }
    public int Vision { get; set; }
}
