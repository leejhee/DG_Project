using UnityEngine;

namespace Client
{
    public class SynergyBuffRecord
    {
        public CharBase Caster { get; private set; }
        public FunctionBase BuffFunction { get; private set; }

        public SynergyBuffRecord(CharBase caster, FunctionBase buffFunction)
        {
            Caster = caster;
            BuffFunction = buffFunction;
        }

        public void KillSynergyBuff()
        {
            if (Caster)
            {
                BuffFunction.KillSelfFunction(true, true);
                Debug.Log($"{Caster.GetID()}번 캐릭 {Caster.name}의 버프 삭제 : {BuffFunction.functionType} | {BuffFunction.DebugIndex}");
            }
            else
            {
                BuffFunction.RunFunction(false);
                Debug.Log($"{Caster.GetID()}번 캐릭 {Caster.name}의 시너지 버프 삭제 : {BuffFunction.functionType} | {BuffFunction.DebugIndex}");
            }
        }
    }
}