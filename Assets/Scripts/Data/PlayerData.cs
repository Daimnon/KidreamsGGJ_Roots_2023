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
    [SerializeField] private float _biteOffset = 1f, _moveToTargetDurationWhileStrong = 1.25f, _moveBackFromTargetDurationWhileStrong = 0.75f, _moveToTargetDurationWhileWeak = 1.5f, _moveBackFromTargetDurationWhileWeak = 1f, _biteSpeedWhileStrong = 1f, _biteSpeedWhileWeak = 0.75f, _biteTime = 0.75f;

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
    [SerializeField] private AnimationCurve _moveToTargetCurve;
    [SerializeField] private AnimationCurve _moveBackFromTargetCurve;

    public AnimationCurve MoveToTargetCurve => _moveToTargetCurve;
    public AnimationCurve MoveBackFromTargetCurve => _moveBackFromTargetCurve;
}
