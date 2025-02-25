using Client;
using UnityEngine;

public class TargetedStrategy : ITargettingStrategy
{
    private CharBase _fixedTarget;

    public Vector3 AbstractTarget { get
        {
            if(_fixedTarget == null)
            {
                return Vector3.zero;
            }
            else
            {
                return _fixedTarget.transform.position;
            }
        } }

    public TargetedStrategy(CharBase target) => _fixedTarget = target;

    public void CheckTargetPoint(Projectile projectile)
    {
        if (_fixedTarget == null) return;
        float displacement =( AbstractTarget - projectile.transform.position).sqrMagnitude;
        if (displacement <= 0.1f)
            projectile.ApplyEffect(_fixedTarget);
    }
}