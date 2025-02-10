using UnityEngine;

namespace Client
{
    public class StraightPathStrategy : IPathStrategy
    {
        public float Speed {  get; set; }

        public StraightPathStrategy(float speed) => Speed = speed;

        // �� Projectile�� Target���� ���� ����?
        // ��Ÿ���� ���, ���� ��� Ÿ���� '��ġ'�� ��ǥ ������ �ȴ�.
        // �� CharBase�� ������ �������� �ʴ´�.
        // ���� targetStrategy�� ����� Transform�� �����ؾ� �Ѵ�.
        public void UpdatePosition(Projectile projectileBase)
        {
            Vector3 direction = 
                (projectileBase.TargetGuide.AbstractTarget - projectileBase.transform.position).normalized;
            projectileBase.transform.position += Speed * Time.deltaTime * direction;

        }
    }
}