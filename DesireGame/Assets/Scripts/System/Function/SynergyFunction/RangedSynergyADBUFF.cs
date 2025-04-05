namespace Client
{
    public class RangedSynergyADBUFF : FunctionBase
    {
        // TODO : 이 킬카운트는 저장되어야 한다.
        private static int _rangedEnemyKillCount;
        

        public RangedSynergyADBUFF(BuffParameter buffParam) : base(buffParam)
        {
        }


        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
                
            UpdateBuffByKillCount(StartFunction);
            
            if(StartFunction)
            {
                // 시너지로 연결된 유닛들 간의 이벤트 버스를 형성한다. 
                MessageManager.SubscribeMessage<OnRangedKill>(this, UpdateBuff);
                // 레인저 막타에 대한 구독을 적들한테 해 놓는다.
                foreach (var enemy in CharManager.Instance.GetOneSide(SystemEnum.eCharType.ENEMY))
                {
                    enemy.CharStat.신상공개 += KillBuffSubscribe;
                }

            }
            else
            {
                MessageManager.RemoveMessageAll(this);
                
                foreach (var enemy in CharManager.Instance.GetOneSide(SystemEnum.eCharType.ENEMY))
                {
                    enemy.CharStat.신상공개 -= KillBuffSubscribe;
                }
            }
        }

        private void UpdateBuffByKillCount(bool isRegister=true)
        {
            if (isRegister)
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.AD, 
                    _FunctionData.input1 + _rangedEnemyKillCount);
            }
            else
            {
                _TargetChar.CharStat.ChangeStateByBuff(SystemEnum.eStats.AD,
                    -(_FunctionData.input1 + _rangedEnemyKillCount));
            }
        }

        private void KillBuffSubscribe(CharBase killer)
        {
            if (killer == true && killer.GetID() == _TargetChar.GetID())
            {
                MessageManager.SendMessage(new OnRangedKill());
            }
        }

        public void UpdateBuff(OnRangedKill killParam)
        {
            UpdateBuffByKillCount(false);
            _rangedEnemyKillCount++;
            UpdateBuffByKillCount();  
        }
    }

    public class OnRangedKill : MessageSystemParam
    {
    }
}