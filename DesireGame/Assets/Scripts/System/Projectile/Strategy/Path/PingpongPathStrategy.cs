using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class PingpongPathStrategy : IPathStrategy
    {
        public Vector3 AbstractTarget { get; private set; }
        public float InitialSpeed { get; private set; }

        //private float currentSpeed;

        private bool returning =false;

        public PingpongPathStrategy(PathStrategyParameter param)
        {
            //�ش� �ڸ����� ��ǥ�� �ϰ� ���ϴ�. ���� Ÿ���� ���� ���� Ȯ�� ����
            AbstractTarget = param.target.CharTransform.position;
            InitialSpeed = param.Speed;
            //currentSpeed = param.Speed;
        }

        public void UpdatePosition(Projectile projectile)
        {
            if (projectile.Caster == false)
            {
                projectile.SetDestroyFlag(true);
                return;
            }

            if (returning)
            {
                AbstractTarget = projectile.Caster.CharTransform.position;
            }
            Vector3 displacement = AbstractTarget - projectile.ProjectileTransform.position;
            float sqrDistance = displacement.sqrMagnitude;
            if(sqrDistance <= 0.1f)
            {
                if (returning)
                    projectile.SetDestroyFlag(true);
                else
                    returning = true;
            }

            Vector3 direction = displacement.normalized;
            projectile.ProjectileTransform.position += InitialSpeed * Time.deltaTime * direction;
            // ���� �� ������ ���ϰ� �ϴ� �׳� ������θ� ��.
        }

        public bool ManageCollision(Collider other, SystemEnum.eCharType enemyType)
        {
            if(other.TryGetComponent(out CharBase collidedChar))
            {
                if (collidedChar.GetCharType() == enemyType)
                    return true;
            }

            return false; 
        }
    }
}