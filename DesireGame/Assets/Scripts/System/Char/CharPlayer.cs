using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class CharPlayer : CharBase
    {
        [SerializeField] public Collider clickAbleCollider = null;

        private Vector3 offset;
        private Camera mainCamera;

        private List<eSynergy> _charSynergies = null;

        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.ALLY;
        
        protected override void Start()
        {
            base.Start();
            mainCamera = Camera.main;
        }

        protected override void CharInit()
        {
            base.CharInit();
            CharManager.Instance.SetChar<CharPlayer>(this);

            #region �ó���
            _charSynergies = new List<eSynergy>()
            {
                _charData.synergy1,
                _charData.synergy2,
                _charData.synergy3
            };

            foreach (var synergy in _charSynergies)
            {
                if (synergy == eSynergy.None) continue;
                var synergyTrigger = DataManager.Instance.SynergyTriggerMap[synergy];
                _functionInfo.AddFunction(new BuffParameter()
                {
                    eFunctionType = eFunction.SYNERGY_TRIGGER,
                    CastChar = this,
                    TargetChar = this,
                    FunctionIndex = synergyTrigger.Index
                });
            }
            #endregion
        }

        void OnMouseDown()
        {
            // ĳ���� AI �÷��� �� ���� ����
            if (_charAI?.isAIRun ?? true)
                return;
            
            if (mainCamera == null) return;
            SetNavMeshAgent(false);
            //// ���콺 Ŭ�� ��ġ�� ĳ���� ��ġ�� ���� ���
            //offset = transform.position - GetMouseWorldPosition();
        }
        private void OnMouseDrag()
        {
            // ĳ���� AI �÷��� �� ���� ����
            if (_charAI?.isAIRun ?? true)
                return;
            if (mainCamera == null) return;

            // ���콺 ��ġ�� �������� ���ؼ� ĳ���� �̵�
            transform.position = GetMouseWorldPosition(); //+ offset;

        }
        private void OnMouseUp()
        {
            PlayerMove msg = new();
            msg.beforeTileIndex = TileIndex;
            msg.moveChar = this;
            MessageManager.SendMessage<PlayerMove>(msg);
            SetNavMeshAgent(true);
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z; // ���� Z �� ����
            return mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }
        
    }
}