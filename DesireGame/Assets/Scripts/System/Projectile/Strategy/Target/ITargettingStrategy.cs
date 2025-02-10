using UnityEngine;

namespace Client
{
    public interface ITargettingStrategy
    {
        public Vector3 AbstractTarget { get; }

        public void CheckTargetPoint(Projectile projectile);

    }
}
