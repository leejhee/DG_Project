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
    public class SkillBase : MonoBehaviour, IContextProvider
    {
        private PlayableDirector _PlayableDirector;
        private CharBase _caster;

        // 구조상 데이터를 포함해야 한다고 판단
        private SkillData _skillData;

        public PlayableDirector PlayableDirector => _PlayableDirector;
        public CharBase CharPlayer => _caster;

        // IContextProvider 구현. Execution의 적용 대상에 대한 파라미터 부여
        public InputParameter InputParameter { get; private set; }

        public void SetCharBase(CharBase caster)
        {
            _caster = caster;
        }

        // 추후 데이터 포함할 방법이 있다면 반드시 이 코드 교체할 것
        public void Init(SkillData data)
        {
            _skillData = data;
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
                Debug.LogError("TimelineAsset 연결안됐다.");
                return;
            }

            // 바인딩하려면 이렇게 순회를 해야한댄다... Find도 있을 텐데 어차피 다 순회해서 검색하는걸거다.
            foreach (TrackAsset track in timelineAsset.GetOutputTracks())
            {
                if (track is AnimationTrack animationTrack)
                {
                    _PlayableDirector.SetGenericBinding(animationTrack, _caster.CharAnim.Animator);
                    Debug.Log($"{track.name}' 트랙에 Animator 바인딩");
                }
            }
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