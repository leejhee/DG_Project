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
                Debug.LogError("TimelineAsset ����ȵƴ�.");
                return;
            }

            // ���ε��Ϸ��� �̷��� ��ȸ�� �ؾ��Ѵ��... Find�� ���� �ٵ� ������ �� ��ȸ�ؼ� �˻��ϴ°ɰŴ�.
            foreach (TrackAsset track in timelineAsset.GetOutputTracks())
            {
                if (track is AnimationTrack animationTrack)
                {
                    _PlayableDirector.SetGenericBinding(animationTrack, _CharBase.CharAnim.Animator);
                    Debug.Log($"{track.name}' Ʈ���� Animator ���ε�");
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