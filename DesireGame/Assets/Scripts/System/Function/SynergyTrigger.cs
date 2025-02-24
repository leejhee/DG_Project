using System;
using System.Collections.Generic;
using System.Threading;

namespace Client
{

    /// <summary>
    /// �ó��� �Ŵ����� CharBase���� ���� ������ Ǯ�� ���� Ŭ����
    /// </summary>
    public class SynergyTrigger : FunctionBase
    {
        private SystemEnum.eSynergy mySynergy; // ���� �ó����� ���� Ʈ����?
        private List<FunctionBase> _distributedCache;
        

        public SynergyTrigger(BuffParameter buffParam) : base(buffParam)
        {

        }

        // �ϴ��� �׳� �̷��� ��. ����� trigger ���鵵�� �ҰԿ�
        public void InitTrigger(SystemEnum.eSynergy synergy)
        {
            mySynergy = synergy;
            // �̰� Index�� �ؼ� distinct �� �� ������ ����. ���� charbase ������ ������ ������ ��.
            SynergyManager.Instance.RegisterCharSynergy(_CastChar, mySynergy);
            RunFunction(true);
        }

        public override void RunFunction(bool StartExecution)
        {
            base.RunFunction(StartExecution);
            if (StartExecution)
            {
                //���� ���? �̴�� ��������?
                //�� �Ű������� �淮 ������ �ٲ㵵 �ȴٸ� ��ü�� ��.
                SynergyManager.Instance.SubscribeToChanges(SubscribeDistribution);
                SynergyManager.Instance.RegisterCharSynergy(_TargetChar, mySynergy);
            }
            else
            {
                SynergyManager.Instance.UnsubscribeToChanges(SubscribeDistribution);
                //�� �Ұ� �ֳ���
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
                    TargetChar = _TargetChar, // Ÿ�� 
                    FunctionIndex = funcIndex
                });

                _distributedCache.Add(synergyBuff);
                synergyBuff.RunFunction(true);
            }

        }

        // �� �� ������ ������ �Ǿ�� �ϴµ�
        // ���������� ���׳��� �ھƼ�
        // �Ŵ������� �׾�� ����. �ϸ� �ڱ��ڽ� killqueue�� ������ 
    }

    public class SynergyParameter
    {
        public SystemEnum.eSynergy triggingSynergy = SystemEnum.eSynergy.None;
        public List<long> functions = new();
    }
}