using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class CharATTACK : CharState
    {
        public CharATTACK(CreateFSMParameter parameter) : base(parameter)
        {
        }

        public override PlayerState NowPlayerState() => PlayerState.ATTACK;

        public override CharState CharAction(FSMParameter parameter, out bool actionSuccess)
        {
            actionSuccess = true;
            switch (parameter.charAction)
            {
                case Client.CharAction.Idle:
                    {
                        if (IsAction)
                            return charFSMInfo.CharNowState;
                        
                        NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.IDLE];
                        ActionInvoke(parameter);
                        return NextCharFSM;
                        
                    }
                case Client.CharAction.Attack:
                    {
                        NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.ATTACK];
                        ActionInvoke(parameter);
                        return NextCharFSM;
                    }
                case Client.CharAction.Move:
                    {
                        NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.MOVE];
                        ActionInvoke(parameter);
                        return NextCharFSM;
                    }
                case Client.CharAction.Execution:
                    {
                        NextCharFSM = charFSMInfo.CharNowState;
                        ActionInvoke(parameter);
                        return NextCharFSM;
                    }
                case Client.CharAction.Hit:
                    {
                        NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.DAMAGED];
                        ActionInvoke(parameter);
                        return NextCharFSM;
                    }
                case Client.CharAction.CC:
                    {
                        NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.DEBUFF];
                        ActionInvoke(parameter);
                        return NextCharFSM;
                    }
                case Client.CharAction.Death:
                    {
                        NextCharFSM = charFSMInfo.FSMDictionary[PlayerState.DEATH];
                        ActionInvoke(parameter);
                        return NextCharFSM;
                    }
            }
            Debug.LogError($"CharFSM Error {NowPlayerState()} No FSM Action : {parameter.charAction}");
            return charFSMInfo.CharNowState;
        }
    }
}