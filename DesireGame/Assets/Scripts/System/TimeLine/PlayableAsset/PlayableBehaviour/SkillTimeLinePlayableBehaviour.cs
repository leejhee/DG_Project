using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    /// <summary>
    /// ��ų�� �÷��̾�� �ൿ
    /// </summary>
    public abstract class SkillTimeLinePlayableBehaviour : PlayableBehaviour
    {
        public CharBase charBase;
        public SkillBase skillBase;

        // �����ڸ� ������ ���� �ʱ�ȭ�� �ϴ°� ���ڰ�...�; �ʱ�ȭ�� �ϴ� virtual�� ������...
        // ���� ����� �� �� ��Ҹ� �ƴ� ���� ū �̵浵 �ȴ�.
        public virtual void InitBehaviour(CharBase charBase, SkillBase skillBase)
        {
            this.charBase = charBase;
            this.skillBase = skillBase;
        }
    }
}