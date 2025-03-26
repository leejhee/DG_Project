using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public abstract class ConditionBase
    {
        protected ConditionData _conditionData;
        
        public ConditionBase(ConditionData data)
        {
            _conditionData = data;
        }

        public abstract bool CheckCondition();

    }

    public class LaplacianUnitOnly : ConditionBase
    {
        private readonly long checkTargetIndex;

        public LaplacianUnitOnly(ConditionData data) : base(data)
        {
            checkTargetIndex = _conditionData.value1;
        }

        public override bool CheckCondition()
        {
            var members = SynergyManager.Instance.GetInfo(SystemEnum.eSynergy.LAPLACIAN);
            if (members == null || members.Count == 0)
            {
                return false;  
            }

            var answerIndices = members.Select(member => member.index).Distinct().ToList();                      
            return (answerIndices.Count == 1) && (answerIndices[0] == checkTargetIndex);
        }
    }

    public class TrueCondition : ConditionBase
    {
        public TrueCondition(ConditionData data) : base(data)
        {
        }

        public override bool CheckCondition()
        {
            return true;
        }
    }

    public static class ConditionFactory
    {
        public static ConditionBase CreateCondition(ConditionData data)
        {
            switch (data.conditionType)
            {
                case SystemEnum.eCondition.LAPLACIAN_ONLY: return new LaplacianUnitOnly(data);
                case SystemEnum.eCondition.LAPLACIAN_ALL:
                case SystemEnum.eCondition.HP_UNDER_N:
                case SystemEnum.eCondition.TRUE: return new TrueCondition(data);
                default: return null;
            }
        }
    }

}