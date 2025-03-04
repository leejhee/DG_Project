using Client;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// ENUM ±¸Çö : SELF
    /// </summary>
    public class SelfTargetStrategy : TargettingStrategyBase
    {
        public SelfTargetStrategy(TargettingStrategyParameter param) : base(param){ }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>() { Caster };
        }
    }

    public class NearEnemyTargetStrategy : TargettingStrategyBase
    {
        public NearEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>() { CharManager.Instance.GetNearestEnemy(Caster)};
        }
    }

    public class NearEnemy2TargetStrategy : TargettingStrategyBase
    {
        public NearEnemy2TargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster),
                CharManager.Instance.GetNearestEnemy(Caster, 1)
            };
        }
    }

    public class NearEnemy3TargetStrategy : TargettingStrategyBase
    {
        public NearEnemy3TargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster),
                CharManager.Instance.GetNearestEnemy(Caster, 1),
                CharManager.Instance.GetNearestEnemy(Caster, 2),
            };
        }
    }

    public class FarthestEnemyTargetStrategy : TargettingStrategyBase
    {
        public FarthestEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster, 0, true)
            };
        }
    }
    public class FarthestEnemy2TargetStrategy : TargettingStrategyBase
    {
        public FarthestEnemy2TargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster, 0, true),
                CharManager.Instance.GetNearestEnemy(Caster, 1, true)
            };
        }
    }

    public class CurrentEnemyTargetStrategy : TargettingStrategyBase
    {
        public CurrentEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            var current = Caster.CharAI.finalTarget;
            if (current == false)
                current = CharManager.Instance.GetNearestEnemy(Caster);
            return new List<CharBase>()
            {
                current
            };
        }
    }

}


