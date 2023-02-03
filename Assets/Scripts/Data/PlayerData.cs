using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "ScriptableObject/Data/Player Data", order = 20)]
public class PlayerData : EntityData
{
    [Header("Player Sprites")]
    [SerializeField] private Sprite _weakSprite;
    [SerializeField] private Sprite _strongSprite;

    public Sprite WeakSprite => _weakSprite;
    public Sprite StrongSprite => _strongSprite;

    [Header("Player Values")]
    [SerializeField] private float _biteDistance = 3f;
    [SerializeField] private float _biteOffset = 1f, _moveToTargetDurationWhileStrong = 1f, _moveBackFromTargetDurationWhileStrong = 0.5f, _moveToTargetDurationWhileWeak = 1.25f, _moveBackFromTargetDurationWhileWeak = 0.75f, _biteSpeedWhileStrong = 1f, _biteSpeedWhileWeak = 0.75f, _biteTime = 0.5f;

    public float BiteDistance => _biteDistance;
    public float BiteOffset => _biteOffset;
    public float MoveToTargetDurationWhileStrong => _moveToTargetDurationWhileStrong;
    public float MoveBackFromTargetDurationWhileStrong => _moveBackFromTargetDurationWhileStrong;
    public float MoveToTargetDurationWhileWeak => _moveToTargetDurationWhileWeak;
    public float MoveBackFromTargetDurationWhileWeak => _moveBackFromTargetDurationWhileWeak;
    public float BiteSpeedWhileStrong => _biteSpeedWhileStrong;
    public float BiteSpeedWhileWeak => _biteSpeedWhileWeak;
    public float BiteTime => _biteTime;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve _moveToTargetCurveBiteSuccess;
    [SerializeField] private AnimationCurve _moveBackFromTargetCurveBiteSuccess;

    public AnimationCurve MoveToTargetCurveBiteSuccess => _moveToTargetCurveBiteSuccess;
    public AnimationCurve MoveBackFromTargetCurveBiteSuccess => _moveBackFromTargetCurveBiteSuccess;
    
    [SerializeField] private AnimationCurve _moveToTargetCurveFailedBite;
    [SerializeField] private AnimationCurve _moveBackFromTargetCurveFailedBite;

    public AnimationCurve MoveToTargetCurveFailedBite => _moveToTargetCurveFailedBite;
    public AnimationCurve MoveBackFromTargetCurveFailedBite => _moveBackFromTargetCurveFailedBite;

}
