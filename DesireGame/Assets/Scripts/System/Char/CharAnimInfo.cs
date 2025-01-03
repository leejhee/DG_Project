using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// ĳ���� ���� �ִϸ��̼�
    /// </summary>
    public class CharAnimInfo
    {
        private CharBase _charBase; // �ִϸ��̼� �÷��� ĳ����

        public Animator Animator { get; set; } = null; // �ִϸ��̼� 
        
        public CharAnimInfo(Animator animator)
        {
            Animator = animator;
            if (Animator == null)
            {
                Debug.LogError("CharAnimInfo �ִϸ��̼� ã�� ����");
            }
        }

        public void PlayAnimation(string playAnim)
        {
            if (Animator == null)
            {
                Debug.LogError("CharAnimInfo �ִϸ��̼� ã�� ����");
                return;
            }

            Animator.Play(playAnim, 0);

        }
    }
}