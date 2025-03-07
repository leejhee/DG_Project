using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class PingpongPathStrategy : IPathStrategy
    {
        public Vector3 AbstractTarget { get; private set; }
        public float InitialSpeed { get; private set; }

        public PingpongPathStrategy(PathStrategyParameter param)
        {
            //�ش� �ڸ����� ��ǥ�� �ϰ� ���ϴ�. ���� Ÿ���� ���� ���� Ȯ�� ����
            AbstractTarget = param.target.CharTransform.position;
            InitialSpeed = param.Speed;
        }

        public void UpdatePosition(Projectile projectile)
        {

        }

        // �ӵ� �����Լ��� ����...?
    }
}