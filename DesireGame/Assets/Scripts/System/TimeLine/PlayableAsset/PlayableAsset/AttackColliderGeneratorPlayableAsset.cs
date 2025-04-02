using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using static Client.SystemEnum;

namespace Client
{
    public class AttackColliderGeneratorPlayableAsset : SkillTimeLinePlayableAsset
    {
        [SerializeField]
        private Vector3 offSet;
        
        [SerializeField]
        private Vector3 size;

        [SerializeField]
        [Header("공격 타입(피격자)")]
        private eIsAttack IsAttack;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            base.CreatePlayable(graph, owner);

            AttackColliderGeneratorPlayableBehavior playableBehaviour = new()
            {
                charBase = charBase,
                skillBase = skillBase,
                OffSet = offSet,
                Size = size
            };

            switch (IsAttack)
            {
                case eIsAttack.Player:
                    playableBehaviour.AttackCollider = ObjectManager.Instance.Load<GameObject>(SystemString.PlayerHitCollider);
                    break;
                case eIsAttack.Monster:
                    playableBehaviour.AttackCollider = ObjectManager.Instance.Load<GameObject>(SystemString.MonsterHitCollider);
                    break;
                default:
                    break;
            }
            var scriptPlayable = ScriptPlayable<AttackColliderGeneratorPlayableBehavior>.Create(graph, playableBehaviour);

            return scriptPlayable;
        }
    }
}