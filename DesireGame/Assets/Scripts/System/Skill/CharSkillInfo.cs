using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// ĳ���� ��ų ���� (ĳ���� Start������ ����)
    /// </summary>
    public class CharSKillInfo
    {
        private Dictionary<long, SkillBase> _dicSkill = new Dictionary<long, SkillBase>(); // ��ų ����Ʈ
        private CharBase _charBase; // ��ų ������
        private Transform _SkillRoot; // ��ų ��Ʈ 

        public Dictionary<long, SkillBase> DicSkill => _dicSkill; // ��ų ����Ʈ

        public CharSKillInfo(CharBase charBase)
        {
            _charBase = charBase;
        }

        /// <summary>
        /// Char Start ����
        /// </summary>
        /// <param name="skillArray"></param>
        public void Init(List<long> skillArray)
        {
            // ��ų ��Ʈ ������Ʈ ����
            string SkillRoot = "SkillRoot";
            GameObject skillRoot = Util.FindChild(_charBase.gameObject, SkillRoot, false);
            if (skillRoot == null)
            {
                skillRoot = new GameObject(SkillRoot);
            }
            _SkillRoot = skillRoot.transform;

            // ��ų �߰�
            for (int i = 0; i < skillArray.Count; i++)
            {
                AddSkill(skillArray[i], i);
            }
        }

        //public void DeleteSkill(eInputSystem skillIndex)
        //{
        //    if (_dicSkill == null)
        //        return;

        //    if (_dicSkill.ContainsKey(skillIndex))
        //    {
        //        _dicSkill.Remove(skillIndex);
        //    }
        //}
        
        public void AddSkill(long skillIndex, int idx)
        {
            if (_dicSkill == null || skillIndex == SystemConst.NO_CONTENT)
                return;

            if (!_dicSkill.ContainsKey(idx + 1))
            {
                SkillBase skillBase = SkillCreator.CreateSkill(skillIndex);
                if (skillBase == null)
                    return;

                skillBase.SetCharBase(_charBase);
                _dicSkill.Add(skillIndex, skillBase);
                skillBase.transform.parent = _SkillRoot;
            }
        }
        public void PlaySkill(long skillIndex, InputParameter parameter)
        {
            if (_dicSkill == null)
                return;

            if (_dicSkill.ContainsKey(skillIndex))
            {
                _dicSkill[skillIndex].PlaySkill(parameter);
            }
        }

    }
}