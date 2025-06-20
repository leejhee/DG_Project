using Client;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// ENUM 구현 : SELF
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
                CharManager.Instance.GetNearestEnemy(Caster, 0, inverse: true)
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
                CharManager.Instance.GetNearestEnemy(Caster, 1, inverse: true),
                CharManager.Instance.GetNearestEnemy(Caster, 0, inverse: true)
            };
        }
    }
    
    public class CurrentEnemyTargetStrategy : TargettingStrategyBase
    {
        public CurrentEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            if (!Caster.CharAI.isAIRun)
            {
                Debug.LogWarning("현재는 AI가 작동하지 않으므로 타겟을 반환하지 않습니다.");
                return new List<CharBase>();
            }

            var current = Caster.CharAI.FinalTarget;
            if (current == false)
                current = CharManager.Instance.GetNearestEnemy(Caster);
            return new List<CharBase>()
            {
                current
            };
        }
    }
    
    public class NearAlly2TargetStrategy : TargettingStrategyBase
    {
        public NearAlly2TargetStrategy(TargettingStrategyParameter param) : base(param)
        {}

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestAlly(Caster, 1), 
                CharManager.Instance.GetNearestAlly(Caster, 2)
            };
        }
    }
    
    public class NearAlly3TargetStrategy : TargettingStrategyBase
    {
        public NearAlly3TargetStrategy(TargettingStrategyParameter param) : base(param)
        {}

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestAlly(Caster, 1), 
                CharManager.Instance.GetNearestAlly(Caster, 2),
                CharManager.Instance.GetNearestAlly(Caster, 3)
            };
        }
    }
    
    
    
    public class EveryAllyTargetStrategy : TargettingStrategyBase
    {
        public EveryAllyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            return CharManager.Instance.GetAllySide(Caster.GetCharType());
        }
    }

    public class EveryEnemyTargetStrategy : TargettingStrategyBase
    {
        public EveryEnemyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            return CharManager.Instance.GetEnemySide(Caster.GetCharType());
        }
    }


}


