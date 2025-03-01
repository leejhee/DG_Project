using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class SkillMarkerFunctionInjector : SkillTimeLineMarker
    {
        [SerializeField] 
        private long functionIndex;

        public override void MarkerAction()
        {
            if(functionIndex == default)
            {
                Debug.LogError("주입할 functionIndex를 다시 한번 확인하세요.");
                return;
            }

            FunctionData data = DataManager.Instance.GetData<FunctionData>(functionIndex);
            if(data == null)
            {
                Debug.LogError("Data가 null이다. Index와 데이터를 다시 한번 확인하세요.");
                return;
            }

            var targets = skillParam.skillTargets;
            for(int i = 0; i< targets.Count; i++)
            {
                FunctionBase function = FunctionFactory.FunctionGenerate(new BuffParameter()
                {
                    CastChar = skillParam.skillCaster,
                    TargetChar = targets[i],
                    eFunctionType = data.function,
                    FunctionIndex = functionIndex
                });
                function.RunFunction(true);
                Debug.Log($"{skillParam.skillCaster.GetID()}번 캐릭터에서 {functionIndex}번 function 주입 완료");
            }

        }


    }
}