using UnityEngine;

[CreateAssetMenu(fileName = "New CommonEntityData", menuName = "ScriptableObject/Data/Common Entity Data", order = 21)]
public class CommonEntityData : ScriptableObject
{
    [Tooltip("Multiplier to set Rigidbody velocity")]
    [field: SerializeField] public int SpeedModifier { get; private set; } = 100;
    
    [field: Header("Entity view raycasting")]
    [field: SerializeField] public float BaseFOVAngle { get; private set; }
    [field: SerializeField] public float BaseViewDistance { get; private set; }
    [field: SerializeField] public float MaxDistanceBetweenRays { get; private set; } = 0.25f;

    [field: Header("Entity view \"lvlups\"")]
    [field: SerializeField] public float FOVAnglePerVisionPoint { get; private set; }
    [field: SerializeField] public float ViewDistancePerVisionPoint { get; private set; }
}