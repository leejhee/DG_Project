using System.Collections.Generic;

namespace Client
{
    //TODO : CC 관련 이펙트 및 UI 리소스 필요한지 알아볼것
    public class EffectInfo
    {
        private EffectBase _currentEffect;

        public void UpdateEffect()
        {
            _currentEffect?.CheckTimeOver();
        }
        
        public void AddEffect(EffectParameter newEffect)
        {
            EffectBase effect = EffectFactory.EffectGenerate(newEffect);
            _currentEffect?.EndEffect();
            _currentEffect = effect;
            _currentEffect?.RunEffect();
        }
        
        public void KillEffect(EffectBase effect)
        {
            if(effect == _currentEffect)
                _currentEffect?.EndEffect();
            _currentEffect = null;
        }
        
    }
}