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
    public class SkillBase : MonoBehaviour
    {
        private PlayableDirector _PlayableDirector;
        private CharBase _CharBase;

        public PlayableDirector PlayableDirector => _PlayableDirector;
        public CharBase CharPlayer => _CharBase;

        public void SetCharBase(CharBase charBase)
        {
            _CharBase = charBase;
        }

        private void Awake()
        {
            _PlayableDirector = GetComponent<PlayableDirector>();
            if (_PlayableDirector == null)
            {
                Debug.LogError($"{transform.name} PlayableDirector is Null");
            }           
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
                    _PlayableDirector.SetGenericBinding(animationTrack, _CharBase.CharAnim.Animator);
                    Debug.Log($"{track.name}' 트랙에 Animator 바인딩");
                }
            }
        }

        public void PlaySkill(InputParameter parameter)
        {
            if (_PlayableDirector == null)
                return;


            _PlayableDirector.Play();
        }
    }
}