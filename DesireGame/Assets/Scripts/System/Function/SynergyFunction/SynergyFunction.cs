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

        // 타겟을 여기서 정해서 뿌려야 한다. 타겟을 List로 하지 않기 때문에 단일 타겟임이 강제된다.
        // TODO : 타게팅에서 0으로 인덱싱하는게 뭔가 느낌이 없다. 단일과 다중 타겟에 차이를 둬서 구조 바꿀 수 있을지 생각할 것
        public void DelayedInit(eSkillTargetType targetType, Action killAction)
        {
            var intendedTargets = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
            {
                Caster = _CastChar,
                type = targetType
            }).GetTargets();

            if (intendedTargets.Count == 0)
                Debug.Log($"따라서 현재 타겟이 타게팅 타입 {targetType}에 따라 정해지지 않습니다.");
            else
                _TargetChar = intendedTargets[0];


            killAction += KillSynergyFunction;
        }

        public void KillSynergyFunction() => _CastChar.FunctionInfo.KillFunction(this);
    }
}