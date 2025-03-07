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
            //해당 자리만을 목표로 하고 갑니다. 따라서 타겟의 실존 여부 확인 안함
            AbstractTarget = param.target.CharTransform.position;
            InitialSpeed = param.Speed;
        }

        public void UpdatePosition(Projectile projectile)
        {

        }

        // 속도 일차함수로 조절...?
    }
}