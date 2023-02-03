using System;
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

    [SerializeField] private int _maxHp;
    [SerializeField] private int _speed;
    [SerializeField] private int _vision;
    [SerializeField] private float _attackRange;
    
    [field: SerializeField] public string Name { get; private set; }
    public int Hp => _maxHp;
    public int Speed => _speed;
    public int Vision => _vision;
    public float AttackRange => _attackRange;

    public int CalculatedSpeed => Speed * commonData.SpeedModifier;
    public event Action OnValidated;

    private void OnValidate()
    {
        OnValidated?.Invoke();
    }

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
