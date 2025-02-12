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

        // ������ �����͸� �����ؾ� �Ѵٰ� �Ǵ�
        private SkillData _skillData;
        private int _nSkillRange; // ���� ��Ÿ�

        public PlayableDirector PlayableDirector => _PlayableDirector;
        public CharBase CharPlayer => _caster;
        public InputParameter InputParameter { get; private set; }
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

        private void Start()
        {
            var timelineAsset = _PlayableDirector.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                Debug.LogError("TimelineAsset ����ȵƴ�.");
                return;
            }

            // ���ε��Ϸ��� �̷��� ��ȸ�� �ؾ��Ѵ��... Find�� ���� �ٵ� ������ �� ��ȸ�ؼ� �˻��ϴ°ɰŴ�.
            //foreach (TrackAsset track in timelineAsset.GetOutputTracks())
            //{
            //    if (track is AnimationTrack animationTrack)
            //    {
            //        _PlayableDirector.SetGenericBinding(animationTrack, _caster.CharAnim.Animator);
            //        Debug.Log($"{track.name}' Ʈ���� Animator ���ε�");
            //    }
            //}
        }

        public void PlaySkill(InputParameter parameter)
        {
            if (_PlayableDirector == null)
                return;

            InputParameter = parameter;         
            _PlayableDirector.Play();
        }
    }
}