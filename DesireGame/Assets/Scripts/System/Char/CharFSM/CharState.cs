using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static Client.SystemEnum;

namespace Client
{

    public enum CharAction
    {
        Idle,
        Attack,
        Move,
        Execution,
        Hit,
        CC,
        Death,
    }

    /// <summary>
    /// FSM 전달용 파라미터
    /// </summary>
    public struct FSMParameter
    {
        //행동 Idle / Attack / Move / CC / Death 
        public CharAction charAction;
        public int priority;
        public bool isPlayAnim;
        public string AnimName;
        public Action<FSMParameter> actionFSMParameter;
        public Action action;

        public FSMParameter
        (
            int _priority = 0,
            bool _isPlayAnim = false,
            string _AnimName = "",
            Action<FSMParameter> _actionFSMParameter = null,
            Action _action = null,
            CharAction _charAction = CharAction.Idle
        )
        {
            charAction =  _charAction;
            priority = _priority;
            isPlayAnim = _isPlayAnim;
            AnimName = _AnimName;
            actionFSMParameter = _actionFSMParameter;
            action = _action;
        }

        public FSMParameter
        (
            Action<FSMParameter> _actionFSMParameter = null,
            Action _action = null,
            CharAction _charAction = CharAction.Idle
        )
        {
            charAction = _charAction;
            priority = 0;
            isPlayAnim = false;
            AnimName = null;
            actionFSMParameter = _actionFSMParameter;
            action = _action;
        }
    }
    
    public struct CreateFSMParameter
    {
        public PlayerState PlayerState;
        public CharBase CharBase;
        public CharFSMInfo CharFSMInfo;

    }

    public static class FSMFactory
    {
        public static CharState CreateFSM(CreateFSMParameter parameter)
        {
            switch (parameter.PlayerState)
            {
                case PlayerState.IDLE:    return new CharIDLE(parameter);
                case PlayerState.MOVE:    return new CharMOVE(parameter);
                case PlayerState.ATTACK:  return new CharATTACK(parameter);
                case PlayerState.DAMAGED: return new CharDAMAGED(parameter);
                case PlayerState.DEBUFF:  return new CharDEBUFF(parameter);
                case PlayerState.DEATH:   return new CharDEATH(parameter);
            }
            return new CharIDLE(parameter);
        }


    }

    public abstract class CharState
    {
        public CharBase charBase { get; set; } // 캐릭터 정보
        public CharFSMInfo charFSMInfo { get; set; } // 캐릭터 FSMInfo 
        public CharState NextCharFSM { get; set; } // 다음 State 예약
        public bool IsAction { get; set; } = false; // 현재 행동중
        public CharState(CreateFSMParameter parameter) 
        {
            charBase = parameter.CharBase;
            charFSMInfo = parameter.CharFSMInfo;
        }
        
        public abstract PlayerState NowPlayerState();

        public abstract CharState CharAction(FSMParameter parameter, out bool actionSuccess);

        public virtual void ActionInvoke(FSMParameter parameter)
        {
            parameter.action?.Invoke();
            parameter.actionFSMParameter?.Invoke(parameter);
        }
        public virtual void AnimPlay(bool play = true)
        {
            if (play)
            {
                charBase.PlayStateAnimation(NowPlayerState());
            }    
                
        }
    }
}