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
                Debug.Log($"{Caster.name}의 버프 삭제 : {BuffFunction.functionType}");
            }
            else
            {
                BuffFunction.RunFunction(false);
                Debug.Log($"시너지 버프 삭제 : {BuffFunction.functionType}");
            }
        }
    }
}