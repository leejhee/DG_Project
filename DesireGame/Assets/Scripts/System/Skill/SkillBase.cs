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
    [RequireComponent(typeof(SkillMarkerReceiver))]
    public class SkillBase : MonoBehaviour, IContextProvider
    {
        private PlayableDirector _PlayableDirector;
        private CharBase _caster;

        private SkillData _skillData;


        public PlayableDirector PlayableDirector => _PlayableDirector;
        public CharBase CharPlayer => _caster;
        public SkillParameter InputParameter { get; private set; }
        public int NSkillRange { get; private set; }
        public SystemEnum.eSkillTargetType TargetType {  get; private set; }

        public void SetCharBase(CharBase caster)
        {
            _caster = caster;
        }

        // ���� ������ ������ ����� �ִٸ� �ݵ�� �� �ڵ� ��ü�� ��
        public void Init(SkillData data)
        {
            _skillData = data;
            NSkillRange = data.skillRange;
            TargetType = data.skillTarget;
        }

        public void ResetSkill()
        {
            Init(_skillData);
        }
        
        // ������ �̰Ÿ��� ��� �ִ��� �𸣰���
        public void SetRange(int value, bool delta=false)
        {
            if (NSkillRange == 0) 
                return;

            if (delta)
                NSkillRange += value;
            else
                NSkillRange = value;
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