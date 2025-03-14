using static Client.SystemEnum;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class FunctionInfo
    {
        private Dictionary<eFunction, List<FunctionBase>> _functionBaseDic = new(); // 기능 

        private Queue<FunctionBase> _functionReadyQueue = new();

        private Queue<FunctionBase> _functionKillQueue = new();

        public void Init()
        {
            for (eFunction i = 0; i < eFunction.eMax; i++)
            {
                _functionBaseDic[i] = new List<FunctionBase>();
            }
        }

        public void UpdateFunctionDic()
        {          
            // 준비 큐에서 딕셔너리로 추가
            while(_functionReadyQueue.Count != 0)
            {
                FunctionBase target = _functionReadyQueue.Dequeue();
                if (!_functionBaseDic[target.functionType].Contains(target)) //버프 중첩 불가 
                {
                    target.InitFunction();
                    target.RunFunction(true);
                    _functionBaseDic[target.functionType].Add(target);
                }                    
            }

            foreach (var functionBaseList in _functionBaseDic)
            {
                foreach (var function in functionBaseList.Value)
                {
                    function.CheckTimeOver();
                    function.Update(Time.deltaTime);
                }
            }

            while (_functionKillQueue.Count != 0)
            {
                FunctionBase target = _functionKillQueue.Dequeue();
                target.RunFunction(false);
                if (_functionBaseDic[target.functionType].Contains(target))
                    _functionBaseDic[target.functionType].Remove(target);
            }
        }

        public void AddFunction(BuffParameter target)
        {
            FunctionBase func = FunctionFactory.FunctionGenerate(target);
            EnqueueFunction(func);
        }
        
        public void KillFunction(FunctionBase target)
        {
            EnqueueKill(target);
        }

        // Function Dictionary로의 접근 통제.
        private void EnqueueFunction(FunctionBase target)
        {
            _functionReadyQueue.Enqueue(target);
        }

        private void EnqueueKill(FunctionBase target)
        {
            _functionKillQueue.Enqueue(target);
        }


    }
}