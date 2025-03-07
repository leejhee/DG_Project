using UnityEngine;

namespace Client
{
    public class StraightPathStrategy : IPathStrategy
    {
        private readonly CharBase fixedTarget;

        public Vector3 AbstractTarget => fixedTarget.transform.position;

        public float InitialSpeed { get; private set; }
        
        public StraightPathStrategy(PathStrategyParameter param)
        {
            fixedTarget = param.target;
            InitialSpeed = param.Speed;
        }

        public void UpdatePosition(Projectile projectile)
        {
            if (fixedTarget == false) return;

            Vector3 displacement = AbstractTarget - projectile.transform.position;

            // Check Target Point
            float sqrDistance = displacement.sqrMagnitude;
            if (sqrDistance <= 0.1f)
            {
                projectile.ApplyEffect(fixedTarget);
                projectile.SetDestroyFlag(true);
            }
                
            // Update Path
            Vector3 direction = displacement.normalized;
            projectile.transform.position += InitialSpeed * Time.deltaTime * direction;
        }


    }
}