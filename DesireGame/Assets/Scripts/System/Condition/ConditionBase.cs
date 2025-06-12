using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Client.SystemEnum;
using System;

// TODO : 캐스팅으로 인한 성능 리스크 괜찮을지.
namespace Client
{
    public struct ConditionParameter
    {
        public Action<bool> conditionCallback;
        public ConditionData conditionData;
    }
 
    /// /////////////////////////////////////////////////////////////////////
    
    // 파생 클래스에서는 각 생성자 부분에 구독하는 부분을 추가할 것
    public abstract class ConditionBase
    {
        protected ConditionData _conditionData;
        protected Action<bool> _conditionCallback;

        protected ConditionBase(ConditionParameter param)
        {
            _conditionData = param.conditionData;
            _conditionCallback = param.conditionCallback;
        }

        //public abstract bool CheckCondition(ConditionCheckParameter param);

        public virtual void CheckInput(ConditionCheckInput param) { }
    }

    public abstract class StatCondition : ConditionBase
    {
        protected StatCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not StatConditionInput)
            {
                Debug.LogError($"형변환 실패 {typeof(StatConditionInput)}");
            }
        }

    }

    public abstract class CharPosCondition : ConditionBase
    {
        protected CharPosCondition(ConditionParameter param) : base(param)
        {
        }
    }

    public abstract class SynergyCondition : ConditionBase
    {
        protected SynergyCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not SynergyConditionInput) return;
        }
    }

    
    
    public class HPUnderNPercent : StatCondition
    {
        private bool _invokedOnce;
        
        public HPUnderNPercent(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not StatConditionInput statCondition)
            {
                Debug.LogError($"형변환 실패로 조건 체크 종료. {typeof(StatConditionInput)}");
                return;
            }

            if (_invokedOnce ||
                statCondition.Delta >= 0 ||
                statCondition.ChangedStat != eStats.NHP)
                return;

            _conditionCallback.Invoke(statCondition.Input < _conditionData.value1);
            _invokedOnce = true;
        }
    }


    public class LaplacianUnitOnly : SynergyCondition
    {
        private readonly long checkTargetIndex;

        public LaplacianUnitOnly(ConditionParameter param) : base(param)
        {
            checkTargetIndex = _conditionData.value1;
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            if (param is not SynergyConditionInput laplacian)
                return;
            
            var members = SynergyManager.Instance.GetInfo(laplacian.CharTypeContext, eSynergy.LAPLACIAN);
            if (members == null || members.Count == 0 || laplacian.ChangedSynergy != eSynergy.LAPLACIAN)
            {
                return;
            }

            var answerIndices = members.Select(member => member.Index).Distinct().ToList();
            _conditionCallback.Invoke(answerIndices.Count == 1 && answerIndices[0] == checkTargetIndex);
        }

    }

    // 있으면 안될 거 같음. 아무리 생각해도요.
    public class TrueCondition : ConditionBase
    {
        public TrueCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckInput param)
        {
            base.CheckInput(param);
            _conditionCallback.Invoke(true);
        }
    }

    public static class ConditionFactory
    {
        public static ConditionBase CreateCondition(ConditionParameter param)
        {
            switch (param.conditionData.conditionType)
            {
                case eCondition.LAPLACIAN_ONLY: return new LaplacianUnitOnly(param);
                case eCondition.HP_UNDER_N:     return new HPUnderNPercent(param);
                case eCondition.TRUE:           return new TrueCondition(param);
                default: return null;
            }
        }
    }

}