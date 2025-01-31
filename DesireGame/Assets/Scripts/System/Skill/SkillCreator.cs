using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public static class SkillCreator
    {
        public static SkillBase CreateSkill(string skillName)
        {
            SkillBase skillBase = null;
            skillBase = ObjectManager.Instance.Instantiate<SkillBase>($"Skill/{skillName}");
            return skillBase;
        }
        public static SkillBase CreateSkill(long skillIndex)
        {
            SkillBase skillBase = null;
            var _skillData = DataManager.Instance.GetData<SkillData>(skillIndex);

            if (_skillData == null)
            {
                Debug.LogWarning($"CreateSkill : {skillIndex} ��ų ���� ����.");
                return null;
            }

            skillBase = ObjectManager.Instance.Instantiate<SkillBase>($"Skill/{_skillData.skillTimeLineName}");

            if (_skillData == null)
            {
                Debug.LogWarning($"CreateSkill : {skillIndex} ��ų ���� ����.");
            }
            return skillBase;
        }
    }
}