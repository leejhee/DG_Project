using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class CharAI
    {
        /* �ʿ��� ��
        1. ��ġ
        2. ����
        ------------
        1. �켱 ���� ��� + Ÿ�� ����
        2. ���� ���
        */

        CharBase charAgent;
        CharBase finalTarget; // �켱 ���� ����� ���� ���

        public CharAI(CharBase charAgent) 
        { 
            this.charAgent = charAgent;
        }

        /// <summary>
        /// ���� Ÿ�� ���ϱ�
        /// </summary>
        public void SetFinalTarget()
        {
            finalTarget = CharManager.Instance.GetNearestEnemy(charAgent);
        }

        /// <summary>
        /// ���� ��� �� �ൿ ����
        /// </summary>
        public void SetState()
        {
            // �̵� �ؾ��ϴ���, ���� �ؾ� �ϴ��� üũ

            // transform ��ġ ��� �Ÿ� ����
            float distanceSqr =  (charAgent.CharTransform.position - finalTarget.CharTransform.position).sqrMagnitude;

            // ��Ÿ����� �ָ� �ִ� ��� �̵�
                // ���� �̵�?

            // ��Ÿ� ���� �ִ� ��� ���� or ��ų üũ

        }

        public void SetAttackMode()
        {
            // ���� or ��ų üũ
            // ��ų ��� ���� : ���� ���� >= �ִ� ����

        }

    }
}