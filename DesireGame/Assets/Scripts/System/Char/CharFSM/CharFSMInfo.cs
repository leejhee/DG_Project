using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// ĳ���ͺ� ���� ���� ��
    /// </summary>
    public class CharFSMInfo
    {
        private Dictionary<PlayerState, CharState> _fsmDictionary = new Dictionary<PlayerState, CharState>();
        private CharBase _charBase;
        private CharState _charNowState;
        private string _charNowAnim = "";

        public CharState CharNowState => _charNowState;
        public PlayerState PlayerState => _charNowState.NowPlayerState();
        public Dictionary<PlayerState, CharState> FSMDictionary => _fsmDictionary;

        public CharFSMInfo(CharBase charBase)
        {
            _charBase = charBase;

            for (PlayerState i = 0; i < PlayerState.OTHER; i++)
            {
                CreateFSMParameter parameter = new CreateFSMParameter();
                parameter.PlayerState = i;
                parameter.CharBase = charBase;
                parameter.CharFSMInfo = this;
                _fsmDictionary[i] = FSMFactory.CreateFSM(parameter);
            }
            _charNowState = _fsmDictionary[PlayerState.IDLE];
        }

        public void CharAction(FSMParameter parameter)
        {
            if (_charNowState == null)
                return;

            PlayerState playerState = _charNowState.NowPlayerState();
            string charNowAnim = _charNowAnim;
            bool isActionSuccess = false;
            _charNowState = _charNowState.CharAction(parameter, out isActionSuccess);

            // �ൿ ����
            if (isActionSuccess == false)
                return;

            // �̹� ���� �ִϸ��̼� �÷������̸� ����
            //_charNowState.AnimPlay(playerState != _charNowState.NowPlayerState());
            if (parameter.isPlayAnim)
            {
                if (_charBase.CharAnim == null)
                    return;
                Animator animator = _charBase.CharAnim.Animator;
                if (animator == null)
                    return;

                animator.Play(parameter.AnimName);
                _charNowState.AnimPlay(parameter.AnimName != charNowAnim);
                _charNowAnim = parameter.AnimName;
            }

        }
    }
}