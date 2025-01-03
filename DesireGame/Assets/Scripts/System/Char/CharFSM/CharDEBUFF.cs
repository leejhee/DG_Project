using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// CC ����
    /// </summary>
    public class CharDEBUFF : CharState
    {
        public CharDEBUFF(CreateFSMParameter parameter) : base(parameter)
        {

        }

        public int NowPriority { get; set; }

        public override CharState CharAction(FSMParameter parameter, out bool actionSuccess)
        {

            actionSuccess = true;
            switch (parameter.charAction)
            {
                case Client.CharAction.Idle:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.IDLE];
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {
                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
                case Client.CharAction.Attack:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.ATTACK];
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {

                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
                case Client.CharAction.Move:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.MOVE];
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {

                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
                case Client.CharAction.Execution:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.CharNowState;
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {

                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
                case Client.CharAction.Hit:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.DAMAGED];
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {

                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
                case Client.CharAction.CC:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.DEBUFF];
                            // ���⼭ CC������ ���ðǵ�....
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {

                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
                case Client.CharAction.Death:
                    {
                        if (parameter.priority >= NowPriority)
                        {
                            NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.DEATH];
                            ActionInvoke(parameter);
                            return NextCharFSM;
                        }
                        else
                        {

                            actionSuccess = false;
                            return charFSMInfo.CharNowState;
                        }
                    }
            }

            actionSuccess = false;
            Debug.LogError($"CharFSM Error {NowPlayerState()} No FSM Action : {parameter.charAction}");
            return charFSMInfo.CharNowState;
        }

        public override PlayerState NowPlayerState() => PlayerState.DEBUFF;
        
    }
}