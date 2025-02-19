using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Client
{
    /// <summary>
    /// ��ų ��ü
    /// </summary>
    public class SkillBase : MonoBehaviour, IContextProvider
    {
        private PlayableDirector _PlayableDirector;
        private CharBase _caster;

        private SkillData _skillData;
        private int _nSkillRange; // ���� ��Ÿ�

        public PlayableDirector PlayableDirector => _PlayableDirector;
        public CharBase CharPlayer => _caster;
        public SkillParameter InputParameter { get; private set; }
        public int NSkillRange => _nSkillRange;
        public SystemEnum.eSkillTargetType TargetType => _skillData.skillTarget;

        public void SetCharBase(CharBase caster)
        {
            _caster = caster;
        }

        // ���� ������ ������ ����� �ִٸ� �ݵ�� �� �ڵ� ��ü�� ��
        public void Init(SkillData data)
        {
            _skillData = data;
            _nSkillRange = data.skillRange;
        }

        private void Awake()
        {
            _PlayableDirector = GetComponent<PlayableDirector>();
        }
    
        public void PlaySkill(SkillParameter parameter)
        {
            if (_PlayableDirector == null)
                return;

            InputParameter = parameter;         
            _PlayableDirector.Play();
        }
    }
}