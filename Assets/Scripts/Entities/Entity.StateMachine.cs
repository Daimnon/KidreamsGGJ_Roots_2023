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
        _anim.SetTrigger(AnimTrigger_Idle);
        moveStaggerAnim.enabled = true;
        _navigation.SetState(
            _cachedPlayer.transform, EntityNavigation.NavigationMode.MoveRandomly);
    }

    private void TransitionToChasing(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToChasing)));
        _anim.SetTrigger(AnimTrigger_ChasingPlayer);
        moveStaggerAnim.enabled = true;
        _navigation.SetState(_cachedPlayer.transform, EntityNavigation.NavigationMode.MoveToPlayer);
    }

    private void TransitionToRunning(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToRunning)));
        moveStaggerAnim.enabled = true;
        _anim.SetTrigger(AnimTrigger_RunningFromPlayer);
    }

    private void TransitionToAttacking(EntityState prevState)
    {
        Debug.Log(LogStr(nameof(TransitionToAttacking)));
        moveStaggerAnim.enabled = false;
        _anim.SetTrigger(AnimTrigger_Attack);
        
        Stub_AttackPlayer(_entityData.Damage);
    }

    private void Stub_AttackPlayer(int damage)
    {
        Debug.Log($"Stub-- damaging player ({damage}) points!");
    }

    private void UpdateIdleState()
    {
        if (_playerInSight) State = EntityState.ChasingPlayer;
    }

    private void UpdateChasingState()
    {
        if (!_playerInSight) State = EntityState.Idle;
        var distToPlayer = Vector2.Distance(_cachedPlayer.transform.position, transform.position);
        if (distToPlayer < _entityData.AttackRange) State = EntityState.Attacking;
    }

    private void UpdateRunningState()
    {
        if (!_playerInSight) State = EntityState.Idle;
    }

    private void UpdateAttackingState()
    {
        
    }
}