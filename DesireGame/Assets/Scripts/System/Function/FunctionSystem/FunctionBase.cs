using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Client
{
    /// <summary>
    /// Execution ���̽� �ý���
    /// </summary>
    public abstract class FunctionBase
    {
        protected CharBase _TargetChar = null; // ��� Ÿ��
        protected CharBase _CastChar = null; // ��� ĳ���� ĳ����
        protected FunctionData _FunctionData = null; // ��� ������

        protected float _StartTime = 0; // ���� �ð�
        protected float _RunTime = 0; // ���� �ð�
        protected float _LifeTime = -1; // ������Ÿ�� -1 �� ���� ����

        public SystemEnum.eFunction functionType;

        #region Debug Property
        public string _TargetName => _TargetChar.name;
        public string _CasterName => _CastChar.name;
        public long DebugIndex => _FunctionData.Index;
        #endregion


        // ���� ������
        public FunctionBase(BuffParameter buffParam)
        {
            _TargetChar = buffParam.TargetChar;
            _CastChar = buffParam.CastChar;
            _FunctionData = DataManager.Instance.GetData<FunctionData>(buffParam.FunctionIndex);

            if (_FunctionData == null)
            {
                Debug.LogError($"Execution : {buffParam.FunctionIndex} ������ ȹ�� ����");
            }

            functionType = buffParam.eFunctionType;

            _LifeTime = _FunctionData.time > 0 ? 
                _FunctionData.time / SystemConst.PER_THOUSAND : _FunctionData.time;
        }

        public virtual void InitFunction() => _StartTime = Time.time;


        /// <summary>
        /// ���� ���۰� ����
        /// </summary>
        /// <param name="StartFunction"> true: �ൿ ���� false �ൿ ���� </param>
        public virtual void RunFunction(bool StartFunction = true)
        {
            if (StartFunction)
            {
                Debug.Log($"Function ���� : " +
                    $"�ε��� {_FunctionData.Index} " +
                    $"Ÿ�� {_FunctionData.function} " +
                    $"�ð� : {_FunctionData.time}");
            }
            else
            {
                Debug.Log($"Function ���� : " +
                    $"�ε��� {_FunctionData.Index} " +
                    $"Ÿ�� {_FunctionData.function} " +
                    $"�ð� : {_FunctionData.time}");

                if (_condition != null)
                    _TargetChar.FunctionInfo.KillCondition(_condition);
            }
        }

        public virtual void Update(float delta) { }

        /// <summary>
        /// ��� �ð� �Ϸ� üũ
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
            if (_FunctionData.ConditionCheck != default)
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
                //    Debug.Log("����� �������� ���Ͽ� �ߵ� ����");
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
                Debug.Log("����� �������� ���Ͽ� �ߵ� ����");
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

                Debug.Log($"{_CasterName}���� {_TargetName}������, {_FunctionData.Index}�� " +
                            $"child function {childData.Index}�� �߰�");
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

        /// <summary> Function ���� ų ����ġ </summary>
        /// <remarks> Caster �� Target�� ������ ��. </remarks>
        public void KillSelfFunction(bool killChildren = false, bool inCaster=false)
        {
            if(inCaster)            
                _CastChar.FunctionInfo.KillFunction(this);             
            else           
                _TargetChar.FunctionInfo.KillFunction(this);
           
            if (killChildren)
                KillChildFunctionToTarget(killChildren);
        }
    }
}