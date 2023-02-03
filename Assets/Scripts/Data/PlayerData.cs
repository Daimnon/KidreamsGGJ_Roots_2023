using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "ScriptableObject/Data/Player Data", order = 20)]
public class PlayerData : EntityData
{
    public Sprite _weakSprite, _strongSprite;
}
