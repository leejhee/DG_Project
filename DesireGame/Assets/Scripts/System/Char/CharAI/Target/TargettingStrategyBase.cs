using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary> eSkillTargetType�� ���� ������ �߻�Ŭ���� </summary>
    public abstract class TargettingStrategyBase 
    {
        public CharBase Caster { get; protected set; }
        public TargettingStrategyBase(TargettingStrategyParameter param) => Caster = param.Caster;
        public abstract List<CharBase> GetTargets();
    }
}
