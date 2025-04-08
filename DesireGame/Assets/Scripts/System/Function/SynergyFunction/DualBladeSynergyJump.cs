using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    public class DualBladeSynergyJump : FunctionBase
    {
        private class OnDualBladeReplaced : MessageSystemParam
        {
            public int ChangedTargetTile;
        }
        
        private static List<CharBase> _dualBladeList = new();
        private int _jumpTarget = -1;
        
        public DualBladeSynergyJump(BuffParameter buffParam) : base(buffParam)
        {
            MessageManager.SubscribeMessage<OnDualBladeReplaced>(this, SetJumpTarget);
            SortReplaceOrder(_TargetChar, isRegister: true);
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                TileManager.Instance.TeleportCharacter(_TargetChar, _jumpTarget);
            }
            else
            {
                // 지속시간 무조건 -1이어야 함
                // 따라서 이게 발동하는 순간은 시너지가 사라지는 순간 / 회원탈퇴하는 순간임
                SortReplaceOrder(_TargetChar, isRegister: false);
            }
        }

        private static void SortReplaceOrder(CharBase target, bool isRegister)
        {
            if(isRegister)
                _dualBladeList.Add(target);
            else
                _dualBladeList.Remove(target);
            _dualBladeList = _dualBladeList.OrderByDescending(c => c.TileIndex).ToList();
            foreach (var dualBlade in _dualBladeList)
            {
                MessageManager.SendMessage(new OnDualBladeReplaced()
                {
                    ChangedTargetTile = TileManager.Instance.ReserveTeleportEnemyRear(dualBlade)
                });
                Debug.Log("정렬 후 재할당 완료");
            }
        }

        private void SetJumpTarget(OnDualBladeReplaced target)
        {
            TileManager.Instance.ChangeReserved(_jumpTarget, target.ChangedTargetTile);
            _jumpTarget = target.ChangedTargetTile;
        }
        
        
    }
}