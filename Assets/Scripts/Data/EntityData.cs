using NaughtyAttributes;
using UnityEngine;

public enum EntityType
{
    Animal,
    Human,
}
    
[CreateAssetMenu(fileName = "New Entity", menuName = "ScriptableObject/Data/Entity Data", order = 21)]
public class EntityData : ScriptableObject
{
    [ValidateInput(nameof(ValidateNotNull))]
    [SerializeField, Expandable] private CommonEntityData commonData;
    
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Hp { get; set; }
    [field: SerializeField] public int Speed { get; set; }
    [field: SerializeField] public int Vision { get; set; }

    public int CalculatedSpeed => Speed * commonData.SpeedModifier;
    
    // Entity View Raycasting - formulas
    [ShowNativeProperty] public float ViewDistance
    {
        get
        {
            
            if (Vision == 0) return 0;
            return commonData.BaseViewDistance + Vision * commonData.ViewDistancePerVisionPoint;
        }
    }

    [ShowNativeProperty] public float ViewFOVAngle
    {
        get
        {
            if (Vision == 0) return 0;
            return commonData.BaseFOVAngle + Vision * commonData.FOVAnglePerVisionPoint;
        }
    }

    // Entity View raycasting - dependent (calculated from FOV/Distance)
    [ShowNativeProperty] public int NumRays => Mathf.CeilToInt(ViewFOVAngle / DeltaAngleRays);
    public float DeltaAngleRays => Mathf.Atan2(commonData.MaxDistanceBetweenRays, ViewDistance) * Mathf.Rad2Deg;


    private bool ValidateNotNull(CommonEntityData x) => x != null;

}
