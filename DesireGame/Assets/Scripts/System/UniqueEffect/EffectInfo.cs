using System.Collections.Generic;

namespace Client
{
    //하나로 통일해서 관리.
    public class EffectInfo
    {
        private Dictionary<SystemEnum.eCCType, EffectBase> _ccDictionary = new();
        private Queue<EffectBase> _ccReadyQueue = new();
        private Queue<EffectBase> _ccKillQueue = new();
        
        public void UpdateEffect()
        {
            while (_ccReadyQueue.Count > 0)
            {
                EffectBase cc = _ccReadyQueue.Dequeue();
                if (cc == null) continue;
                
                if (!_ccDictionary.TryGetValue(cc.EffectType, out EffectBase replaced))
                    _ccDictionary.Add(cc.EffectType, cc);
                else
                {
                    replaced.ReplaceCrowdControl(cc);
                    replaced.RunEffect();
                }
            }
            
            foreach (var cc in _ccDictionary.Values)
            {
                cc.CheckTimeOver();
                cc.Update();
            }
            
            while (_ccKillQueue.Count > 0)
            {
                EffectBase target = _ccKillQueue.Dequeue();
                target.EndEffect();
                _ccDictionary.Remove(target.EffectType);
            }
        }
        
        private void AddCrowdControl(EffectBase crowdControl)
        {
            _ccReadyQueue.Enqueue(crowdControl);
        }

        private void KillCrowdControl(EffectBase crowdControl)
        {
            _ccKillQueue.Enqueue(crowdControl);
        }
        
        //type column이 하나밖에 없어서, 여기서 분기시킴. 
        public void AddEffect(EffectParameter param)
        {
            EffectBase effect = EffectFactory.EffectGenerate(param);
            if(effect != null)
                AddCrowdControl(effect);
        }

        public void KillEffect(EffectBase effect)
        {
            if(effect != null)
                KillCrowdControl(effect);
        }
    }
}