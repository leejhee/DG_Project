using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    public class AttackColliderGeneratorPlayableBehavior : SkillTimeLinePlayableBehaviour
    {
        public GameObject AttackCollider { get; set; }
        public Vector3 OffSet { get; set; }
        public Vector3 Size { get; set; }

        private GameObject _collider = null;

        // Ŭ���� ���۵� �� ȣ��
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Vector3 vec = OffSet;
            if (AttackCollider == null)
                return;

            _collider = ObjectManager.Instance.Instantiate(AttackCollider, charBase.transform);
            if (_collider == null)
                return;

            _collider.transform.localPosition = vec;
            _collider.transform.localScale = Size;

            // GetComponent �̷��� �Ƿ���....
            //AttackCollider attackCollider = _collider.GetComponent<AttackCollider>();

            //if (attackCollider == null)
                //return;

            //if (!_collider.TryGetComponent(out AttackCollider attackCollider)) return;

            //attackCollider.SetData(charBase,skillBase);
        }

        // Ŭ���� ���� �� ȣ��
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_collider == null)
                return;
            GameObject.Destroy(_collider);
        }
    }
}