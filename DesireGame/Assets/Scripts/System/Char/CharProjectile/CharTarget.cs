using System;
using System.Collections;
using UnityEngine;

namespace Client
{
    // 따로 포물선 궤도를 고집하지 않음.
    public class CharTarget : CharProjectal
    {
        [SerializeField]
        private CharBase Target;
        private Vector3 targetPos;
        private float dist;

        [SerializeField]
        private float chaseSpeed;
        
        private Coroutine moveCoroutine;

        protected override void CharInit()
        {
            base.CharInit();
            moveCoroutine = StartCoroutine(ChaseTarget(Origin, Target));
        }
        private void FixedUpdate()
        {
            targetPos = Target.gameObject.transform.position;
            dist = Vector3.Distance(targetPos, transform.position);
        }

        private IEnumerator ChaseTarget(Vector3 origin, CharBase target)
        {
            var fixedUpdate = new WaitForFixedUpdate();
            do
            {
                Vector3 direction = (targetPos - transform.position).normalized;
                transform.position += chaseSpeed * direction * Time.fixedDeltaTime;
                yield return fixedUpdate;
            } while (dist >= 0.5f);

            Debug.Log($"Arrived position : {transform.position}");
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Debug.Log("정상적으로 도달하여 투사체가 사라집니다. ExecutionBase 필요");
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
        }


    }
}