using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Client
{
    public class AnimationPlayableAsset: SkillTimeLinePlayableAsset
    {
        [SerializeField]
        private AnimationClip animationClip;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            base.CreatePlayable(graph, owner);

            var playableBehaviour = new AnimationPlayableBehaviour();

            playableBehaviour.animator = charBase.CharAnim.Animator;
            playableBehaviour.animationClip = animationClip;
            var scriptPlayable = ScriptPlayable<AnimationPlayableBehaviour>.Create(graph, playableBehaviour);

            // AnimationClipPlayable ����
            var animationPlayable = AnimationClipPlayable.Create(graph, animationClip);

            // �÷��̺��� ����
            scriptPlayable.AddInput(animationPlayable, 0, 1);

            return scriptPlayable;
        }
    }
}