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
        
        // TODO : 이것도 Dictionary를 통해 연결해야하나?
        private long _currentAutoAttack;
        private long _currentSkill;

        private readonly long _defaultAutoAttack;
        private readonly long _defaultSkill;
        
        public Dictionary<long, SkillBase> DicSkill => _dicSkill; // 스킬 리스트

        
        public CharSKillInfo(CharBase charBase)
        {
            _charBase = charBase;
            //TODO : 이 부분 AddSkill에서 자연스럽게 할당할 수 있도록 다시 리팩토링하기
            _currentAutoAttack = _defaultAutoAttack = charBase.CharData.skill1;
            _currentSkill = _defaultSkill = charBase.CharData.skill2;
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
            if (!skillRoot)
            {
                skillRoot = new GameObject(SkillRoot);
            }
            _SkillRoot = skillRoot.transform;

            // 스킬 추가
            for (int i = 0; i < skillArray.Count; i++)
            {
                AddSkill(skillArray[i]);
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

        public void ChangeSkill(CharAI.eAttackMode changingMode, long changingIndex)
        {
            AddSkill(changingIndex);
            if(changingMode == CharAI.eAttackMode.Auto)
                _currentAutoAttack = changingIndex;
            else if(changingMode == CharAI.eAttackMode.Skill)
                _currentSkill = changingIndex;
        }

        public void ResetSkill()
        {
            _currentSkill = _defaultSkill;
            _currentAutoAttack = _defaultAutoAttack;
        }
        
        private void AddSkill(long skillIndex)
        {
            if (_dicSkill == null || skillIndex == SystemConst.NO_CONTENT)
                return;
            
            if (!_dicSkill.ContainsKey(skillIndex))
            {
                SkillBase skillBase = SkillCreator.CreateSkill(skillIndex);
                if (!skillBase)
                    return;
                skillBase.SetCharBase(_charBase);
                _dicSkill.Add(skillIndex, skillBase);
                skillBase.transform.parent = _SkillRoot;
            }
            
            /*
            //if (!_dicSkill.ContainsKey(idx + 1))
            //{
            //    SkillBase skillBase = SkillCreator.CreateSkill(skillIndex);
            //    if (skillBase == null)
            //        return;

            //    skillBase.SetCharBase(_charBase);
            //    _dicSkill.Add(skillIndex, skillBase);
            //    skillBase.transform.parent = _SkillRoot;
            //}*/
        }
        private void PlaySkill(long skillIndex, SkillParameter parameter)
        {
            if (_dicSkill == null)
                return;

            if (_dicSkill.ContainsKey(skillIndex))
            {
                _dicSkill[skillIndex].PlaySkill(parameter);
            }
        }

        public void PlayByMode(CharAI.eAttackMode mode, SkillParameter parameter)
        {
            if(mode == CharAI.eAttackMode.Auto)
                PlaySkill(_currentAutoAttack, parameter);
            else if(mode == CharAI.eAttackMode.Skill)
                PlaySkill(_currentSkill, parameter);
        }
        
        public SkillAIInfo GetInfoByMode(CharAI.eAttackMode mode)
        {
            if (mode == CharAI.eAttackMode.Auto)
                return _dicSkill[_currentAutoAttack].GetAIInfo();
            else if (mode == CharAI.eAttackMode.Skill)
                return _dicSkill[_currentAutoAttack].GetAIInfo();

            return default;
        }
    }

    public struct SkillAIInfo
    {
        public SystemEnum.eSkillTargetType TargetType;
        public int Range;
    }
}