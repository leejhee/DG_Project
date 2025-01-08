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
        protected Animator  _Animator;       // 애니메이터

        public Animator Animator => _Animator;


        /// <summary>
        /// 초기화
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