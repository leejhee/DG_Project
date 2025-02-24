using System;
using System.Collections.Generic;
using System.Threading;

namespace Client
{

    /// <summary>
    /// 시너지 매니저와 CharBase와의 강한 결합을 풀기 위한 클래스
    /// </summary>
    public class SynergyTrigger : FunctionBase
    {
        private SystemEnum.eSynergy mySynergy; // 무슨 시너지에 대한 트리거?
        private List<FunctionBase> _distributedCache;
        

        public SynergyTrigger(BuffParameter buffParam) : base(buffParam)
        {

        }

        // 일단은 그냥 이렇게 함. 상수로 trigger 만들도록 할게요
        public void InitTrigger(SystemEnum.eSynergy synergy)
        {
            mySynergy = synergy;
            // 이거 Index로 해서 distinct 할 수 있으면 하자. 굳이 charbase 넣을지 말지는 질문할 것.
            SynergyManager.Instance.RegisterCharSynergy(_CastChar, mySynergy);
            RunFunction(true);
        }

        public override void RunFunction(bool StartExecution)
        {
            base.RunFunction(StartExecution);
            if (StartExecution)
            {
                //이중 등록? 이대로 괜찮은가?
                //저 매개변수들 경량 구조로 바꿔도 된다면 교체할 것.
                SynergyManager.Instance.SubscribeToChanges(SubscribeDistribution);
                SynergyManager.Instance.RegisterCharSynergy(_TargetChar, mySynergy);
            }
            else
            {
                SynergyManager.Instance.UnsubscribeToChanges(SubscribeDistribution);
                //더 할거 있나요
            }
        }

        public void SubscribeDistribution(SynergyParameter param)
        {
            if (mySynergy != param.triggingSynergy) return;
            if (_distributedCache == null) return;
            foreach (var cached in _distributedCache)
            {
                if (cached == null)
                    continue;
                RunFunction(false);
            }
                

            foreach(var funcIndex in param.functions)
            {
                FunctionData synergyBuffData = DataManager.Instance.GetData<FunctionData>(funcIndex);
                if (synergyBuffData is null) continue;
                FunctionBase synergyBuff = FunctionFactory.FunctionGenerate(new BuffParameter()
                {
                    eFunctionType = synergyBuffData.function,
                    CastChar = _CastChar,
                    TargetChar = _TargetChar, // 타겟 
                    FunctionIndex = funcIndex
                });

                _distributedCache.Add(synergyBuff);
                synergyBuff.RunFunction(true);
            }

        }

        // 팔 때 버프가 해제가 되어야 하는데
        // 버프에서도 안테나를 박아서
        // 매니저에서 죽어라 버프. 하면 자기자신 killqueue로 보내면 
    }

    public class SynergyParameter
    {
        public SystemEnum.eSynergy triggingSynergy = SystemEnum.eSynergy.None;
        public List<long> functions = new();
    }
}