using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    public class ProjectileGeneratorPlayableAsset : SkillTimeLinePlayableAsset
    {
        // [TODO] : 마커로 옮겨서 발사만 하고 끝나는 거로 합시다. Projectile이 지속될 필요 없음
        [SerializeField] GameObject ProjectilePrefab;
        [SerializeField] float Range;
        [SerializeField] float Speed;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            base.CreatePlayable(graph, owner);
            ProjectileGeneratorPlayableBehaviour playableBehaviour = new()
            {
                charBase = charBase,
                skillBase = skillBase,
                ProjectilePrefab = ProjectilePrefab,
                Range = Range,
                Speed = Speed
            };
            var scriptPlayable = ScriptPlayable<ProjectileGeneratorPlayableBehaviour>.Create(graph, playableBehaviour);
            return scriptPlayable;
        }

    }
}