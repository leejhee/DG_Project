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
            // 캐릭터 AI 플래이 중 조작 금지
            if (_charAI?.isAIRun ?? true)
                return;
            
            if (mainCamera == null) return;
            SetNavMeshAgent(false);
            //// 마우스 클릭 위치와 캐릭터 위치의 차이 계산
            //offset = transform.position - GetMouseWorldPosition();
        }
        private void OnMouseDrag()
        {
            // 캐릭터 AI 플래이 중 조작 금지
            if (_charAI?.isAIRun ?? true)
                return;
            if (mainCamera == null) return;

            // 마우스 위치에 오프셋을 더해서 캐릭터 이동
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
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z; // 현재 Z 값 유지
            return mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }
        
    }
}