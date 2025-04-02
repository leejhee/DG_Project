using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Client
{
    /// <summary>
    /// Execution 베이스 시스템
    /// </summary>
    public abstract class FunctionBase
    {
        protected CharBase _TargetChar = null; // 기능 타겟
        protected CharBase _CastChar = null; // 기능 캐스팅 캐릭터
        protected FunctionData _FunctionData = null; // 기능 데이터

        protected float _StartTime = 0; // 시작 시간
        protected float _RunTime = 0; // 현재 시간
        protected float _LifeTime = -1; // 라이프타임 -1 은 무한 지속

        public SystemEnum.eFunction functionType;

        #region Debug Property
        public string _TargetName => _TargetChar.name;
        public string _CasterName => _CastChar.name;
        public long DebugIndex => _FunctionData.Index;
        #endregion


        // 버프 생성자
        public FunctionBase(BuffParameter buffParam)
        {
            _TargetChar = buffParam.TargetChar;
            _CastChar = buffParam.CastChar;
            _FunctionData = DataManager.Instance.GetData<FunctionData>(buffParam.FunctionIndex);

            if (_FunctionData == null)
            {
                Debug.LogError($"Execution : {buffParam.FunctionIndex} 데이터 획득 실패");
            }

            functionType = buffParam.eFunctionType;

            _LifeTime = _FunctionData.time > 0 ? 
                _FunctionData.time / SystemConst.PER_THOUSAND : _FunctionData.time;
        }

        public virtual void InitFunction() => _StartTime = Time.time;


        /// <summary>
        /// 버프 시작과 종료
        /// </summary>
        /// <param name="StartFunction"> true: 행동 시작 false 행동 종료 </param>
        public virtual void RunFunction(bool StartFunction = true)
        {
            if (StartFunction)
            {
                Debug.Log($"Function 시작 : " +
                    $"인덱스 {_FunctionData.Index} " +
                    $"타입 {_FunctionData.function} " +
                    $"시간 : {_FunctionData.time}");
            }
            else
            {
                Debug.Log($"Function 종료 : " +
                    $"인덱스 {_FunctionData.Index} " +
                    $"타입 {_FunctionData.function} " +
                    $"시간 : {_FunctionData.time}");

                if (_condition != null)
                    _TargetChar.FunctionInfo.KillCondition(_condition);
            }
        }

        public virtual void Update(float delta) { }

        /// <summary>
        /// 기능 시간 완료 체크
        /// </summary>
        public void CheckTimeOver()
        {
            if (_LifeTime == -1f) return;

            float runTime = Time.time - _StartTime;
            if (runTime > _LifeTime || _LifeTime == 0f)
            {
                _TargetChar.FunctionInfo.KillFunction(this);
            }

        }

        private ConditionBase _condition = null;

        public void CheckFollowingCondition()
        {
            if (_FunctionData.ConditionCheck != 0)
            {
                var data = DataManager.Instance.GetData<ConditionData>(_FunctionData.ConditionCheck);
                if (data == null) return;
                _condition = ConditionFactory.CreateCondition(new ConditionParameter()
                {
                    conditionData = data,
                    conditionCallback = ConditionCheckCallback
                });
                _TargetChar.FunctionInfo.AddCondition(_condition);
                {
                //if (condition.CheckCondition())
                //{
                //    foreach(var followingfunction in _FunctionData.ConditionFuncList)
                //    {
                //        var func = DataManager.Instance.GetData<FunctionData>(followingfunction);
                //        AddChildFunctionToTarget(func);
                //    }
                //}
                //else
                //{
                //    Debug.Log("컨디션 만족하지 못하여 발동 안함");
                //}
                }
            }
        }

        private void ConditionCheckCallback(bool result)
        {
            if(result)
            {
                foreach (var followingfunction in _FunctionData.ConditionFuncList)
                {
                    var func = DataManager.Instance.GetData<FunctionData>(followingfunction);
                    AddChildFunctionToTarget(func);
                }
            }
            else
            {
                Debug.Log("컨디션 만족하지 못하여 발동 안함");
            }
        }


        protected List<FunctionBase> _children = new();
        public void AddChildFunctionToTarget(FunctionData childData)
        {
            if (childData == null || childData.function == default)
                return;
            else
            {
                var child = FunctionFactory.FunctionGenerate(new BuffParameter()
                {
                    CastChar = _CastChar,
                    TargetChar = _TargetChar,
                    eFunctionType = childData.function,
                    FunctionIndex = childData.Index
                });
                _children.Add(child);
                _TargetChar.FunctionInfo.AddFunction(child);

                Debug.Log($"{_CasterName}에서 {_TargetName}으로의, {_FunctionData.Index}의 " +
                            $"child function {childData.Index}번 추가");
            }               
        }

        public void KillChildFunctionToTarget(bool killAfterChildren=false)
        {
            foreach(var child in _children)
            {
                if(child == null) continue;
                child.KillSelfFunction(killAfterChildren, inCaster: false);
            }                              
        }

        /// <summary> Function 본인 킬 스위치 </summary>
        /// <remarks> Caster 및 Target에 주의할 것. </remarks>
        public void KillSelfFunction(bool killChildren = false, bool inCaster=false)
        {
            if(inCaster)            
                _CastChar.FunctionInfo.KillFunction(this);             
            else           
                _TargetChar.FunctionInfo.KillFunction(this);
           
            if (killChildren)
                KillChildFunctionToTarget(true);
        }
    }
}