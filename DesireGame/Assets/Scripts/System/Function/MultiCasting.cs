using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class MultiCasting : FunctionBase
    {
        public MultiCasting(BuffParameter buffParam) : base(buffParam)
        {
        }


        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                var indices = new List<long>()
                {
                    _FunctionData.input1,
                    _FunctionData.input2,
                    _FunctionData.input3,
                    _FunctionData.input4,
                    _FunctionData.input5
                };
                foreach (var index in indices)
                {
                    if(index != SystemConst.NO_CONTENT)
                    {
                        var data = DataManager.Instance.GetData<FunctionData>(index);
                        if (data == null)
                        {
                            Debug.LogWarning($"{index} �����Ͱ� ������ Ȯ���غ�����. ���� �ε����� �Ѿ");
                            continue;
                        }
                        
                        AddChildFunctionToTarget(data);

                        //_TargetChar.FunctionInfo.AddFunction(new BuffParameter()
                        //{
                        //    CastChar = _CastChar,
                        //    TargetChar = _TargetChar,
                        //    eFunctionType = data.function,
                        //    FunctionIndex = index
                        //});
                        Debug.Log($"���������� ���� �Ϸ� _CastChar {_CastChar.GetID()} _TargetChar {_TargetChar.GetID()} index {index}");
                    }
                }
            }
            
        }
    }
}