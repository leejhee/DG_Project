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
                Debug.LogError("������ functionIndex�� �ٽ� �ѹ� Ȯ���ϼ���.");
                return;
            }

            FunctionData data = DataManager.Instance.GetData<FunctionData>(functionIndex);
            if(data == null)
            {
                Debug.LogError("Data�� null�̴�. Index�� �����͸� �ٽ� �ѹ� Ȯ���ϼ���.");
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
                Debug.Log($"{skillParam.skillCaster.GetID()}�� ĳ���Ϳ��� {functionIndex}�� function ���� �Ϸ�");
            }

        }


    }
}