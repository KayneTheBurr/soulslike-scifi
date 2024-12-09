using UnityEngine;

public class PermanentDamageCollider : DamageCollider
{
    public CharacterGroup characterGroup;

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (!WorldUtilityManager.instance.CanIDamageThisTarget(characterGroup, damageTarget.characterGroup)) return;

        base.DamageTarget(damageTarget);
    }
}
