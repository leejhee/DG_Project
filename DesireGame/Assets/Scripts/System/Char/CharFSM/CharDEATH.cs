using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    public class CharDEATH: CharState
    {
        public CharDEATH(CreateFSMParameter parameter) : base(parameter)
        { }

        public override CharState CharAction(FSMParameter parameter, out bool actionSuccess)
        {

            actionSuccess = true;
            switch (parameter.charAction)
            {
                case Client.CharAction.Idle:
                case Client.CharAction.Attack:
                case Client.CharAction.Move:
                case Client.CharAction.Execution:
                case Client.CharAction.Hit:
                case Client.CharAction.CC:
                    {
                        actionSuccess = false;
                        return NextCharFSM;
                    }
                case Client.CharAction.Death:
                    {

                        actionSuccess = false;
                        return NextCharFSM;
                    }
            }

            actionSuccess = false;
            Debug.LogError($"CharFSM Error {NowPlayerState()} No FSM Action : {parameter.charAction}");
            return charFSMInfo.CharNowState;
        }

        public override PlayerState NowPlayerState() => PlayerState.DEATH;

    }
}