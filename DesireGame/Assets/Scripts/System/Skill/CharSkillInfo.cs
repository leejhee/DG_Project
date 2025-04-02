using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 캐릭터 스킬 정보 (캐릭터 Start시점에 생성)
    /// </summary>
    public class CharSKillInfo
    {
        private Dictionary<long, SkillBase> _dicSkill = new Dictionary<long, SkillBase>(); // 스킬 리스트
        private CharBase _charBase; // 스킬 시전자
        private Transform _SkillRoot; // 스킬 루트 

        public Dictionary<long, SkillBase> DicSkill => _dicSkill; // 스킬 리스트

        public CharSKillInfo(CharBase charBase)
        {
            _charBase = charBase;
        }

        /// <summary>
        /// Char Start 시점
        /// </summary>
        /// <param name="skillArray"></param>
        public void Init(List<long> skillArray)
        {
            // 스킬 루트 오브젝트 제작
            string SkillRoot = "SkillRoot";
            GameObject skillRoot = Util.FindChild(_charBase.gameObject, SkillRoot, false);
            if (skillRoot == null)
            {
                skillRoot = new GameObject(SkillRoot);
            }
            _SkillRoot = skillRoot.transform;

            // 스킬 추가
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
        public void PlaySkill(long skillIndex, SkillParameter parameter)
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