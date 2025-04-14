using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Client
{
    /// <summary>
    /// 스킬 객체
    /// </summary>
    [RequireComponent(typeof(SkillMarkerReceiver))]
    public class SkillBase : MonoBehaviour, IContextProvider
    {
        private PlayableDirector _PlayableDirector;
        private CharBase _caster;

        private SkillData _skillData;
        
        private int _nSkillRange;
        private SystemEnum.eSkillTargetType _targetType;

        public PlayableDirector PlayableDirector => _PlayableDirector;
        public CharBase CharPlayer => _caster;
        public SkillParameter InputParameter { get; private set; }
        
        public SkillAIInfo GetAIInfo() => new(){ TargetType = _targetType, Range = _nSkillRange };
        
        public void SetCharBase(CharBase caster)
        {
            _caster = caster;
        }

        // 추후 데이터 포함할 방법이 있다면 반드시 이 코드 교체할 것
        public void Init(SkillData data)
        {
            _skillData = data;
            _nSkillRange = data.skillRange;
            _targetType = data.skillTarget;
        }

        public void ResetSkill()
        {
            Init(_skillData);
        }
        
        public void SetRange(int value, bool delta=false)
        {
            if (_nSkillRange == 0) 
                return;

            if (delta)
                _nSkillRange += value;
            else
                _nSkillRange = value;
        }

        private void Awake()
        {
            _PlayableDirector = GetComponent<PlayableDirector>();
        }
    
        public void PlaySkill(SkillParameter parameter)
        {
            if (!_PlayableDirector)
                return;

            InputParameter = parameter;         
            _PlayableDirector.Play();
        }
    }
}