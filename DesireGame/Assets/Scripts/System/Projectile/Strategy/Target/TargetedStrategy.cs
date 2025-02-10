using Client;
using UnityEngine;

public class TargetedStrategy : ITargettingStrategy
{
    private CharBase _fixedTarget;

    public Vector3 AbstractTarget => _fixedTarget.transform.position;

    public TargetedStrategy(CharBase target) => _fixedTarget = target;

    public void CheckTargetPoint(Projectile projectile)
    {
        float displacement =( AbstractTarget - projectile.transform.position).sqrMagnitude;
        if (displacement <= 0.1f)
            projectile.ApplyEffect(_fixedTarget);
    }
}