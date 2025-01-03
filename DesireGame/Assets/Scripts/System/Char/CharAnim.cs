using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.AnimationDefine;
using static Client.SystemEnum;
using static Client.InputManager;
using Client;
using UnityEngine.AI;

namespace Client
{
    public class CharAnim 
    {
        protected Animator  _Animator;       // �ִϸ�����

        public Animator Animator => _Animator;


        /// <summary>
        /// �ʱ�ȭ
        /// </summary>
        public void Initialized(Animator animator)
        {
            _Animator = animator;
        }
        public void PlayAnimation(PlayerState state)
        {
            _Animator.CrossFade($"{state}",1f,-1,0);
        }
    } 
}