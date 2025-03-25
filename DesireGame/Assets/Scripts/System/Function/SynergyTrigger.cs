using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{

    /// <summary>
    /// �ó��� Ʈ���� function. ����Ʈ ���� å�ӵ� ���
    /// </summary>
    public class SynergyTrigger : FunctionBase
    {
        private SystemEnum.eSynergy mySynergy; // ���� �ó����� ���� Ʈ����?       
        private CharLightWeightInfo myCharLightWeightInfo;

        public SynergyTrigger(BuffParameter buffParam) : base(buffParam)
        {
            mySynergy = (SystemEnum.eSynergy)_FunctionData.input1;
            myCharLightWeightInfo = new CharLightWeightInfo()
            {
                index = _CastChar.Index,
                uid = _CastChar.GetID()
            };
        }

        public override void RunFunction(bool StartExecution)
        {
            base.RunFunction(StartExecution);
            if (StartExecution)
            {
                SynergyManager.Instance.RegisterSynergy(myCharLightWeightInfo, mySynergy);
                
                //[TODO] : �� ���� �ó��� ���������� �����
                _CastChar.Dead += () =>
                {
                    _CastChar.FunctionInfo.KillFunction(this);
                };
            }
            else
            {
                SynergyManager.Instance.DeleteSynergy(myCharLightWeightInfo, mySynergy);
            }
            
        }
    }

    public class SynergyParameter
    {
        public SystemEnum.eSynergy triggingSynergy = SystemEnum.eSynergy.None;
        public long function;
    }
}