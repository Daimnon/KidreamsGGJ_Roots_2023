using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class SpriteDirection : MonoBehaviour
{
    public enum Mode
    {
        None,
        RigidBody,
        NavMesh,
    }

    [SerializeField] private Mode mode;

    [SerializeField, ShowIf(nameof(ModeIsRigidBody))] private Rigidbody2D rb;
    [SerializeField, ShowIf(nameof(ModeIsNavMesh))] private NavMeshAgent agent;
    
    [Header("Graphics")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private FaceDirection spriteDefaultDirection = FaceDirection.Right;
    
    public FaceDirection Direction { get; private set; } // In case we need it

    public Vector2 Vector => Direction == FaceDirection.Right ? Vector2.right : Vector2.left;

    // Naughty
    private bool ModeIsRigidBody() => mode == Mode.RigidBody;
    private bool ModeIsNavMesh() => mode == Mode.NavMesh;

    private void OnValidate()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
        if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        var oldDir = Direction;
        Direction = GetDirection(oldDir);
        if (Direction != oldDir)
        {
            var shouldFlipX = Direction != spriteDefaultDirection;
            spriteRenderer.flipX = shouldFlipX;
        }
    }

    private FaceDirection GetDirection(FaceDirection oldDirection)
    {
        var velocity = mode switch
        {
            Mode.NavMesh => (Vector2) agent.velocity,
            Mode.RigidBody => rb.velocity,
            Mode.None => Vector2.zero,
        };
        
        return velocity.x switch
        {
            >0 => FaceDirection.Right,
            <0 => FaceDirection.Left,
            _ => oldDirection
        };
    }
}