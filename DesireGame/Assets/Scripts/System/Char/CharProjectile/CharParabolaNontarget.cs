using System.Collections;
using UnityEngine;

namespace Client
{
    public class CharParabolaNontarget : CharProjectile
    {
        [SerializeField]
        private Vector3 Target;         // 목표 지점

        [SerializeField]
        private float heightGap;        // 최고점의 상대적 높이

        private float FixedMaxHeight;   // 최고점의 절대적 높이       
        private float elaspedTime = 0f; // 경과 시간

        private float InitialSpeed;     // 초기 속력
        private Vector3 directionXZ;    // 수평 성분
        private float LaunchAngle;      // 발사각
        private float cosAngle;
        private float sinAngle;

        private Coroutine moveCoroutine;

        protected override void CharInit()
        {
            base.CharInit();
            
            GetSpeedAndAngle();
            moveCoroutine = StartCoroutine(UpdatePosition(Origin, Target));
        }

        /// <summary> 입력값(투사체 최대 높이, 중력, 타겟 위치)을 통한 궤도 산출 </summary>
        private void GetSpeedAndAngle()
        {
            // 투사체당 최고점의 높이 조절
            FixedMaxHeight = Origin.y + heightGap;

            // xz평면 요소와 y축 요소 구함
            Vector3 horizontalVec = new Vector3(Target.x - Origin.x, 0, Target.z - Origin.z);
            float horizontalDistance = Vector3.Magnitude(horizontalVec);
            directionXZ = horizontalVec.normalized;

            Vector3 verticalVec = (Target.y -  Origin.y) * Vector3.up;
            float verticalDistance = Vector3.Magnitude(verticalVec);

            // 포물선의 식을 통해 각도 유도
            float tangent = 
                2f * (FixedMaxHeight + Mathf.Sqrt(FixedMaxHeight *(FixedMaxHeight - verticalDistance))) / horizontalDistance;
            LaunchAngle = Mathf.Atan(tangent);

            // 각도의 cos, sin 성분 구하기
            cosAngle = Mathf.Cos(LaunchAngle);
            sinAngle = Mathf.Sin(LaunchAngle);

            // 발사 속도 구하기
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
            Debug.Log("정상적으로 도달하여 투사체가 사라집니다. ExecutionBase 필요");
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
        }
    }
}