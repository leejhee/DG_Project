using UnityEngine;

namespace Client
{
    public interface IPathStrategy
    {
        public Vector3 AbstractTarget { get; }
        public float Speed { get; }
        public void UpdatePosition(Projectile projectile);
    }
}