using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharPlayer : CharBase
    {
        [SerializeField] public Collider clickAbleCollider = null;

        private Vector3 offset;
        private Camera mainCamera;

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