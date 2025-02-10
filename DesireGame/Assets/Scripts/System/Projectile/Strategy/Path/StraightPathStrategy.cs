using UnityEngine;

namespace Client
{
    public class StraightPathStrategy : IPathStrategy
    {
        public float Speed {  get; set; }

        public StraightPathStrategy(float speed) => Speed = speed;

        // 왜 Projectile의 Target으로 접근 안함?
        // 논타겟일 경우, 생성 당시 타겟의 '위치'가 목표 지점이 된다.
        // 즉 CharBase를 끝까지 추적하지 않는다.
        // 따라서 targetStrategy에 저장된 Transform을 참조해야 한다.
        public void UpdatePosition(Projectile projectileBase)
        {
            Vector3 direction = 
                (projectileBase.TargetGuide.AbstractTarget - projectileBase.transform.position).normalized;
            projectileBase.transform.position += Speed * Time.deltaTime * direction;

        }
    }
}