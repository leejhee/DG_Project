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

        private Queue<FunctionBase> _functionOnBattleStartQueue = new();

        public void Init()
        {
            for (eFunction i = 0; i < eFunction.eMax; i++)
            {
                _functionBaseDic[i] = new List<FunctionBase>();
            }

            StageManager.Instance.OnStartCombat += AddBattleStartQueueFunction;
        }

        private void AddBattleStartQueueFunction()
        {
            while(_functionOnBattleStartQueue.Count > 0)
            {
                _functionReadyQueue.Enqueue(_functionOnBattleStartQueue.Dequeue());
            }
        }


        public void UpdateFunctionDic()
        {          
            // 준비 큐에서 딕셔너리로 추가
            while(_functionReadyQueue.Count != 0)
            {
                FunctionBase target = _functionReadyQueue.Dequeue();
                if (target == null) continue;

                if(!_functionBaseDic.ContainsKey(target.functionType))
                    _functionBaseDic.Add(target.functionType, new List<FunctionBase>());

                if (!_functionBaseDic[target.functionType].Contains(target)) //버프 중첩 불가? (Equals override 안해서 영향 없이 중첩되는 상황)
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
            EnqueueImmediateFunction(func);
        }
               
        public void AddFunction(BuffParameter target, eBuffTriggerTime triggerTime)
        {
            FunctionBase func = FunctionFactory.FunctionGenerate(target);
            if(triggerTime == eBuffTriggerTime.BORN)
            {
                EnqueueImmediateFunction(func);
            }
            else if(triggerTime == eBuffTriggerTime.COMBAT)
            {
                EnqueueInitialFunction(func);
            }
        }

        public void AddFunction(FunctionBase target, eBuffTriggerTime triggerTime=eBuffTriggerTime.BORN)
        {
            if (target == null) return;
            if (triggerTime == eBuffTriggerTime.BORN)
            {
                EnqueueImmediateFunction(target);
            }
            else if (triggerTime == eBuffTriggerTime.COMBAT)
            {
                EnqueueInitialFunction(target);
            }
        }

        // 이거 써야 할지 다시 한번 생각해보자.
        // 시너지 전용 enqueue. 제네릭으로 다른 건 안되게 막음. 어쩔수 없다...
        public void AddFunction<T>(T synergyFunction) where T : SynergyFunction
        {
            EnqueueImmediateFunction(synergyFunction);
        }


        public void KillFunction(FunctionBase target)
        {
            EnqueueKill(target);
        }

        // Function Dictionary로의 접근 통제.
        private void EnqueueImmediateFunction(FunctionBase target)
        {
            _functionReadyQueue.Enqueue(target);
        }

        private void EnqueueKill(FunctionBase target)
        {
            _functionKillQueue.Enqueue(target);
        }

        private void EnqueueInitialFunction(FunctionBase target)
        {
            _functionOnBattleStartQueue.Enqueue(target);
        }

        #region CONDITION PART
        private List<ConditionBase> _conditions = new();

        public void AddCondition(ConditionParameter param)
        {
            var condition = ConditionFactory.CreateCondition(param);
            if(condition != null)
                _conditions.Add(condition);
        }

        public void AddCondition(ConditionBase condition)
        {
            _conditions.Add(condition);
        }

        public void KillCondition(ConditionBase condition)
        {
            _conditions.Remove(condition);
        }

        public void EvaluateCondition(ConditionCheckInput param)
        {
            foreach(var condition in _conditions)
                condition.CheckInput(param);
        }
        #endregion
    }
}