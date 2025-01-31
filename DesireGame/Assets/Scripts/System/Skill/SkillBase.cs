using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    /// <summary>
    /// ½ºÅ³ °´Ã¼
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

        public void PlaySkill(InputParameter parameter)
        {
            if (_PlayableDirector == null)
                return;

            _PlayableDirector.Play();
        }
    }
}