public class Villager : Entity
{
    // TODO: projectiles (handle attack range in base class first?)
    // TODO: Grave system (separate class, but will hold the Villagers
    protected override void TransitionToAttacking(EntityState prevState)
    {
        base.TransitionToAttacking(prevState);
    }

    protected override void UpdateAttackingState()
    {
        base.UpdateAttackingState();
    }
}