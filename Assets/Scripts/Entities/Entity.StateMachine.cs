using System;
using UnityEngine;

public partial class Entity
{
    private EntityState State
    {
        get => _state;
        set
        {
            if (value == _state) return;

            _updateAction = value switch
            {
                EntityState.Idle => UpdateIdleState,
                EntityState.ChasingPlayer => UpdateChasingState,
                EntityState.RunningFromPlayer => UpdateRunningState,
                EntityState.Attacking => UpdateAttackingState,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            Action<EntityState> transitionAction = value switch
            {
                EntityState.Idle => TransitionToIdle,
                EntityState.ChasingPlayer => TransitionToChasing,
                EntityState.RunningFromPlayer => TransitionToRunning,
                EntityState.Attacking => TransitionToAttacking,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            transitionAction(_state);
            _state = value;
        }
    }

    private void TransitionToIdle(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToIdle)));
        
        _navigation.enabled = true;
        _navigation.SetState(CachedPlayerTransform, EntityNavigation.NavigationMode.MoveRandomly);
        _anim.SetTrigger(AnimTrigger_Idle);
        moveStaggerAnim.enabled = true;
    }

    private void TransitionToChasing(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToChasing)));
        
        _navigation.enabled = true;
        _navigation.SetState(CachedPlayerTransform, EntityNavigation.NavigationMode.MoveToPlayer);
        
        _anim.SetTrigger(AnimTrigger_ChasingPlayer);
        moveStaggerAnim.enabled = true;
    }

    private void TransitionToRunning(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToRunning)));
        
        _navigation.enabled = true;
        
        moveStaggerAnim.enabled = true;
        _anim.SetTrigger(AnimTrigger_RunningFromPlayer);
    }

    private void TransitionToAttacking(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToAttacking)));
        
        _navigation.enabled = false;
        
        moveStaggerAnim.enabled = false;
        _anim.SetTrigger(AnimTrigger_Attack);
    }

    private void Stub_AttackPlayer(int damage)
    {
        Debug.Log($"Stub-- damaging player ({damage}) points!");
    }

    private void UpdateIdleState()
    {
        if (_playerInSight)
        {
            State = PlayerSeenState;
        }
    }
    
    private void UpdateChasingState()
    {
        if (!_playerInSight)
        {
            State = EntityState.Idle;
            return;
        }
        var isInAttackRange = IsPlayerInAttackRange();
        if (isInAttackRange) State = EntityState.Attacking;
    }

    private void UpdateRunningState()
    {
        if (!_playerInSight)
        {
            State = EntityState.Idle;
            return;
        }
    }

    private void UpdateAttackingState()
    {
        if (!_playerInSight)
        {
            State = EntityState.Idle;
            return;
        }

        if (!IsPlayerInAttackRange())
        {
            State = PlayerSeenState;
        }
    }
}