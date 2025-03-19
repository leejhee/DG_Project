using System;
using System.Collections.Generic;
using UnityEngine;

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
            SynergyManager.Instance.RegisterCharSynergy(new CharLightWeightInfo()
            {
                index = _CastChar.Index,
                uid = _CastChar.GetID()
            }, mySynergy);
            RunFunction(true);
        }

        public override void RunFunction(bool StartExecution)
        {
            base.RunFunction(StartExecution);
            if (StartExecution)
            {
                SynergyManager.Instance.SubscribeToChanges(mySynergy, SubscribeDistribution);
            }
            else
            {
                SynergyManager.Instance.UnsubscribeToChanges(mySynergy, SubscribeDistribution);
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

            FunctionData synergyBuffData = DataManager.Instance.GetData<FunctionData>(param.function);
            if (synergyBuffData is null)
            {
                Debug.Log("�ó��� �����Ͱ� null�̷���. ������ �����սô�.");
                return;
            }

            FunctionBase synergyBuff = FunctionFactory.FunctionGenerate(new BuffParameter()
            {
                eFunctionType = synergyBuffData.function,
                CastChar = _CastChar,
                TargetChar = _TargetChar, // Ÿ�� 
                FunctionIndex = param.function
            });

            _distributedCache.Add(synergyBuff);
            synergyBuff.RunFunction(true);

        }

        // �� �� ������ ������ �Ǿ�� �ϴµ�
        // ���������� ���׳��� �ھƼ�
        // �Ŵ������� �׾�� ����. �ϸ� �ڱ��ڽ� killqueue�� ������ 
    }

    public class SynergyParameter
    {
        public SystemEnum.eSynergy triggingSynergy = SystemEnum.eSynergy.None;
        public long function;
    }
}