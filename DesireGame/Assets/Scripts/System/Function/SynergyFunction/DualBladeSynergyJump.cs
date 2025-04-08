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
            TileManager.Instance.OnTileSetCharacter += 정렬하고재계산할것;
            가입또는탈퇴(_TargetChar, isRegister: true);
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
                가입또는탈퇴(_TargetChar, isRegister: false);
            }
        }

        private static void 가입또는탈퇴(CharBase target, bool isRegister)
        {
            if (isRegister)
                _dualBladeList.Add(target);
            else
                _dualBladeList.Remove(target);
            정렬하고재계산할것();
        }

        private static void 정렬하고재계산할것()
        {
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