using static Client.SystemEnum;
using UnityEngine;
using System;

namespace Client
{
    public class SynergyFunction : FunctionBase
    {

        public SynergyFunction(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);

        }

        // Ÿ���� ���⼭ ���ؼ� �ѷ��� �Ѵ�. Ÿ���� List�� ���� �ʱ� ������ ���� Ÿ������ �����ȴ�.
        // TODO : Ÿ���ÿ��� 0���� �ε����ϴ°� ���� ������ ����. ���ϰ� ���� Ÿ�ٿ� ���̸� �ּ� ���� �ٲ� �� ������ ������ ��
        public void DelayedInit(eSkillTargetType targetType, Action killAction)
        {
            var intendedTargets = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
            {
                Caster = _CastChar,
                type = targetType
            }).GetTargets();

            if (intendedTargets.Count == 0)
                Debug.Log($"���� ���� Ÿ���� Ÿ���� Ÿ�� {targetType}�� ���� �������� �ʽ��ϴ�.");
            else
                _TargetChar = intendedTargets[0];


            killAction += KillSynergyFunction;
        }

        public void KillSynergyFunction() => _CastChar.FunctionInfo.KillFunction(this);
    }
}