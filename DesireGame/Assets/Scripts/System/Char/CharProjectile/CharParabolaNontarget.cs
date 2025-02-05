using System.Collections;
using UnityEngine;

namespace Client
{
    public class CharParabolaNontarget : CharProjectile
    {
        [SerializeField]
        private Vector3 Target;         // ��ǥ ����

        [SerializeField]
        private float heightGap;        // �ְ����� ����� ����

        private float FixedMaxHeight;   // �ְ����� ������ ����       
        private float elaspedTime = 0f; // ��� �ð�

        private float InitialSpeed;     // �ʱ� �ӷ�
        private Vector3 directionXZ;    // ���� ����
        private float LaunchAngle;      // �߻簢
        private float cosAngle;
        private float sinAngle;

        private Coroutine moveCoroutine;

        protected override void CharInit()
        {
            base.CharInit();
            
            GetSpeedAndAngle();
            moveCoroutine = StartCoroutine(UpdatePosition(Origin, Target));
        }

        /// <summary> �Է°�(����ü �ִ� ����, �߷�, Ÿ�� ��ġ)�� ���� �˵� ���� </summary>
        private void GetSpeedAndAngle()
        {
            // ����ü�� �ְ����� ���� ����
            FixedMaxHeight = Origin.y + heightGap;

            // xz��� ��ҿ� y�� ��� ����
            Vector3 horizontalVec = new Vector3(Target.x - Origin.x, 0, Target.z - Origin.z);
            float horizontalDistance = Vector3.Magnitude(horizontalVec);
            directionXZ = horizontalVec.normalized;

            Vector3 verticalVec = (Target.y -  Origin.y) * Vector3.up;
            float verticalDistance = Vector3.Magnitude(verticalVec);

            // �������� ���� ���� ���� ����
            float tangent = 
                2f * (FixedMaxHeight + Mathf.Sqrt(FixedMaxHeight *(FixedMaxHeight - verticalDistance))) / horizontalDistance;
            LaunchAngle = Mathf.Atan(tangent);

            // ������ cos, sin ���� ���ϱ�
            cosAngle = Mathf.Cos(LaunchAngle);
            sinAngle = Mathf.Sin(LaunchAngle);

            // �߻� �ӵ� ���ϱ�
            InitialSpeed = Mathf.Sqrt(2f * gravity * FixedMaxHeight) / sinAngle;
        }

        private IEnumerator UpdatePosition(Vector3 Origin, Vector3 Target)
        {
            float dist = Vector3.Distance(transform.position, Target);          
            while (dist >= 0.5f)
            {
                Debug.Log(dist);
                float nowXZ = InitialSpeed * cosAngle * elaspedTime;
                float nowY = InitialSpeed * sinAngle * elaspedTime - 0.5f * gravity * elaspedTime * elaspedTime;
                Vector3 updatedPosition = nowXZ * directionXZ + nowY * Vector3.up;
                transform.position = updatedPosition;
                dist = Vector3.Distance(transform.position, Target);
                elaspedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            Debug.Log($"Arrived position : {transform.position}");
            Destroy( gameObject );
        }

        private void OnDestroy()
        {
            Debug.Log("���������� �����Ͽ� ����ü�� ������ϴ�. ExecutionBase �ʿ�");
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
        }
    }
}