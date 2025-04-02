using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Client.SystemEnum;
using System;

// TODO : 자잘한 Condition 계산을 위한 기준을 부여할 Static helper 만들 것.
namespace Client
{
    public struct ConditionParameter
    {
        public Action<bool> conditionCallback;
        public ConditionData conditionData;
    }

    // 원형
    public class ConditionCheckParameter
    {
        public eCondition conditionType;
    }

    public class StatConditionParameter : ConditionCheckParameter
    {
        public CharStat stat;
        public eStats changedStat;

        public long input;
    }

    public class SynergyConditionParameter : ConditionCheckParameter
    {
        public eSynergy changedSynergy;
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////
    /// </summary>

    // 파생 클래스에서는 각 생성자 부분에 구독하는 부분을 추가할 것
    public abstract class ConditionBase
    {
        protected ConditionData _conditionData;
        protected Action<bool> _conditionCallback;      
        
        public ConditionBase(ConditionParameter param)
        {
            _conditionData = param.conditionData;
            _conditionCallback += param.conditionCallback;
        }

        //public abstract bool CheckCondition(ConditionCheckParameter param);

        public virtual void CheckInput(ConditionCheckParameter param) { }
    }

    public abstract class StatCondition : ConditionBase
    {
        protected StatCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckParameter param)
        {
            base.CheckInput(param);
            if (param is not StatConditionParameter) return;            
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

        public override void CheckInput(ConditionCheckParameter param)
        {
            base.CheckInput(param);
            if (param is not SynergyConditionParameter) return;
        }
    }


    public class HPUnderNPercent : StatCondition
    {
        public HPUnderNPercent(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckParameter param)
        {
            base.CheckInput(param);
            var StatCondition = param as StatConditionParameter; // 타입 검사는 부모에서 수행
            _conditionCallback.Invoke(StatCondition.input < _conditionData.value1);
        }
    }


    public class LaplacianUnitOnly : SynergyCondition
    {
        private readonly long checkTargetIndex;

        public LaplacianUnitOnly(ConditionParameter param) : base(param)
        {

        }

        //public override bool CheckCondition(ConditionCheckParameter param)
        //{
        //    var members = SynergyManager.Instance.GetInfo(SystemEnum.eSynergy.LAPLACIAN);
        //    if (members == null || members.Count == 0)
        //    {
        //        return false;  
        //    }
        //
        //    var answerIndices = members.Select(member => member.index).Distinct().ToList();                      
        //    return (answerIndices.Count == 1) && (answerIndices[0] == checkTargetIndex);
        //}

        public override void CheckInput(ConditionCheckParameter param)
        {
            base.CheckInput(param);
            var members = SynergyManager.Instance.GetInfo(eSynergy.LAPLACIAN);
            if (members == null || members.Count == 0)
            {
                return;
            }

            var answerIndices = members.Select(member => member.index).Distinct().ToList();
            _conditionCallback.Invoke((answerIndices.Count == 1) && (answerIndices[0] == checkTargetIndex));
        }

    }

    public class TrueCondition : ConditionBase
    {
        public TrueCondition(ConditionParameter param) : base(param)
        {
        }

        public override void CheckInput(ConditionCheckParameter param)
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